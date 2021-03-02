using System;
using System.Collections.Generic;
using System.Linq;
using Quaver.API.Enums;
using Quaver.API.Maps.AutoMod.Issues;
using Quaver.API.Maps.AutoMod.Issues.Autoplay;
using Quaver.API.Maps.AutoMod.Issues.HitObjects;
using Quaver.API.Maps.AutoMod.Issues.ScrollVelocities;
using Quaver.API.Maps.AutoMod.Issues.TimingPoints;
using Quaver.API.Maps.Structures;
using Quaver.API.Replays;
using Quaver.API.Replays.Virtual;

namespace Quaver.API.Maps.AutoMod
{
    public class AutoMod
    {
        /// <summary>
        /// </summary>
        public Qua Qua { get; }

        /// <summary>
        /// </summary>
        public List<AutoModIssue> Issues { get; private set; } = new List<AutoModIssue>();

        /// <summary>
        ///     The amount of time in milliseconds where a long note would be considered too short
        /// </summary>
        public const int ShortLongNoteThreshold = 36;

        /// <summary>
        ///     The amount in time in milliseconds where two objects are considered to be overlapping
        /// </summary>
        public const int OverlappingObjectsThreshold = 10;

        /// <summary>
        ///     The amount of time in milliseconds where a break would be considered too excessive
        ///     and against the ranking criteria.
        /// </summary>
        public const int BreakTime = 30000;

        /// <summary>
        /// </summary>
        /// <param name="qua"></param>
        public AutoMod(Qua qua) => Qua = qua;

        /// <summary>
        ///     Starts running through all AutoMod checks for this map.
        /// </summary>
        public void Run()
        {
            Issues = new List<AutoModIssue>();

            DetectHitObjectIssues();
            DetectTimingPointIssues();
            DetectScrollVelocityIssues();
            DetectAutoplayIssues();
        }

        /// <summary>
        ///     Detects issues related to HitObjects.
        ///
        ///     It will check for the following:
        ///         - Overlapping Notes
        ///         - Short Long Notes
        ///         - Notes that are placed before the audio begins
        ///         - There must be one note in every column
        ///         - 30s+ break time
        /// </summary>
        private void DetectHitObjectIssues()
        {
            var previousNoteInColumns = new List<HitObjectInfo>();

            for (var i = 0; i < Qua.GetKeyCount(); i++)
                previousNoteInColumns.Add(null);

            for (var i = 0; i < Qua.HitObjects.Count; i++)
            {
                var hitObject = Qua.HitObjects[i];
                var laneIndex = hitObject.Lane - 1;

                // Check if the long note is too short
                if (hitObject.IsLongNote && Math.Abs(hitObject.EndTime - hitObject.StartTime) <= ShortLongNoteThreshold)
                    Issues.Add(new AutoModIssueShortLongNote(hitObject));

                // Check if the object is before the object is before the audio begins
                if (hitObject.StartTime < 0 || ( hitObject.IsLongNote && hitObject.EndTime < 0 ))
                    Issues.Add(new AutoModIssueObjectBeforeStart(hitObject));

                // Any checks below this point require the previous object in the column, so don't run
                // for the first object in the map.
                if (hitObject == Qua.HitObjects.First())
                {
                    previousNoteInColumns[laneIndex] = hitObject;
                    continue;
                }

                var previousObjInMap = Qua.HitObjects[i - 1];

                // Check for excessive break time.
                if (hitObject.StartTime - previousObjInMap.StartTime >= BreakTime ||
                   (previousObjInMap.IsLongNote && hitObject.StartTime - previousObjInMap.EndTime >= BreakTime))
                {
                    Issues.Add(new AutoModIssueExcessiveBreakTime(hitObject));
                }

                // Retrieve the previous object in the column if one exists.
                var prevColObject = previousNoteInColumns[laneIndex];

                if (prevColObject == null)
                {
                    previousNoteInColumns[laneIndex] = hitObject;
                    continue;
                }

                // Check if the objects are overlapping in start times
                if (Math.Abs(hitObject.StartTime - prevColObject.StartTime) <= OverlappingObjectsThreshold)
                    Issues.Add(new AutoModIssueOverlappingObjects(new[] {hitObject, prevColObject}));

                // Check for long note overlaps
                if (prevColObject.IsLongNote)
                {
                    // Check if the object is overlapping the previous object's long note end.
                    if (Math.Abs(hitObject.StartTime - prevColObject.EndTime) <= OverlappingObjectsThreshold)
                        Issues.Add(new AutoModIssueOverlappingObjects(new[] {hitObject, prevColObject}));

                    // Check if the object is "inside" of the previous long note.
                    if (hitObject.StartTime >= prevColObject.StartTime && hitObject.StartTime <= prevColObject.EndTime)
                        Issues.Add(new AutoModIssueOverlappingObjects(new[] {hitObject, prevColObject}));
                }

                previousNoteInColumns[hitObject.Lane - 1] = hitObject;
            }

            DetectMissingObjectInColumns(previousNoteInColumns);
        }

        /// <summary>
        ///     Detects if each column has an object placed inside of it.
        /// </summary>
        private void DetectMissingObjectInColumns(List<HitObjectInfo> previousNoteInColumns)
        {
            var columnsMissing = new List<int>();

            for (var i = 0; i < previousNoteInColumns.Count; i++)
            {
                if (previousNoteInColumns[i] == null)
                    columnsMissing.Add(i + 1);
            }

            Issues.Add(new AutoModIssueObjectInAllColumns(columnsMissing));
        }

        /// <summary>
        ///     Detects issues related to timing points
        ///
        ///     It will check for the following:
        ///         - Overlapping Timing Points
        /// </summary>
        private void DetectTimingPointIssues()
        {
            for (var i = 0; i < Qua.TimingPoints.Count; i++)
            {
                var current = Qua.TimingPoints[i];

                if (i == 0)
                    continue;

                var previous = Qua.TimingPoints[i - 1];

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (current.StartTime == previous.StartTime)
                    Issues.Add(new AutoModeIssueTimingPointOverlap(new [] { current, previous }));
            }
        }

        /// <summary>
        ///     Detects issues related to scroll velocities
        /// </summary>
        private void DetectScrollVelocityIssues()
        {
            for (var i = 0; i < Qua.SliderVelocities.Count; i++)
            {
                var current = Qua.SliderVelocities[i];

                if (i == 0)
                    continue;

                var previous = Qua.SliderVelocities[i - 1];

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (current.StartTime == previous.StartTime)
                    Issues.Add(new AutoModIssueScrollVelocityOverlap(new []{ current, previous }));
            }
        }

        /// <summary>
        ///     Detects issues related to Autoplay (It should be able to 100%)
        /// </summary>
        private void DetectAutoplayIssues()
        {
            if (Qua.HitObjects.Count <= 1)
                return;

            var replay = new Replay(Qua.Mode, "", ModIdentifier.Autoplay, "");
            replay = Replay.GeneratePerfectReplayKeys(replay, Qua);

            var virtualReplayPlayer = new VirtualReplayPlayer(replay, Qua);
            virtualReplayPlayer.PlayAllFrames();

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (virtualReplayPlayer.ScoreProcessor.Accuracy == 100)
                return;

            Issues.Add(new AutoModIssueAutoplayFailure());
        }
    }
}