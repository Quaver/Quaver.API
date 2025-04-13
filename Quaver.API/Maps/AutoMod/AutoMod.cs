using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ATL;
using Quaver.API.Enums;
using Quaver.API.Maps.AutoMod.Issues;
using Quaver.API.Maps.AutoMod.Issues.Audio;
using Quaver.API.Maps.AutoMod.Issues.Autoplay;
using Quaver.API.Maps.AutoMod.Issues.HitObjects;
using Quaver.API.Maps.AutoMod.Issues.Images;
using Quaver.API.Maps.AutoMod.Issues.Map;
using Quaver.API.Maps.AutoMod.Issues.Metadata;
using Quaver.API.Maps.AutoMod.Issues.ScrollVelocities;
using Quaver.API.Maps.AutoMod.Issues.TimingGroups;
using Quaver.API.Maps.AutoMod.Issues.TimingPoints;
using Quaver.API.Maps.Structures;
using Quaver.API.Replays;
using Quaver.API.Replays.Virtual;
using SixLabors.ImageSharp;

namespace Quaver.API.Maps.AutoMod
{
    public class AutoMod
    {
        /// <summary>
        /// </summary>
        public Qua Qua { get; }

        /// <summary>
        /// </summary>
        public Track AudioTrackInfo { get; private set; }

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
        ///     The amount of time in milliseconds a map must be to be considered rankable
        /// </summary>
        public const int RequiredMapLength = 45000;

        /// <summary>
        ///     The max file size of a background in bytes.
        /// </summary>
        public const int MaxBackgroundFileSize = 4000000;

        /// <summary>
        ///     The max file size of a background in bytes.
        /// </summary>
        public const int MaxBannerFileSize = 2000000;

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

            LoadAudioTrackData();
            DetectHitObjectIssues();
            DetectTimingPointIssues();
            DetectScrollVelocityIssues();
            DetectAutoplayIssues();
            DetectMapLengthIssues();
            DetectMetadataIssues();
            DetectPreviewPointIssues();
            DetectImageIssues();
            DetectAudioFileIssues();
        }

        /// <summary>
        ///     Loads audio track metadata.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void LoadAudioTrackData()
        {
            var path = Qua.GetAudioPath();

            if (!File.Exists(path))
                return;

            try
            {
                AudioTrackInfo = new Track(path, true);
            }
            catch (Exception)
            {
                // ignored
            }
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

                if (!Qua.TimingGroups.ContainsKey(hitObject.TimingGroup))
                    Issues.Add(new AutoModIssueObjectInvalidTimingGroup(hitObject));

                // Check if the long note is too short
                if (hitObject.IsLongNote && Math.Abs(hitObject.EndTime - hitObject.StartTime) < ShortLongNoteThreshold)
                    Issues.Add(new AutoModIssueShortLongNote(hitObject));

                // Check if the object is before the object is before the audio begins
                if (hitObject.StartTime < 0 || (hitObject.IsLongNote && hitObject.EndTime < 0))
                    Issues.Add(new AutoModIssueObjectBeforeStart(hitObject));

                // Check if object is after the audio ends
                if (AudioTrackInfo != null && (hitObject.StartTime > AudioTrackInfo.DurationMs
                                                || hitObject.EndTime > AudioTrackInfo.DurationMs))
                {
                    Issues.Add(new AutoModIssueObjectAfterAudioEnd(hitObject));
                }

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
                    Issues.Add(new AutoModIssueExcessiveBreakTime(previousObjInMap));
                }

                // Retrieve the previous object in the column if one exists.
                var prevColObject = previousNoteInColumns[laneIndex];

                if (prevColObject == null)
                {
                    previousNoteInColumns[laneIndex] = hitObject;
                    continue;
                }

                // Check for long note overlaps
                if (prevColObject.IsLongNote)
                {
                    // Check if the object is overlapping the previous object's long note end.
                    if (Math.Abs(hitObject.StartTime - prevColObject.EndTime) <= OverlappingObjectsThreshold)
                        Issues.Add(new AutoModIssueOverlappingObjects(new[] { hitObject, prevColObject }));

                    // Check if the object is "inside" of the previous long note.
                    if (hitObject.StartTime >= prevColObject.StartTime && hitObject.StartTime < prevColObject.EndTime - OverlappingObjectsThreshold)
                        Issues.Add(new AutoModIssueOverlappingObjects(new[] { hitObject, prevColObject }));
                }
                else
                {
                    // Check if the objects are overlapping in start times
                    if (Math.Abs(hitObject.StartTime - prevColObject.StartTime) <= OverlappingObjectsThreshold)
                        Issues.Add(new AutoModIssueOverlappingObjects(new[] { hitObject, prevColObject }));
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

            if (columnsMissing.Count > 0)
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

                if (AudioTrackInfo != null && (current.StartTime > AudioTrackInfo.DurationMs))
                    Issues.Add(new AutoModIssueTimingPointAfterAudioEnd(current));

                if (i == 0)
                    continue;

                var previous = Qua.TimingPoints[i - 1];

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (current.StartTime == previous.StartTime)
                    Issues.Add(new AutoModeIssueTimingPointOverlap(new[] { current, previous }));
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

                if (AudioTrackInfo != null && (current.StartTime > AudioTrackInfo.DurationMs))
                    Issues.Add(new AutoModIssueScrollVelocityAfterEnd(current));

                if (i == 0)
                    continue;

                var previous = Qua.SliderVelocities[i - 1];

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (current.StartTime == previous.StartTime)
                    Issues.Add(new AutoModIssueScrollVelocityOverlap(new[] { current, previous }));
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

        /// <summary>
        ///     Detects issues related to map length
        /// </summary>
        private void DetectMapLengthIssues()
        {
            if (Qua.Length < RequiredMapLength)
                Issues.Add(new AutoModIssueMapLength());
        }

        /// <summary>
        ///     Detects issues related to the map's preview point.
        /// </summary>
        private void DetectPreviewPointIssues()
        {
            if (Qua.SongPreviewTime <= 0)
                Issues.Add(new AutoModIssuePreviewPoint());
        }

        /// <summary>
        ///     Detects issues related to the map's metadata.
        /// </summary>
        private void DetectMetadataIssues()
        {
            DetectNonRomanizedMetadata();
            DetectNonStandardizedMetadata();
            DetectNonCommaSeparatedTags();
        }

        /// <summary>
        ///     Detects if any portion of the metadata uses non-romanized characters.
        /// </summary>
        private void DetectNonRomanizedMetadata()
        {
            if (HasNonAsciiCharacters(Qua.Artist))
                Issues.Add(new AutoModIssueNonRomanized("Artist"));

            if (HasNonAsciiCharacters(Qua.Title))
                Issues.Add(new AutoModIssueNonRomanized("Title"));
        }

        /// <summary>
        ///     Detects if metadata violates standardization rules.
        /// </summary>
        private void DetectNonStandardizedMetadata()
        {
            DetectNonStandardizedField("Artist", Qua.Artist);
            DetectNonStandardizedField("Title", Qua.Title);
        }

        /// <summary>
        ///     Detects if a given field violates standardization rules.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        private void DetectNonStandardizedField(string fieldName, string fieldValue)
        {
            var vsMatch = new Regex(@"\b(vs\.?|versus\.?)", RegexOptions.IgnoreCase).Match(fieldValue);
            if (!fieldValue.Contains("vs.") && vsMatch.Success)
                Issues.Add(new AutoModIssueNonStandardizedMetadata(fieldName, vsMatch.Groups[1].Value, "vs."));

            var featMatch = new Regex(@"\b(ft[\s|.]|feat\s)", RegexOptions.IgnoreCase).Match(fieldValue);
            if (featMatch.Success)
                Issues.Add(new AutoModIssueNonStandardizedMetadata(fieldName, featMatch.Groups[1].Value, "feat."));
        }

        private void DetectNonCommaSeparatedTags()
        {
            if (Qua.Tags == null)
                return;

            if (Qua.Tags.Length > 10 && !Qua.Tags.Contains(','))
                Issues.Add(new AutoModIssueNonCommaSeparatedTags());
        }

        /// <summary>
        ///     Checks a string for non-ascii characters
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool HasNonAsciiCharacters(string str) => str.Any(c => c > 128);

        /// <summary>
        ///     Detects issues related to the background and banner files:
        ///     - File too large
        ///     - Resolution outsize valid range
        /// </summary>
        private void DetectImageIssues()
        {
            DetectBackgroundIssues();
            DetectBannerIssues();
        }

        private void DetectBackgroundIssues()
        {
            var bgPath = Qua.GetBackgroundPath();
            if (bgPath == null || !File.Exists(bgPath))
            {
                Issues.Add(new AutoModIssueNoBackground());
                return;
            }

            DetectImageFileIssues("background", bgPath, MaxBackgroundFileSize, 1280, 720, 2560, 1440);
        }

        private void DetectBannerIssues()
        {
            var bannerPath = Qua.GetBannerPath();
            if (bannerPath == null || !File.Exists(bannerPath)) return;

            DetectImageFileIssues("banner", bannerPath, MaxBannerFileSize, 421, 82, 1263, 246);
        }

        /// <summary>
        ///     Detects issues related to a given image file:
        ///     - File too large
        ///     - Resolution outsize valid range
        /// </summary>
        /// <param name="item"></param>
        /// <param name="path"></param>
        /// <param name="maxSize"></param>
        /// <param name="minWidth"></param>
        /// <param name="minHeight"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        private void DetectImageFileIssues(string item, string path, int maxSize, int minWidth, int minHeight, int maxWidth, int maxHeight)
        {
            // Check background file size
            if (new FileInfo(path).Length > maxSize)
                Issues.Add(new AutoModIssueImageTooLarge(item, maxSize));

            try
            {
                using (var image = Image.Load(path))
                    if (image.Width < minWidth || image.Height < minHeight || image.Width > maxWidth || image.Height > maxHeight)
                        Issues.Add(new AutoModIssueImageResolution(item, minWidth, minHeight, maxWidth, maxHeight));
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        ///     Detects issues with the audio file if one exists
        /// </summary>
        private void DetectAudioFileIssues()
        {
            if (AudioTrackInfo == null)
                return;

            if (Path.GetExtension(AudioTrackInfo.Path).ToLower() != ".mp3")
                Issues.Add(new AutoModIssueAudioFormat());

            if (AudioTrackInfo.Bitrate > 192)
                Issues.Add(new AutoModIssueAudioBitrate());
        }
    }
}
