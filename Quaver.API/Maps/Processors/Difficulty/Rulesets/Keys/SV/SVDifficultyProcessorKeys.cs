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

        // Representation of the map's density
        // Should be expressed in 4 Key actions per second
        public float ActionsPerSecond { get; private set; }

        // Any map densities greater than this are treated as this number instead
        // Should be expressed in 4 Key actions per second
        public float MaxDensity { get; private set; }

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

        // Time each note (including LN ends) is visible for
        // Should be expressed in ms
        public Dictionary<int, float> NoteVisibilities { get; private set; } = new Dictionary<int, float>();

        // Notes with less visibility than this are considered invisible
        // Should be expressed in ms
        public float VisibilityThreshold { get; private set; }

        // Notes with less visibility than this are considered impossible to react to
        // Should be expressed in ms
        public float ReactionThreshold { get; private set; }

        public SVDIfficultyProcessorKeys(Qua map, int baseReadingHeight = 250, int playfieldHeight = 450,
                                         float visibilityThreshold = 1000 / 60f, float reactionThreshold = 150,
                                         float maxDensity = 18)
        {
            Map = map;

            BaseReadingHeight = baseReadingHeight;
            PlayfieldHeight = playfieldHeight;
            VisibilityThreshold = visibilityThreshold;
            ReactionThreshold = reactionThreshold;

            MaxDensity = maxDensity / Map.GetKeyCount() * 4;
            ActionsPerSecond = Map.GetActionsPerSecond() / Map.GetKeyCount() * 4;

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
            CalculateNoteVisibilities();

            ComputeNoteVisibilityFactor();
            ComputeNoteSpacingFactor();
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

        private void CalculateNoteVisibilities()
        {
            // calculate visibility for each important note time
            foreach (int time in NoteTimes)
            {
                float visibility = 0; // in ms

                // playfield positions when the note is at the top or bottom of the playfield
                long playfieldPositionNoteOnBottom = Positions[time];
                long playfieldPositionNoteOnTop = playfieldPositionNoteOnBottom - (long)(PlayfieldHeight * TrackRounding);

                // times when the note enters or exits the playfield area
                List<float> intersectionTimes = new List<float>();

                // used when scrolling stops when a velocity position is directly on a playfield boundary position
                int prevDirection = Math.Sign(Map.InitialScrollVelocity);

                // if gameplay starts with note visible in playfield area,
                // add intersection time to represent the note "entering" the playfield area
                if (PositionBetween(GetPositionFromTime(-3000), playfieldPositionNoteOnTop, playfieldPositionNoteOnBottom))
                    intersectionTimes.Add(-3000);

                // find intersections from -3000 ms to the first sv
                if (Map.SliderVelocities[0].StartTime > -3000)
                {
                    if (PositionBetween(playfieldPositionNoteOnTop, GetPositionFromTime(-3000), VelocityPositionMarkers[0]))
                        intersectionTimes.Add(GetTimeFromPosition(playfieldPositionNoteOnTop, GetPositionFromTime(-3000), Map.InitialScrollVelocity, -3000));

                    if (PositionBetween(playfieldPositionNoteOnBottom, GetPositionFromTime(-3000), VelocityPositionMarkers[0]))
                        intersectionTimes.Add(GetTimeFromPosition(playfieldPositionNoteOnBottom, GetPositionFromTime(-3000), Map.InitialScrollVelocity, -3000));
                }

                // find intersections during the normal length of the map
                for (int i = 1; i < VelocityPositionMarkers.Count; i++)
                {
                    // times past the note time are irrelevant to visibility
                    if (Map.SliderVelocities[i - 1].StartTime > time)
                        break;

                    // times before -3000 ms aren't seen by the player
                    if (Map.SliderVelocities[i - 1].StartTime < -3000)
                        continue;

                    // if first velocity position is equal to a playfield boundary position
                    // whether to add another intersection time depends on the scroll direction
                    // previous iteration will always add to intersection times
                    if (VelocityPositionMarkers[i - 1] == playfieldPositionNoteOnBottom || VelocityPositionMarkers[i - 1] == playfieldPositionNoteOnTop)
                    {
                        int currentDirection = Math.Sign(Map.SliderVelocities[i - 1].Multiplier);

                        // skip any 0x SVs while in this state
                        if (currentDirection == 0)
                            continue;

                        // scrolling doesn't reverse and so there is only one intersection (one enter/exit event)
                        if (currentDirection == prevDirection)
                            continue;

                        // currentDirection != prevDirection
                        // scrolling reverses and so enters and then exits the playfield area
                        intersectionTimes.Add(Map.SliderVelocities[i - 1].StartTime);
                        prevDirection *= -1;
                        continue;
                    }

                    if (PositionBetween(playfieldPositionNoteOnTop, VelocityPositionMarkers[i - 1], VelocityPositionMarkers[i]))
                    {
                        var sv = Map.SliderVelocities[i - 1];
                        intersectionTimes.Add(GetTimeFromPosition(playfieldPositionNoteOnTop, VelocityPositionMarkers[i - 1], sv.Multiplier, sv.StartTime));
                    }

                    if (PositionBetween(playfieldPositionNoteOnBottom, VelocityPositionMarkers[i - 1], VelocityPositionMarkers[i]))
                    {
                        var sv = Map.SliderVelocities[i - 1];
                        intersectionTimes.Add(GetTimeFromPosition(playfieldPositionNoteOnBottom, VelocityPositionMarkers[i - 1], sv.Multiplier, sv.StartTime));
                    }

                    prevDirection = Math.Sign(Map.SliderVelocities[i - 1].Multiplier);
                }

                // if note is during last sv:
                //     playfield top intersects with note if scroll direction is positive
                //     playfield bottom intersects with note
                if (time > Map.SliderVelocities.Last().StartTime)
                {
                    if (Math.Sign(Map.SliderVelocities.Last().Multiplier) == 1)
                    {
                        var sv = Map.SliderVelocities.Last();
                        intersectionTimes.Add(GetTimeFromPosition(playfieldPositionNoteOnTop, VelocityPositionMarkers.Last(), sv.Multiplier, sv.StartTime));
                    }

                    intersectionTimes.Add(time);
                }

                // reverse scrolling may cause order to be incorrect
                intersectionTimes.Sort();

                // in the case a map reverse scrolls into a note, there will be an odd number of intersections
                // in this case the last intersection time contributes no visibility, and so can be removed
                if (intersectionTimes.Count % 2 == 1)
                    intersectionTimes.RemoveAt(intersectionTimes.Count - 1);

                // calculate total time visible from intersection times
                // each pair of intersections is one playfield enter and one playfield exit
                for (int i = 0; i < intersectionTimes.Count - 1; i += 2)
                {
                    visibility += intersectionTimes[i + 1] - intersectionTimes[i];
                }

                NoteVisibilities[time] = visibility;
            }
        }

        public bool IsNoteReactable(int time) => NoteVisibilities[time] >= ReactionThreshold;

        public bool IsNoteVisible(int time) => NoteVisibilities[time] >= VisibilityThreshold;

        private bool PositionBetween(long position, long a, long b)
        {
            if (a < b)
                return (a <= position && position <= b);
            else
                return (b <= position && position <= a);
        }

        private void ComputeNoteVisibilityFactor()
        {
            double sum = 0;

            foreach (int time in NoteTimes)
            {
                // lower limit of human reaction time is about 150 ms
                // convert visibility of [150, playfield time] => visibility factor [0, 1]
                float visibility = NoteVisibilities[time];
                visibility = Math.Min(Math.Max(visibility, 150), PlayfieldHeight);
                sum += Math.Pow(1 - (visibility - 150) / (PlayfieldHeight - 150), 2);
            }

            NoteVisibilityFactor = (float)(Math.Sqrt(sum / NoteTimes.Count));
        }

        private void ComputeNoteSpacingFactor()
        {
            if (Map.HitObjects.Count < 2)
            {
                NoteSpacingFactor = 0;
                return;
            }

            double sum = 0;
            int n = NoteTimes.Count - 1;

            for (int i = 1; i < NoteTimes.Count; i++)
            {
                if (!IsNoteVisible(NoteTimes[i - 1]) || !IsNoteVisible(NoteTimes[i]))
                {
                    n--;
                    continue;
                }

                long svSpacing = Math.Abs(Positions[NoteTimes[i]] - Positions[NoteTimes[i - 1]]);
                long normalSpacing = GetPositionFromTimeWithoutSV(NoteTimes[i]) - GetPositionFromTimeWithoutSV(NoteTimes[i - 1]);

                // normalSpacing should theoretically never be 0
                // svSpacing could be any number 0 or greater
                float relativeSpacing = svSpacing / (float)normalSpacing;
                relativeSpacing = Math.Min(relativeSpacing, 1);

                // assuming perceived difficulty increases linearly with some geometric change in density
                double densityLevel = -1 * Math.Log(relativeSpacing, Math.Sqrt(2));

                double difficulty = densityLevel / 6;

                // reduce perceived difficulty based off of map's normal density
                difficulty *= Math.Min(ActionsPerSecond / MaxDensity, 1);

                // make sure we don't get infinity difficulty lol
                difficulty = Math.Min(difficulty, 1);

                sum += Math.Pow(difficulty, 2);
            }

            NoteSpacingFactor = (float)(Math.Sqrt(sum / (NoteTimes.Count - 1)));
        }

        private void ComputeReadingHeightFactor()
        {
            // currently treats notes above the screen and notes below the screen as different
            // when in reality they are both offscreen and so should be treated the same

            // tiny flickers can cause this calculation to not be reprsentative of the true reading height

            double sum = 0;
            int n = NoteTimes.Count;
            float lastReadingHeight = BaseReadingHeight;

            foreach (int time in NoteTimes)
            {
                if (!IsNoteReactable(time))
                {
                    n--;
                    continue;
                }

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

            ReadingHeightFactor = (float)(Math.Sqrt(sum / n));
        }

        private void ComputeOverallSVDifficulty()
        {
            SVDifficulty = (NoteSpacingFactor + NoteVisibilityFactor + ReadingHeightFactor) / 3 * 100;
            //SVDifficulty = ReadingHeightFactor * 100;
        }

        public float GetTimeFromPosition(long position, long svPosition, float svMultiplier, float svTime)
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