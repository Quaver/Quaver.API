using Quaver.API.Maps;
using Quaver.API.Maps.Structures;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys
{
    public class SVDIfficultyProcessorKeys
    {
        public static string Version { get; } = "0.0.1";

        public Qua Map { get; private set; }

        public float SVDifficulty { get; private set; }

        // Assumed comfortable reading height of a player, expressed in ms
        public int BaseReadingHeight { get; set; }

        // Assumed playfield height, expressed in ms
        public int PlayfieldHeight { get; set; }

        // How much the spacing between notes affects difficulty
        public float NoteSpacingFactor { get; private set; }

        // How much the time notes spend on screen affects difficulty
        public float NoteVisibilityFactor { get; private set; }

        // How much changes in reading height affect difficulty
        public float ReadingHeightFactor { get; private set; }

        // Anything regarding position calculations is currently just copy pasted from HitObjectManagerKeys.cs
        public List<long> VelocityPositionMarkers { get; private set; } = new List<long>();

        /// <summary>
        ///     Used to Round TrackPosition from Long to Float
        /// </summary>
        public static float TrackRounding { get; } = 100;

        // Times important to SV difficulty calculations
        // Should include all note StartTimes and all LN EndTimes
        public List<int> NoteTimes { get; private set; } = new List<int>();

        // Positions important to SV difficulty calculations
        // Should include the position(s) of every note
        public Dictionary<int, long> Positions { get; private set; } = new Dictionary<int, long>();

        public SVDIfficultyProcessorKeys(Qua map, int baseReadingHeight = 250, int playfieldHeight = 450)
        {
            Map = map;
            BaseReadingHeight = baseReadingHeight;
            PlayfieldHeight = playfieldHeight;

            CalculateSVDifficulty();
        }

        public void CalculateSVDifficulty()
        {
            if (Map == null || Map.HitObjects.Count == 0) return;

            Map.NormalizeSVs();

            if (Map.SliderVelocities.Count == 0)
            {
                SVDifficulty = 0;
                return;
            }

            InitializePositionMarkers();
            InitializeNoteTimes();
            CalculateImportantPositions();

            ComputeNoteSpacingFactor();
            ComputeNoteVisibilityFactor();
            ComputeReadingHeightFactor();

            ComputeOverallSVDifficulty();
        }

        private void InitializeNoteTimes()
        {
            foreach (var hitObject in Map.HitObjects)
            {
                if (!NoteTimes.Contains(hitObject.StartTime))
                    NoteTimes.Add(hitObject.StartTime);

                if (hitObject.IsLongNote && !NoteTimes.Contains(hitObject.EndTime))
                    NoteTimes.Add(hitObject.EndTime);
            }

            NoteTimes.Sort();
        }

        private void CalculateImportantPositions()
        {
            foreach (var time in NoteTimes)
                Positions[time] = GetPositionFromTime(time);
        }

        private void ComputeNoteSpacingFactor()
        {
            double sum = 0f;

            for (int i = 1; i < NoteTimes.Count; i++)
            {
                long svSpacing = Math.Abs(Positions[NoteTimes[i]] - Positions[NoteTimes[i - 1]]);
                long normalSpacing = GetPositionFromTimeWithoutSV(NoteTimes[i]) - GetPositionFromTimeWithoutSV(NoteTimes[i - 1]);

                // normalSpacing should theoretically never be 0
                // svSpacing could be any number 0 or greater
                float ratio = svSpacing / (float)normalSpacing;
                ratio = Math.Min(ratio, 1);

                sum += Math.Pow(1 - ratio, 2);
            }

            NoteSpacingFactor = (float)(Math.Sqrt(sum / (NoteTimes.Count - 1)));
        }

        private void ComputeNoteVisibilityFactor()
        {
            // uses the earliest appearance of a note to determine its visibility
            // currently any amount of time a note theoretically spends on screen counts
            // even if it's only on screen for less than a frame
            // or if it's hidden under another note at the same position

            double sum = 0;

            foreach (int time in NoteTimes)
            {
                long notePosition = Positions[time];
                long earliestVisiblePosition = notePosition - (long)(PlayfieldHeight * TrackRounding);
                float earliestVisibleTime = GetEarliestTimeFromPosition(earliestVisiblePosition);
                float reactionTime = time - earliestVisibleTime;

                // upper limit of human reaction time is about 150 ms
                // convert reaction time of [150, playfield time] => visibility factor [0, 1]
                reactionTime = Math.Min(Math.Max(reactionTime, 150), PlayfieldHeight);
                sum += Math.Pow(1 - (reactionTime - 150) / (PlayfieldHeight - 150), 2);
            }

            NoteVisibilityFactor = (float)(Math.Sqrt(sum / NoteTimes.Count));
        }

        private void ComputeReadingHeightFactor()
        {
            // currently treats notes above the screen and notes below the screen as different
            // when in reality they are both offscreen and so should be treated the same

            // tiny flickers can cause this calculation to not be reprsentative of the true reading height

            double sum = 0;
            float lastReadingHeight = BaseReadingHeight;

            foreach (int time in NoteTimes)
            {
                float playfieldTime = time - BaseReadingHeight;
                long notePosition = Positions[time];
                long playfieldPosition = GetPositionFromTime(playfieldTime);
                float readingHeight = (notePosition - playfieldPosition) / TrackRounding;

                // if reading height is offscreen set it to top of screen
                // receptor-based skins are able to see a bit lower than 0
                readingHeight = (0 <= readingHeight && readingHeight <= PlayfieldHeight) ? readingHeight : PlayfieldHeight;

                // absolute change in relative deviance
                sum += Math.Pow(Math.Abs(readingHeight - lastReadingHeight) / PlayfieldHeight, 2);
                lastReadingHeight = readingHeight;
            }

            ReadingHeightFactor = (float)(Math.Sqrt(sum / NoteTimes.Count));
        }

        private void ComputeOverallSVDifficulty()
        {
            SVDifficulty = (NoteSpacingFactor + NoteVisibilityFactor + ReadingHeightFactor) / 3 * 100;
            //SVDifficulty = ReadingHeightFactor * 100;
        }

        public float GetEarliestTimeFromPosition(long position)
        {
            int gameplayStart = -3000; // Gameplay starts at -3000 ms
            if (GetPositionFromTime(gameplayStart) <= position && position < VelocityPositionMarkers[0])
                return GetEarliestTimeFromPosition(position, GetPositionFromTime(gameplayStart), Map.InitialScrollVelocity, gameplayStart);

            // find the two earliest SVs the position can be found between
            for (int i = 1; i < VelocityPositionMarkers.Count; i++)
            {
                if (VelocityPositionMarkers[i - 1] <= position && position < VelocityPositionMarkers[i])
                {
                    var sv = Map.SliderVelocities[i - 1];
                    return GetEarliestTimeFromPosition(position, VelocityPositionMarkers[i - 1], sv.Multiplier, sv.StartTime);
                }
            }

            // c# is dumb
            var lastsv = Map.SliderVelocities.Last();
            return GetEarliestTimeFromPosition(position, VelocityPositionMarkers.Last(), lastsv.Multiplier, lastsv.StartTime);
        }

        public float GetEarliestTimeFromPosition(long position, long svPosition, float svMultiplier, float svTime)
        {
            return (position - svPosition) / svMultiplier / TrackRounding + svTime;
        }

        /// <summary>
        ///     Create SV-position points for computation optimization
        /// </summary>
        private void InitializePositionMarkers()
        {
            if (Map.SliderVelocities.Count == 0)
                return;

            // Compute for Change Points
            var position = (long)(Map.SliderVelocities[0].StartTime * Map.InitialScrollVelocity * TrackRounding);
            VelocityPositionMarkers.Add(position);

            for (var i = 1; i < Map.SliderVelocities.Count; i++)
            {
                position += (long)((Map.SliderVelocities[i].StartTime - Map.SliderVelocities[i - 1].StartTime)
                                   * Map.SliderVelocities[i - 1].Multiplier * TrackRounding);
                VelocityPositionMarkers.Add(position);
            }
        }

        /// <summary>
        ///     Get Hit Object (End/Start) position from audio time (Unoptimized.)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public long GetPositionFromTime(double time)
        {
            int i;
            for (i = 0; i < Map.SliderVelocities.Count; i++)
            {
                if (time < Map.SliderVelocities[i].StartTime)
                    break;
            }

            return GetPositionFromTime(time, i);
        }

        /// <summary>
        ///     Get Hit Object (End/Start) position from audio time and SV Index.
        ///     Index used for optimization
        /// </summary>
        /// <param name="time"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public long GetPositionFromTime(double time, int index)
        {
            if (index == 0)
            {
                // Time starts before the first SV point
                return (long) (time * Map.InitialScrollVelocity * TrackRounding);
            }

            index--;

            var curPos = VelocityPositionMarkers[index];
            curPos += (long)((time - Map.SliderVelocities[index].StartTime) * Map.SliderVelocities[index].Multiplier * TrackRounding);
            return curPos;
        }

        public long GetPositionFromTimeWithoutSV(double time) => (long)(time * TrackRounding);
    }
}