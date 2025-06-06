/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2019 Swan & The Quaver Team <support@quavergame.com>.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Force.DeepCloner;
using MonoGame.Extended.Collections;
using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.API.Maps.Processors.Difficulty;
using Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys;
using Quaver.API.Maps.Structures;
using YamlDotNet.Serialization;

namespace Quaver.API.Maps
{
    [Serializable] // ReSharper disable CognitiveComplexity CompareOfFloatsByEqualityOperator
    public class Qua
    {
        /// <summary>
        ///     The name of the audio file
        /// </summary>
        public string AudioFile { get; set; }

        /// <summary>
        ///     Time in milliseconds of the song where the preview starts
        /// </summary>
        public int SongPreviewTime { get; set; }

        /// <summary>
        ///     The name of the background file
        /// </summary>
        public string BackgroundFile { get; set; }

        /// <summary>
        ///     The name of the mapset banner
        /// </summary>
        public string BannerFile { get; set; }

        /// <summary>
        ///     The unique Map Identifier (-1 if not submitted)
        /// </summary>
        public int MapId { get; set; } = -1;

        /// <summary>
        ///     The unique Map Set identifier (-1 if not submitted)
        /// </summary>
        public int MapSetId { get; set; } = -1;

        /// <summary>
        ///     The game mode for this map
        /// </summary>
        public GameMode Mode { get; set; }

        /// <summary>
        ///     The title of the song
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The artist of the song
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        ///     The source of the song (album, mixtape, etc.)
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        ///     Any tags that could be used to help find the song.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        ///     The creator of the map
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        ///     The difficulty name of the map.
        /// </summary>
        public string DifficultyName { get; set; }

        /// <summary>
        ///     A description about this map.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The genre of the song
        /// </summary>
        public string Genre { get; set; }

        /// <summary>
        ///     Gets the value determining whether to use the old LN rendering system. (earliest/latest -> start/end)
        /// </summary>
        public bool LegacyLNRendering { get; set; }

        /// <summary>
        ///     Indicates if the BPM changes affect scroll velocity.
        ///
        ///     If this is set to false, SliderVelocities are in the denormalized format (BPM affects SV),
        ///     and if this is set to true, SliderVelocities are in the normalized format (BPM does not affect SV).
        ///
        ///     Use NormalizeSVs and DenormalizeSVs to change this value.
        ///
        ///     It's "does not affect" rather than "affects" so that the "affects" value (in this case, false) serializes to nothing to support old maps.
        /// </summary>
        public bool BPMDoesNotAffectScrollVelocity { get; set; }

        /// <summary>
        ///    The initial scroll velocity before the first SV change.
        ///
        ///    Only matters if BPMDoesNotAffectScrollVelocity is true.
        /// </summary>
        public float InitialScrollVelocity
        {
            get => DefaultScrollGroup.InitialScrollVelocity;
            set => DefaultScrollGroup.InitialScrollVelocity = value;
        }

        /// <summary>
        ///     If true, the map will have a +1 scratch key, allowing for 5/8 key play
        /// </summary>
        public bool HasScratchKey { get; set; }

        /// <summary>
        ///     EditorLayer .qua data
        /// </summary>
        public List<EditorLayerInfo> EditorLayers { get; private set; } = new List<EditorLayerInfo>();

        /// <summary>
        ///     Bookmark .qua data
        /// </summary>
        public List<BookmarkInfo> Bookmarks { get; private set; } = new List<BookmarkInfo>();

        /// <summary>
        ///     CustomAudioSamples .qua data
        /// </summary>
        public List<CustomAudioSampleInfo> CustomAudioSamples { get; set; } = new List<CustomAudioSampleInfo>();

        /// <summary>
        ///     SoundEffects .qua data
        /// </summary>
        public List<SoundEffectInfo> SoundEffects { get; private set; } = new List<SoundEffectInfo>();

        /// <summary>
        ///     TimingPoint .qua data
        /// </summary>
        public List<TimingPointInfo> TimingPoints { get; private set; } = new List<TimingPointInfo>();

        /// <summary>
        ///     Slider Velocity .qua data
        ///
        ///     Note that SVs can be both in normalized and denormalized form, depending on BPMDoesNotAffectSV.
        ///     Check WithNormalizedSVs if you need normalized SVs.
        /// </summary>
        public List<SliderVelocityInfo> SliderVelocities
        {
            get => DefaultScrollGroup.ScrollVelocities;
            private set => DefaultScrollGroup.ScrollVelocities = value;
        }

        /// <summary>
        ///     SSF for default group
        /// </summary>
        public List<ScrollSpeedFactorInfo> ScrollSpeedFactors
        {
            get => DefaultScrollGroup.ScrollSpeedFactors;
            private set => DefaultScrollGroup.ScrollSpeedFactors = value;
        }

        /// <summary>
        ///     HitObject .qua data
        /// </summary>
        public List<HitObjectInfo> HitObjects { get; private set; } = new List<HitObjectInfo>();

        public Dictionary<string, TimingGroup> TimingGroups { get; private set; } =
            new Dictionary<string, TimingGroup>();

        [YamlIgnore]
        public ScrollGroup DefaultScrollGroup { get; } = new ScrollGroup
        {
            ColorRgb = "86,254,110"
        };

        [YamlIgnore] public ScrollGroup GlobalScrollGroup => (ScrollGroup)TimingGroups[GlobalScrollGroupId];

        /// <summary>
        ///     Reserved ID for default scroll group
        /// </summary>
        public const string DefaultScrollGroupId = "$Default";

        /// <summary>
        ///     Reserved ID for global scroll group (applied to every scroll groups)
        /// </summary>
        public const string GlobalScrollGroupId = "$Global";

        /// <summary>
        ///     Finds the length of the map
        /// </summary>
        /// <returns></returns>
        [YamlIgnore]
        public int Length => HitObjects.Count is 0 ? 0 : MaxObjectTime();

        /// <summary>
        ///     Integer based seed used for shuffling the lanes when randomize mod is active.
        ///     Defaults to -1 if there is no seed.
        /// </summary>
        [YamlIgnore]
        public int RandomizeModifierSeed { get; set; } = -1;

        /// <summary>
        ///     The path of the .qua file if it is being parsed from one.
        /// </summary>
        [YamlIgnore]
        private string FilePath { get; set; }

        /// <summary>
        ///     Ctor
        /// </summary>
        public Qua()
        {
            LinkDefaultScrollGroup();
        }

        /// <summary>
        ///     Link <see cref="DefaultScrollGroup"/> to <see cref="TimingGroups"/>
        ///     so <see cref="TimingGroups"/>[<see cref="DefaultScrollGroupId"/>] points to that group.
        /// </summary>
        private void LinkDefaultScrollGroup()
        {
            TimingGroups[DefaultScrollGroupId] = DefaultScrollGroup;
            TimingGroups.TryAdd(GlobalScrollGroupId, new ScrollGroup());
        }

        /// <summary>
        ///     Returns true if the two maps are equal by value.
        /// </summary>
        /// <param name="other">the Qua to compare to</param>
        /// <returns></returns>
        public bool EqualByValue(Qua other) =>
            AudioFile == other.AudioFile &&
            SongPreviewTime == other.SongPreviewTime &&
            BackgroundFile == other.BackgroundFile &&
            BannerFile == other.BannerFile &&
            MapId == other.MapId &&
            MapSetId == other.MapSetId &&
            Mode == other.Mode &&
            Title == other.Title &&
            Artist == other.Artist &&
            Source == other.Source &&
            Tags == other.Tags &&
            Creator == other.Creator &&
            DifficultyName == other.DifficultyName &&
            Description == other.Description &&
            Genre == other.Genre &&
            TimingPoints.SequenceEqual(other.TimingPoints, TimingPointInfo.ByValueComparer) &&
            SliderVelocities.SequenceEqual(other.SliderVelocities, SliderVelocityInfo.ByValueComparer) &&
            InitialScrollVelocity == other.InitialScrollVelocity &&
            CompareTimingGroups(other.TimingGroups) &&
            BPMDoesNotAffectScrollVelocity == other.BPMDoesNotAffectScrollVelocity &&
            LegacyLNRendering == other.LegacyLNRendering &&
            HasScratchKey == other.HasScratchKey &&
            HitObjects.SequenceEqual(other.HitObjects, HitObjectInfo.ByValueComparer) &&
            CustomAudioSamples.SequenceEqual(other.CustomAudioSamples, CustomAudioSampleInfo.ByValueComparer) &&
            SoundEffects.SequenceEqual(other.SoundEffects, SoundEffectInfo.ByValueComparer) &&
            EditorLayers.SequenceEqual(other.EditorLayers, EditorLayerInfo.ByValueComparer) &&
            Bookmarks.SequenceEqual(other.Bookmarks, BookmarkInfo.ByValueComparer) &&
            RandomizeModifierSeed == other.RandomizeModifierSeed;

        private bool CompareTimingGroups(Dictionary<string, TimingGroup> other)
        {
            if (TimingGroups == null || other == null || TimingGroups.Count != other.Count)
                return false;
            foreach (var (key, value) in TimingGroups)
            {
                if (!other.TryGetValue(key, out var otherGroup) || !otherGroup.Equals(value))
                    return false;
            }

            return true;
        }

        private static IDeserializer Deserializer =>
            new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithTagMapping("!ScrollGroup", typeof(ScrollGroup))
                .Build();

        private static ISerializer Serializer =>
            new SerializerBuilder()
                .WithTagMapping("!ScrollGroup", typeof(ScrollGroup))
                .Build();

        /// <summary>
        ///     Loads a .qua file from a stream
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="checkValidity"></param>
        /// <returns></returns>
        public static Qua Parse(byte[] buffer, bool checkValidity = true)
        {
            using var input = new StringReader(Encoding.UTF8.GetString(buffer, 0, buffer.Length));

            var qua = (Qua)Deserializer.Deserialize(input, typeof(Qua));

            RestoreDefaultValues(qua);
            AfterLoad(qua, checkValidity);

            return qua;
        }

        /// <summary>
        ///     Takes in a path to a .qua file and attempts to parse it.
        ///     Will throw an error if unable to be parsed.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="checkValidity"></param>
        public static Qua Parse(string path, bool checkValidity = true)
        {
            Qua qua;

            using (var file = File.OpenText(path))
            {
                qua = (Qua)Deserializer.Deserialize(file, typeof(Qua));
                qua.FilePath = path;

                RestoreDefaultValues(qua);
            }

            AfterLoad(qua, checkValidity);

            return qua;
        }

        /// <summary>
        ///     Serializes the Qua object and returns a string of it
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            static HitObjectInfo SerializableHitObject(HitObjectInfo obj) =>
                new HitObjectInfo
                {
                    EditorLayer = obj.EditorLayer, EndTime = obj.EndTime,
                    HitSound = obj.HitSound == HitSounds.Normal ? 0 : obj.HitSound, KeySounds = obj
                       .KeySounds
                       .Select(x => new KeySoundInfo { Sample = x.Sample, Volume = x.Volume == 100 ? 0 : x.Volume })
                       .ToList(),
                    Lane = obj.Lane, StartTime = obj.StartTime,
                    TimingGroup = obj.TimingGroup == DefaultScrollGroupId ? null : obj.TimingGroup
                };

            static SoundEffectInfo SerializableSoundEffect(SoundEffectInfo x) =>
                x.Volume == 100
                    ? new SoundEffectInfo { StartTime = x.StartTime, Sample = x.Sample, Volume = 0 }
                    : x;

            static TimingPointInfo SerializableTimingPoint(TimingPointInfo x) =>
                x.Signature is TimeSignature.Quadruple
                    ? new TimingPointInfo { Bpm = x.Bpm, Signature = 0, StartTime = x.StartTime, Hidden = x.Hidden }
                    : x;

            // Sort the object before saving.
            Sort();

            // Set default values to zero so they don't waste space in the .qua file.
            var originalTimingPoints = TimingPoints;
            var originalHitObjects = HitObjects;
            var originalSoundEffects = SoundEffects;
            var originalBookmarks = Bookmarks;
            var originalTimingGroups = TimingGroups;
            var originalDefaultSsfs = ScrollSpeedFactors;

            TimingPoints = originalTimingPoints.Select(SerializableTimingPoint).ToList();
            HitObjects = originalHitObjects.Select(SerializableHitObject).ToList();
            SoundEffects = originalSoundEffects.Select(SerializableSoundEffect).ToList();
            TimingGroups = originalTimingGroups.ToDictionary(x => x.Key, x => x.Value);
            TimingGroups.Remove(DefaultScrollGroupId);

            // Remove empty global scroll group
            var globalScrollGroup = GlobalScrollGroup;
            if (globalScrollGroup.ScrollVelocities.Count == 0 && globalScrollGroup.ScrollSpeedFactors.Count == 0)
                TimingGroups.Remove(GlobalScrollGroupId);

            // Don't serialize the field at all if we dont have additional timing groups
            if (TimingGroups.Count == 0)
                TimingGroups = null;

            // Doing this to keep compatibility with older versions of .qua (.osu and .sm file conversions). It won't serialize
            // the bookmarks in the file.
            if (Bookmarks.Count == 0)
                Bookmarks = null;

            if (ScrollSpeedFactors.Count == 0)
                ScrollSpeedFactors = null;

            using var stringWriter = new StringWriter { NewLine = "\r\n" };
            Serializer.Serialize(stringWriter, this);
            var serialized = stringWriter.ToString();

            // Restore the original lists.
            TimingPoints = originalTimingPoints;
            HitObjects = originalHitObjects;
            SoundEffects = originalSoundEffects;
            Bookmarks = originalBookmarks;
            ScrollSpeedFactors = originalDefaultSsfs;

            TimingGroups = originalTimingGroups;
            TimingGroups[GlobalScrollGroupId] = globalScrollGroup;
            LinkDefaultScrollGroup();
            Debug.Assert(TimingGroups != null && TimingGroups.Count >= 2);

            return serialized;
        }

        /// <summary>
        ///     Serializes the Qua object and writes it to a file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path) => File.WriteAllText(path, Serialize());

        /// <summary>
        ///     If the .qua file is actually valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid() => Validate().Count == 0;

        /// <summary>
        ///     Returns all validation errors in the .qua file.
        /// </summary>
        /// <returns></returns>
        public List<string> Validate()
        {
            var errors = new List<string>();

            // If there aren't any HitObjects
            if (HitObjects.Count == 0)
                errors.Add("There are no HitObjects.");

            // If there aren't any TimingPoints
            if (TimingPoints.Count == 0)
                errors.Add("There are no TimingPoints.");

            // Check if the mode is actually valid
            if (!Enum.IsDefined(typeof(GameMode), Mode))
                errors.Add($"The GameMode '{Mode}' is invalid.");

            // Check that sound effects are valid.
            foreach (var info in SoundEffects)
            {
                // Sample should be a valid array index.
                if (info.Sample < 1 || info.Sample >= CustomAudioSamples.Count + 1)
                    errors.Add($"SoundEffect at {info.StartTime} has an invalid Sample index.");

                // The sample volume should be between 1 and 100.
                if (info.Volume < 1 || info.Volume > 100)
                    errors.Add($"SoundEffect at {info.StartTime} has an invalid Volume.");
            }

            // Check that hit objects are valid.
            foreach (var info in HitObjects)
            {
                // LN end times should be > start times.
                if (info.IsLongNote && info.EndTime <= info.StartTime)
                    errors.Add($"LongNote at {info.StartTime} has an invalid EndTime.");

                // Check that key sounds are valid.
                foreach (var keySound in info.KeySounds)
                {
                    // Sample should be a valid array index.
                    if (keySound.Sample < 1 || keySound.Sample >= CustomAudioSamples.Count + 1)
                        errors.Add($"KeySound at {info.StartTime} has an invalid Sample index.");

                    // The sample volume should be above 0.
                    if (keySound.Volume < 1)
                        errors.Add($"KeySound at {info.StartTime} has an invalid Volume.");
                }
            }

            return errors;
        }

        /// <summary>
        ///     Does some sorting of the Qua
        /// </summary>
        public void Sort()
        {
            LinkDefaultScrollGroup();

            SortBookmarks();
            SortHitObjects();
            SortSoundEffects();
            SortTimingPoints();
            SortSliderVelocities();
            SortScrollSpeedFactors();
        }

        /// <summary>
        ///     The average notes per second in the map.
        /// </summary>
        /// <returns></returns>
        public float AverageNotesPerSecond(float rate = 1.0f) => HitObjects.Count / (Length / (1000f * rate));

        /// <summary>
        ///     Calculates and returns the map's actions per second.
        ///
        ///     Actions per second is defined as:
        ///         - The amount of presses and long note releases the player performs a second
        ///         - Excludes break and intro times.
        ///
        ///     * Should be used instead of <see cref="AverageNotesPerSecond"/> for a more accurate
        ///     representation of density.
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public float GetActionsPerSecond(float rate = 1.0f)
        {
            var actions = new List<int>();

            foreach (var ho in HitObjects)
            {
                actions.Add(ho.StartTime);

                if (ho.IsLongNote)
                    actions.Add(ho.EndTime);
            }

            if (actions.Count == 0)
                return 0;

            actions.Sort();

            var length = actions.Last();

            // Remove empty intro time
            length -= actions.First();

            // Exclude break times from the total length
            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];

                if (i == 0)
                    continue;

                var previousAction = actions[i - 1];

                var difference = action - previousAction;

                if (difference >= 1000)
                    length -= difference;
            }

            return actions.Count / (length / (1000f * rate));
        }

        /// <summary>
        ///    In Quaver, the key count is defined by the game mode.
        ///    This translates mode to key count.
        /// </summary>
        /// <returns></returns>
        public int GetKeyCount(bool includeScratch = true) => ModeHelper.ToKeyCount(Mode, HasScratchKey && includeScratch);

        /// <summary>
        ///     Finds the most common BPM in a Qua object.
        /// </summary>
        /// <returns></returns>
        public float GetCommonBpm()
        {
            if (TimingPoints.Count is 0)
                return 0;

            // This fallback isn't really justified, but it's only used for tests.
            if (HitObjects.Count is 0)
                return TimingPoints[0].Bpm;

            var lastObject = HitObjects.OrderByDescending(x => x.IsLongNote ? x.EndTime : x.StartTime).First();
            double lastTime = lastObject.IsLongNote ? lastObject.EndTime : lastObject.StartTime;
            var durations = new Dictionary<float, int>();

            for (var i = TimingPoints.Count - 1; i >= 0; i--)
            {
                var point = TimingPoints[i];

                // Make sure that timing points past the last object don't break anything.
                if (point.StartTime > lastTime)
                    continue;

                var duration = (int)(lastTime - (i == 0 ? 0 : point.StartTime));
                lastTime = point.StartTime;

                if (!durations.TryAdd(point.Bpm, duration))
                    durations[point.Bpm] += duration;
            }

            // osu! hangs on loading the map in this case; we return a sensible result.
            return durations.Count is 0 ? TimingPoints[0].Bpm : durations.OrderByDescending(x => x.Value).First().Key;
        }

        /// <summary>
        ///     Gets the timing point at a particular time in the map.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public TimingPointInfo GetTimingPointAt(double time)
        {
            var index = TimingPoints.IndexAtTime((float)time);

            // If the point can't be found, we want to return either null if there aren't
            // any points, or the first timing point, since it'll be considered as a part of it anyway.
            return index == -1 ? TimingPoints.Count is 0 ? null : TimingPoints[0] : TimingPoints[index];
        }

        /// <summary>
        ///     Gets the bookmark at a particular time in the map.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public BookmarkInfo GetBookmarkAt(int time) => Bookmarks.AtTime(time);

        /// <summary>
        ///     Gets a scroll velocity at a particular time in the map
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timingGroupId"></param>
        /// <returns></returns>
        public SliderVelocityInfo GetScrollVelocityAt(double time, string timingGroupId = DefaultScrollGroupId) =>
            ((ScrollGroup)TimingGroups[timingGroupId]).GetScrollVelocityAt(time);

        /// <summary>
        ///     Gets a scroll velocity at a particular time in the map
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public ScrollSpeedFactorInfo GetScrollSpeedFactorAt(double time, string timingGroupId = DefaultScrollGroupId) =>
            ((ScrollGroup)TimingGroups[timingGroupId]).GetScrollSpeedFactorAt(time);

        /// <summary>
        ///    Finds the length of a timing point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double GetTimingPointLength(TimingPointInfo point)
        {
            // Find the index of the current timing point.
            var index = TimingPoints.IndexOf(point);

            // ??
            if (index == -1)
                throw new ArgumentException();

            // There is another timing point ahead of this one
            // so we'll need to get the length of the two points.
            if (index + 1 < TimingPoints.Count)
                return TimingPoints[index + 1].StartTime - TimingPoints[index].StartTime;

            // Only one timing point, so we can assume that it goes to the end of the map.
            return Length - point.StartTime;
        }

        /// <summary>
        ///     O(n)
        ///     Returns the list of hit objects that are in the specified group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<HitObjectInfo> GetTimingGroupObjects(string id) =>
            HitObjects.Where(hitObjectInfo => hitObjectInfo.TimingGroup == id);

        /// <summary>
        ///     O(n log m) Given a list of timing group IDs, return a dictionary of (ID, [HitObject])
        /// </summary>
        /// <param name="timingGroupIds"></param>
        /// <returns></returns>
        public Dictionary<string, List<HitObjectInfo>> GetTimingGroupObjects(HashSet<string> timingGroupIds)
        {
            var result = new Dictionary<string, List<HitObjectInfo>>();
            foreach (var hitObjectInfo in HitObjects.Where(hitObjectInfo =>
                         timingGroupIds.Contains(hitObjectInfo.TimingGroup)))
            {
                if (!result.TryGetValue(hitObjectInfo.TimingGroup, out var list))
                {
                    list = new List<HitObjectInfo>();
                    result.Add(hitObjectInfo.TimingGroup, list);
                }
                list.Add(hitObjectInfo);
            }
            return result;
        }

        /// <summary>
        ///     Solves the difficulty of the map and returns the data for it.
        /// </summary>
        /// <param name="mods"></param>
        /// <param name="applyMods"></param>
        /// <returns></returns>
        public DifficultyProcessor SolveDifficulty(ModIdentifier mods = ModIdentifier.None, bool applyMods = false)
        {
            var qua = this;

            // Create a new version of the qua with modifiers applied, and use that for calculations.
            // ReSharper disable once InvertIf
            if (applyMods)
            {
                qua = qua.DeepClone();
                qua.ApplyMods(mods);
            }

            if (ModeHelper.IsKeyMode(qua.Mode))
                return new DifficultyProcessorKeys(qua, new StrainConstantsKeys(), mods);
            else
                throw new InvalidEnumArgumentException();
        }

        /// <summary>
        ///     Computes the "SV-ness" of a map.
        ///
        ///     SliderVelocities, TimingPoints and HitObjects must be sorted by time before calling this function.
        /// </summary>
        /// <returns></returns>
        public double SVFactor()
        {
            // SVs below this are considered the same. "Basically stationary."
            const float MIN_MULTIPLIER = 1e-3f;
            // SVs above this are considered the same. "Basically teleport."
            const float MAX_MULTIPLIER = 1e2f;

            var qua = WithNormalizedSVs();

            // Create a list of important timestamps from the perspective of playing the map.
            var importantTimestamps = new List<float>();

            foreach (var hitObject in HitObjects)
            {
                importantTimestamps.Add(hitObject.StartTime);

                if (hitObject.IsLongNote)
                    importantTimestamps.Add(hitObject.EndTime);
            }

            importantTimestamps.Sort();
            var nextImportantTimestampIndex = 0;

            var sum = 0d;

            for (var i = 1; i < qua.SliderVelocities.Count; i++)
            {
                var prevSv = qua.SliderVelocities[i - 1];
                var sv = qua.SliderVelocities[i];

                // Find the first important timestamp after the SV.
                while (nextImportantTimestampIndex < importantTimestamps.Count &&
                    importantTimestamps[nextImportantTimestampIndex] < sv.StartTime)
                    nextImportantTimestampIndex++;

                // Don't count the SV if there's nothing important within 1 second after it.
                // This is to prevent line art from contributing to the SV-ness.
                if (nextImportantTimestampIndex >= importantTimestamps.Count ||
                    importantTimestamps[nextImportantTimestampIndex] > sv.StartTime + 1000)
                    continue;

                var prevMultiplier = Math.Min(Math.Max(Math.Abs(prevSv.Multiplier), MIN_MULTIPLIER), MAX_MULTIPLIER);
                var multiplier = Math.Min(Math.Max(Math.Abs(sv.Multiplier), MIN_MULTIPLIER), MAX_MULTIPLIER);

                // The difference between SV multipliers is computed under a log, because it matters that the SV multiplier
                // changed, for example, ten-fold (from 0.1× to 1× or from 1× to 10×), and not that it changed _by_ some value (e.g. by 0.9 or by 9).
                var prevLogMultiplier = Math.Log(prevMultiplier);
                var logMultiplier = Math.Log(multiplier);

                var difference = Math.Abs(logMultiplier - prevLogMultiplier);
                sum += difference;
            }

            return sum;
        }

        public override string ToString() => $"{Artist} - {Title} [{DifficultyName}]";

        /// <summary>
        ///     Replaces long notes with regular notes starting at the same time.
        /// </summary>
        public void ReplaceLongNotesWithRegularNotes()
        {
            foreach (var temp in HitObjects)
                temp.EndTime = 0;
        }

        /// <summary>
        ///     Replaces regular notes with long notes and vice versa.
        ///
        ///     HitObjects and TimingPoints MUST be sorted by StartTime prior to calling this method,
        ///     see <see cref="Sort()"/>.
        /// </summary>
        public void ApplyInverse()
        {
            // Minimal LN and gap lengths in milliseconds.
            //
            // Ideally this should be computed in a smart way using the judgements so that it is always possible to get
            // perfects, but making map mods depend on the judgements (affected by strict/chill/accuracy adjustments) is
            // a terrible idea. I'm setting these to values that will probably work fine for the majority of the
            // cases.
            const int MINIMAL_LN_LENGTH = 36;
            const int MINIMAL_GAP_LENGTH = 36;

            var newHitObjects = new List<HitObjectInfo>();

            var keyCount = GetKeyCount();

            // An array indicating whether the currently processed HitObject is the first in its lane.
            var firstInLane = new bool[keyCount];

            for (var i = 0; i < firstInLane.Length; i++)
                firstInLane[i] = true;

            for (var i = 0; i < HitObjects.Count; i++)
            {
                var currentObject = HitObjects[i];

                // The scratch lane (the last one in Quaver) should not be affected by Inverse.
                if (HasScratchKey && currentObject.Lane == keyCount)
                {
                    newHitObjects.Add(currentObject);
                    continue;
                }

                // Find the next and second next hit object in the lane.
                HitObjectInfo nextObjectInLane = null, secondNextObjectInLane = null;

                for (var j = i + 1; j < HitObjects.Count; j++)
                    if (HitObjects[j].Lane == currentObject.Lane)
                    {
                        if (nextObjectInLane == null)
                            nextObjectInLane = HitObjects[j];
                        else
                        {
                            secondNextObjectInLane = HitObjects[j];
                            break;
                        }
                    }

                var isFirstInLane = firstInLane[currentObject.Lane - 1];
                firstInLane[currentObject.Lane - 1] = false;

                // If this is the only object in its lane, keep it as is.
                if (nextObjectInLane == null && isFirstInLane)
                {
                    newHitObjects.Add(currentObject);
                    continue;
                }

                // Figure out the time gap between the end of the LN which we'll create and the next object.
                int? timeGap = null;

                if (nextObjectInLane != null)
                {
                    var timingPoint = GetTimingPointAt(nextObjectInLane.StartTime);
                    float bpm;

                    // If the timing point starts at the next object, we want to use the previous timing point's BPM.
                    // For example, consider a fast section of the map transitioning into a very low BPM ending starting
                    // with the next hit object. Since the LN release and the gap are still in the fast section, they
                    // should use the fast section's BPM.
                    if ((int)Math.Round(timingPoint.StartTime) == nextObjectInLane.StartTime)
                    {
                        var prevTimingPointIndex = TimingPoints.IndexAtTimeBefore(timingPoint.StartTime);

                        // No timing points before the object? Just use the first timing point then, it has the correct
                        // BPM.
                        if (prevTimingPointIndex == -1)
                            prevTimingPointIndex = 0;

                        bpm = TimingPoints[prevTimingPointIndex].Bpm;
                    }
                    else
                        bpm = timingPoint.Bpm;

                    // The time gap is quarter of the milliseconds per beat.
                    timeGap = (int?)Math.Max(Math.Round(15000 / bpm), MINIMAL_GAP_LENGTH);
                }

                // Summary of the changes:
                // Regular 1 -> Regular 2 => LN until 2 - time gap
                // Regular 1 -> LN 2      => LN until 2
                //      LN 1 -> Regular 2 => LN from 1 end until 2 - time gap
                //      LN 1 -> LN 2      => LN from 1 end until 2
                //
                // Exceptions:
                // - last LNs are kept (treated as regular 2)
                // - last regular objects are removed and treated as LN 2

                if (currentObject.IsLongNote)
                {
                    // LNs before regular objects are changed so they start where they ended and end a time gap before
                    // the object.
                    // LNs before LNs do the same but without a time gap.

                    if (nextObjectInLane == null)
                    {
                        // If this is the last object in its lane, though, then it's probably a better idea
                        // to leave it be. For example, finishing long LNs in charts.
                    }
                    else
                    {
                        currentObject.StartTime = currentObject.EndTime; // (this part can mess up the ordering)
                        currentObject.EndTime = nextObjectInLane.StartTime - timeGap.Value;

                        // Clear the keysounds as we're moving the start, so they won't make sense.
                        currentObject.KeySounds = new List<KeySoundInfo>();

                        // If the next object is not an LN, and it's the last object in the lane, or if it's an LN and
                        // not the last object in the lane, create a regular object at the next object's start position.
                        if ((secondNextObjectInLane == null) != nextObjectInLane.IsLongNote)
                            currentObject.EndTime = nextObjectInLane.StartTime;

                        // Filter out really short LNs or even negative length resulting from jacks or weird BPM values.
                        if (currentObject.EndTime - currentObject.StartTime < MINIMAL_LN_LENGTH)
                            // These get skipped entirely.
                            //
                            // Actually, there can be a degenerate pattern of multiple LNs with really short gaps
                            // in between them (less than MINIMAL_LN_LENGTH), which this logic will convert
                            // into nothing. That should be pretty rare though.
                            continue;
                    }
                }
                else
                {
                    // Regular objects are replaced with LNs starting from their start and ending quarter of a beat
                    // before the next object's start.
                    if (nextObjectInLane == null)
                        // If this is the last object in lane, though, then it's not included, and instead the previous
                        // LN spans up to this object's StartTime.
                        continue;

                    currentObject.EndTime = nextObjectInLane.StartTime - timeGap.Value;

                    // If the next object is not an LN, and it's the last object in the lane, or if it's an LN and
                    // not the last object in the lane, this LN should span until its start.
                    if ((secondNextObjectInLane == null) == (nextObjectInLane.EndTime == 0))
                        currentObject.EndTime = nextObjectInLane.StartTime;

                    // Filter out really short LNs or even negative length resulting from jacks or weird BPM values.
                    if (currentObject.EndTime - currentObject.StartTime < MINIMAL_LN_LENGTH)
                        // These get converted back into regular objects.
                        currentObject.EndTime = 0;
                }

                newHitObjects.Add(currentObject);
            }

            // LN conversion can mess up the ordering, so sort it again. See the (this part can mess up the ordering)
            // comment above.
            newHitObjects.HybridSort();
            HitObjects = newHitObjects;
        }

        /// <summary>
        ///     Applies mods to the map.
        /// </summary>
        /// <param name="mods">a list of mods to apply</param>
        public void ApplyMods(ModIdentifier mods)
        {
            if (mods.HasFlag(ModIdentifier.NoLongNotes))
                ReplaceLongNotesWithRegularNotes();

            if (mods.HasFlag(ModIdentifier.Inverse))
                ApplyInverse();

            // FullLN is NLN followed by Inverse.
            if (mods.HasFlag(ModIdentifier.FullLN))
            {
                ReplaceLongNotesWithRegularNotes();
                ApplyInverse();
            }

            if (mods.HasFlag(ModIdentifier.Mirror))
                MirrorHitObjects();
        }

        /// <summary>
        ///     Used by the Randomize modifier to shuffle around the lanes.
        ///     Replaces long notes with regular notes starting at the same time.
        /// </summary>
        public void RandomizeLanes(int seed)
        {
            // if seed is default, then abort.
            if (seed == -1)
                return;

            RandomizeModifierSeed = seed;

            var values = new List<int>();
            values.AddRange(Enumerable.Range(0, GetKeyCount(false)).Select(x => x + 1));
            values.Shuffle(new Random(seed));

            if (HasScratchKey)
                values.Add(GetKeyCount());

            foreach (var temp in HitObjects)
                temp.Lane = values[temp.Lane - 1];
        }

        /// <summary>
        ///     Flips the lanes of the HitObjects
        /// </summary>
        public void MirrorHitObjects()
        {
            var keyCount = GetKeyCount();

            if (HasScratchKey) // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var temp in HitObjects)
                {
                    // The scratch lane (which is the last lane in Quaver) should not be mirrored.
                    if (temp.Lane != keyCount)
                        temp.Lane = keyCount - temp.Lane;
                }
            else
                foreach (var temp in HitObjects)
                    temp.Lane = keyCount - temp.Lane + 1;
        }

        /// <summary>
        /// </summary>
        public void SortBookmarks() => Bookmarks.HybridSort();

        /// <summary>
        /// </summary>
        public void SortScrollSpeedFactors()
        {
            foreach (var (_, timingGroup) in TimingGroups)
            {
                if (!(timingGroup is ScrollGroup scrollGroup))
                    continue;
                scrollGroup.ScrollSpeedFactors.HybridSort();
            }
        }

        /// <summary>
        /// </summary>
        public void SortHitObjects() => HitObjects.HybridSort();

        /// <summary>
        /// </summary>
        public void SortSliderVelocities()
        {
            foreach (var (_, timingGroup) in TimingGroups)
            {
                if (!(timingGroup is ScrollGroup scrollGroup))
                    continue;
                scrollGroup.ScrollVelocities.HybridSort();
            }
        }

        /// <summary>
        /// </summary>
        public void SortSoundEffects() => SoundEffects.HybridSort();

        /// <summary>
        /// </summary>
        public void SortTimingPoints() => TimingPoints.HybridSort();

        /// <summary>
        ///     Gets the judgement of a particular hitobject in the map
        /// </summary>
        /// <param name="ho"></param>
        /// <returns></returns>
        public int GetHitObjectJudgementIndex(HitObjectInfo ho)
        {
            var total = 0;

            foreach (var h in HitObjects)
            {
                if (h == ho)
                    return total;

                total += h.IsLongNote ? 2 : 1;
            }

            return -1;
        }

        /// <summary>
        ///     Gets a hitobject at a particular judgement index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public HitObjectInfo GetHitObjectAtJudgementIndex(int index)
        {
            var total = 0;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var h in HitObjects)
                if (total++ == index || (h.IsLongNote && total++ == index))
                    return h;

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="qua"></param>
        public static void RestoreDefaultValues(Qua qua)
        {
            qua.LinkDefaultScrollGroup();

            qua.TimingGroups.TryAdd(GlobalScrollGroupId, new ScrollGroup());

            // Restore default values.
            for (var i = 0; i < qua.TimingPoints.Count; i++)
            {
                var tp = qua.TimingPoints[i];

                if (tp.Signature == 0)
                    tp.Signature = TimeSignature.Quadruple;

                qua.TimingPoints[i] = tp;
            }

            for (var i = 0; i < qua.HitObjects.Count; i++)
            {
                var obj = qua.HitObjects[i];

                if (obj.HitSound == 0)
                    obj.HitSound = HitSounds.Normal;

                obj.TimingGroup ??= DefaultScrollGroupId;

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var keySound in obj.KeySounds)
                    if (keySound.Volume == 0)
                        keySound.Volume = 100;

                qua.HitObjects[i] = obj;
            }

            for (var i = 0; i < qua.SoundEffects.Count; i++)
            {
                var info = qua.SoundEffects[i];

                if (info.Volume == 0)
                    info.Volume = 100;

                qua.SoundEffects[i] = info;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="qua"></param>
        /// <param name="checkValidity"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void AfterLoad(Qua qua, bool checkValidity)
        {
            if (checkValidity && qua.Validate() is var errors && errors.Count > 0)
                throw new ArgumentException(string.Join("\n", errors));

            // Try to sort the Qua before returning.
            qua.Sort();
        }

        /// <summary>
        ///     Converts SVs to the normalized format (BPM does not affect SV).
        ///
        ///     Must be done after sorting TimingPoints and SliderVelocities.
        /// </summary>
        public void NormalizeSVs()
        {
            if (BPMDoesNotAffectScrollVelocity)
                // Already normalized.
                return;

            var baseBpm = GetCommonBpm();

            var normalizedScrollVelocities = new List<SliderVelocityInfo>();

            var currentBpm = TimingPoints[0].Bpm;
            var currentSvIndex = 0;
            float? currentSvStartTime = null;
            var currentSvMultiplier = 1f;
            float? currentAdjustedSvMultiplier = null;
            float? initialSvMultiplier = null;

            for (var i = 0; i < TimingPoints.Count; i++)
            {
                var timingPoint = TimingPoints[i];

                var nextTimingPointHasSameTimestamp = i + 1 < TimingPoints.Count &&
                    TimingPoints[i + 1].StartTime == timingPoint.StartTime;

                while (true)
                {
                    if (currentSvIndex >= SliderVelocities.Count)
                        break;

                    var sv = SliderVelocities[currentSvIndex];

                    if (sv.StartTime > timingPoint.StartTime)
                        break;

                    // If there are more timing points on this timestamp, the SV only applies on the
                    // very last one, so skip it for now.
                    if (nextTimingPointHasSameTimestamp && sv.StartTime == timingPoint.StartTime)
                        break;

                    if (sv.StartTime < timingPoint.StartTime)
                    {
                        // The way that osu! handles infinite BPM is more akin to "arbitrarily large SV".
                        // We chose the smallest power of two greater than `MAX_MULTIPLIER`
                        // from `SVFactor` to make `DenormalizeSVs` more accurate.
                        var multiplier = float.IsInfinity(currentBpm) ? 128 : sv.Multiplier * (currentBpm / baseBpm);

                        if (currentAdjustedSvMultiplier == null)
                        {
                            currentAdjustedSvMultiplier = multiplier;
                            initialSvMultiplier = multiplier;
                        }

                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (multiplier != currentAdjustedSvMultiplier.Value)
                        {
                            normalizedScrollVelocities.Add(
                                new SliderVelocityInfo { StartTime = sv.StartTime, Multiplier = multiplier }
                            );

                            currentAdjustedSvMultiplier = multiplier;
                        }
                    }

                    currentSvStartTime = sv.StartTime;
                    currentSvMultiplier = sv.Multiplier;
                    currentSvIndex += 1;
                }

                // Timing points reset the previous SV multiplier.
                if (currentSvStartTime == null || currentSvStartTime.Value < timingPoint.StartTime)
                    currentSvMultiplier = 1;

                currentBpm = timingPoint.Bpm;

                var multiplierToo = float.IsInfinity(currentBpm) ? 128 : currentSvMultiplier * (currentBpm / baseBpm);

                if (currentAdjustedSvMultiplier == null)
                {
                    currentAdjustedSvMultiplier = multiplierToo;
                    initialSvMultiplier = multiplierToo;
                }

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (multiplierToo == currentAdjustedSvMultiplier.Value)
                    continue;

                normalizedScrollVelocities.Add(
                    new SliderVelocityInfo { StartTime = timingPoint.StartTime, Multiplier = multiplierToo }
                );

                currentAdjustedSvMultiplier = multiplierToo;
            }

            for (; currentSvIndex < SliderVelocities.Count; currentSvIndex++)
            {
                var sv = SliderVelocities[currentSvIndex];
                var multiplier = float.IsInfinity(currentBpm) ? 128 : sv.Multiplier * (currentBpm / baseBpm);

                Debug.Assert(currentAdjustedSvMultiplier != null, nameof(currentAdjustedSvMultiplier) + " != null");

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (multiplier == currentAdjustedSvMultiplier.Value)
                    continue;

                normalizedScrollVelocities.Add(
                    new SliderVelocityInfo { StartTime = sv.StartTime, Multiplier = multiplier }
                );

                currentAdjustedSvMultiplier = multiplier;
            }

            BPMDoesNotAffectScrollVelocity = true;
            normalizedScrollVelocities.HybridSort();
            SliderVelocities = normalizedScrollVelocities;
            InitialScrollVelocity = initialSvMultiplier ?? 1;
        }

        /// <summary>
        ///     Converts SVs to the denormalized format (BPM affects SV).
        ///
        ///     Must be done after sorting TimingPoints and SliderVelocities.
        /// </summary>
        public void DenormalizeSVs()
        {
            if (!BPMDoesNotAffectScrollVelocity)
                // Already denormalized.
                return;

            var baseBpm = GetCommonBpm();

            var denormalizedScrollVelocities = new List<SliderVelocityInfo>();
            var currentBpm = TimingPoints[0].Bpm;

            // For the purposes of this conversion, 0 and +inf should be handled like max value.
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (currentBpm == 0 || float.IsPositiveInfinity(currentBpm))
                currentBpm = float.MaxValue;

            var currentSvIndex = 0;
            var currentSvMultiplier = InitialScrollVelocity;
            float? currentAdjustedSvMultiplier = null;

            for (var i = 0; i < TimingPoints.Count; i++)
            {
                var timingPoint = TimingPoints[i];

                while (true)
                {
                    if (currentSvIndex >= SliderVelocities.Count)
                        break;

                    var sv = SliderVelocities[currentSvIndex];

                    if (sv.StartTime > timingPoint.StartTime)
                        break;

                    if (sv.StartTime < timingPoint.StartTime)
                    {
                        // The way that osu! handles infinite BPM is more akin to "arbitrarily large SV".
                        // We chose the greatest power of two less than `MIN_MULTIPLIER`
                        // from `SVFactor` to make `NormalizeSVs` more accurate.
                        var multiplier = float.IsInfinity(currentBpm) ? 1 / 128f : sv.Multiplier / (currentBpm / baseBpm);

                        // ReSharper disable once CompareOfFloatsByEqualityOperator
                        if (currentAdjustedSvMultiplier == null || multiplier != currentAdjustedSvMultiplier)
                        {
                            // ReSharper disable once CompareOfFloatsByEqualityOperator
                            if (currentAdjustedSvMultiplier == null && sv.Multiplier != InitialScrollVelocity)
                                // Insert an SV 1 ms earlier to simulate the initial scroll speed multiplier.
                                denormalizedScrollVelocities.Add(
                                    new SliderVelocityInfo
                                    {
                                        StartTime = sv.StartTime - 1,
                                        Multiplier = float.IsInfinity(currentBpm) ? 1 / 128f : InitialScrollVelocity / (currentBpm / baseBpm),
                                    }
                                );

                            denormalizedScrollVelocities.Add(
                                new SliderVelocityInfo { StartTime = sv.StartTime, Multiplier = multiplier }
                            );

                            currentAdjustedSvMultiplier = multiplier;
                        }
                    }

                    currentSvMultiplier = sv.Multiplier;
                    currentSvIndex += 1;
                }

                currentBpm = timingPoint.Bpm;

                // For the purposes of this conversion, 0 and +inf should be handled like max value.
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (currentBpm == 0 || float.IsPositiveInfinity(currentBpm))
                    currentBpm = float.MaxValue;

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (currentAdjustedSvMultiplier == null && currentSvMultiplier != InitialScrollVelocity)
                    // Insert an SV 1 ms earlier to simulate the initial scroll speed multiplier.
                    denormalizedScrollVelocities.Add(
                        new SliderVelocityInfo
                        {
                            StartTime = timingPoint.StartTime - 1,
                            Multiplier = float.IsInfinity(currentBpm) ? 1 / 128f : InitialScrollVelocity / (currentBpm / baseBpm),
                        }
                    );

                // Timing points reset the SV multiplier.
                currentAdjustedSvMultiplier = 1;

                // Skip over multiple timing points at the same timestamp.
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (i + 1 < TimingPoints.Count && TimingPoints[i + 1].StartTime == timingPoint.StartTime)
                    continue;

                // C# is stupid.
                var multiplierToo = float.IsInfinity(currentBpm) ? 1 / 128f : currentSvMultiplier / (currentBpm / baseBpm);

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (multiplierToo == currentAdjustedSvMultiplier.Value)
                    continue;

                denormalizedScrollVelocities.Add(
                    new SliderVelocityInfo { StartTime = timingPoint.StartTime, Multiplier = multiplierToo }
                );

                currentAdjustedSvMultiplier = multiplierToo;
            }

            for (; currentSvIndex < SliderVelocities.Count; currentSvIndex++)
            {
                var sv = SliderVelocities[currentSvIndex];
                var multiplier = float.IsInfinity(currentBpm) ? 1 / 128f : sv.Multiplier / (currentBpm / baseBpm);

                Debug.Assert(currentAdjustedSvMultiplier != null, nameof(currentAdjustedSvMultiplier) + " != null");

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (multiplier == currentAdjustedSvMultiplier.Value)
                    continue;

                denormalizedScrollVelocities.Add(
                    new SliderVelocityInfo { StartTime = sv.StartTime, Multiplier = multiplier }
                );

                currentAdjustedSvMultiplier = multiplier;
            }

            InitialScrollVelocity = 0;
            BPMDoesNotAffectScrollVelocity = false;
            denormalizedScrollVelocities.HybridSort();
            SliderVelocities = denormalizedScrollVelocities;
        }

        /// <summary>
        ///     Returns a Qua with normalized SVs.
        /// </summary>
        /// <returns></returns>
        public Qua WithNormalizedSVs()
        {
            var qua = this.DeepClone();
            qua.NormalizeSVs();
            return qua;
        }

        /// <summary>
        ///     Returns a Qua with denormalized SVs.
        /// </summary>
        /// <returns></returns>
        public Qua WithDenormalizedSVs()
        {
            var qua = this.DeepClone();
            qua.DenormalizeSVs();
            return qua;
        }

        /// <summary>
        ///     Returns the path of the file background. If no background exists, it will return null.
        /// </summary>
        /// <returns></returns>
        public string GetBackgroundPath() => GetFullPath(BackgroundFile);

        /// <summary>
        ///     Returns the path of the banner background. If no background exists, it will return null.
        /// </summary>
        /// <returns></returns>
        public string GetBannerPath() => GetFullPath(BannerFile);

        private string GetFullPath(string file) =>
            string.IsNullOrEmpty(file) || string.IsNullOrEmpty(FilePath)
                ? null
                : Path.Join(Path.GetDirectoryName(FilePath.AsSpan()), file);

        /// <summary>
        ///     Returns the path of the audio track file. If no track exists, it will return null.
        /// </summary>
        /// <returns></returns>
        public string GetAudioPath() => GetFullPath(AudioFile);

        private int MaxObjectTime()
        {
            Debug.Assert(HitObjects.Count != 0, "HitObjects.Count != 0");
            var max = HitObjects[^1].StartTime;
            var span = ListHelper.GetUnderlyingArray(HitObjects).AsSpan(0, HitObjects.Count);

            // Incredibly niche micro-optimization: CPUs are able to perform better branch prediction when this is done
            // backwards because matches are likely to only ever occur at the end of the span, which means that the CPU
            // will default to predicting false after the first few loops, which is almost always correct. Theoretically
            // this could be even faster if we implement SIMD, but since this requires far more rewriting than just a
            // for-loop, I will leave it as is until we specifically require this function to be as fast as possible.
            for (var i = span.Length - 1; i >= 0; i--)
                if (span[i].EndTime is var end && end > max)
                    max = end;

            return max;
        }
    }
}