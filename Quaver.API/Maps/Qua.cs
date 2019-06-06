/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2019 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MonoGame.Extended.Collections;
using Quaver.API.Enums;
using Quaver.API.Maps.Parsers;
using Quaver.API.Maps.Processors.Difficulty;
using Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys;
using Quaver.API.Maps.Processors.Scoring;
using Quaver.API.Maps.Structures;
using YamlDotNet.Serialization;

namespace Quaver.API.Maps
{
    [Serializable]
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
        ///     EditorLayer .qua data
        /// </summary>
        public List<EditorLayerInfo> EditorLayers { get; private set; } = new List<EditorLayerInfo>();

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
        /// </summary>
        public List<SliderVelocityInfo> SliderVelocities { get; private set; } = new List<SliderVelocityInfo>();

        /// <summary>
        ///     HitObject .qua data
        /// </summary>
        public List<HitObjectInfo> HitObjects { get; private set; } = new List<HitObjectInfo>();

        /// <summary>
        ///     Finds the length of the map
        /// </summary>
        /// <returns></returns>
        [YamlIgnore]
        public int Length => HitObjects.Count == 0 ? 0 : HitObjects.Max(x => Math.Max(x.StartTime, x.EndTime));

        /// <summary>
        ///     Integer based seed used for shuffling the lanes when randomize mod is active.
        ///     Defaults to -1 if there is no seed.
        /// </summary>
        [YamlIgnore]
        public int RandomizeModifierSeed { get; set; } = -1;

        /// <summary>
        ///     Ctor
        /// </summary>
        public Qua() {}

        /// <summary>
        ///     Returns true if the two maps are equal by value.
        /// </summary>
        /// <param name="other">the Qua to compare to</param>
        /// <returns></returns>
        public bool EqualByValue(Qua other)
        {
            return AudioFile == other.AudioFile
                   && SongPreviewTime == other.SongPreviewTime
                   && BackgroundFile == other.BackgroundFile
                   && MapId == other.MapId
                   && MapSetId == other.MapSetId
                   && Mode == other.Mode
                   && Title == other.Title
                   && Artist == other.Artist
                   && Source == other.Source
                   && Tags == other.Tags
                   && Creator == other.Creator
                   && DifficultyName == other.DifficultyName
                   && Description == other.Description
                   && TimingPoints.SequenceEqual(other.TimingPoints, TimingPointInfo.ByValueComparer)
                   && SliderVelocities.SequenceEqual(other.SliderVelocities, SliderVelocityInfo.ByValueComparer)
                   && HitObjects.SequenceEqual(other.HitObjects, HitObjectInfo.ByValueComparer)
                   && CustomAudioSamples.SequenceEqual(other.CustomAudioSamples, CustomAudioSampleInfo.ByValueComparer)
                   && SoundEffects.SequenceEqual(other.SoundEffects, SoundEffectInfo.ByValueComparer)
                   && EditorLayers.SequenceEqual(other.EditorLayers, EditorLayerInfo.ByValueComparer)
                   && RandomizeModifierSeed == other.RandomizeModifierSeed;
        }

        /// <summary>
        ///     Loads a .qua file from a stream
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="checkValidity"></param>
        /// <returns></returns>
        public static Qua Parse(byte[] buffer, bool checkValidity = true)
        {
            var input = new StringReader(Encoding.UTF8.GetString(buffer, 0, buffer.Length));

            var deserializer = new DeserializerBuilder();
            deserializer.IgnoreUnmatchedProperties();
            var qua = (Qua)deserializer.Build().Deserialize(input, typeof(Qua));

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
                var deserializer = new DeserializerBuilder();
                deserializer.IgnoreUnmatchedProperties();
                qua = (Qua)deserializer.Build().Deserialize(file, typeof(Qua));

                RestoreDefaultValues(qua);
            }

            AfterLoad(qua, checkValidity);

            return qua;
        }

        /// <summary>
        ///     Serializes the Qua object to a file.
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            // Sort the object before saving.
            Sort();

            // Set default values to zero so they don't waste space in the .qua file.
            var originalTimingPoints = TimingPoints;
            var originalHitObjects = HitObjects;
            var originalSoundEffects = SoundEffects;

            TimingPoints = new List<TimingPointInfo>();
            foreach (var tp in originalTimingPoints)
            {
                if (tp.Signature == TimeSignature.Quadruple)
                {
                    TimingPoints.Add(new TimingPointInfo()
                    {
                        Bpm = tp.Bpm,
                        Signature = 0,
                        StartTime = tp.StartTime
                    });
                }
                else
                {
                    TimingPoints.Add(tp);
                }
            }

            HitObjects = new List<HitObjectInfo>();
            foreach (var obj in originalHitObjects)
            {
                var keySoundsWithDefaults = new List<KeySoundInfo>();
                foreach (var keySound in obj.KeySounds)
                {
                    keySoundsWithDefaults.Add(new KeySoundInfo
                    {
                        Sample = keySound.Sample,
                        Volume = keySound.Volume == 100 ? 0 : keySound.Volume
                    });
                }

                HitObjects.Add(new HitObjectInfo()
                {
                    EndTime = obj.EndTime,
                    HitSound = obj.HitSound == HitSounds.Normal ? 0 : obj.HitSound,
                    KeySounds = keySoundsWithDefaults,
                    Lane = obj.Lane,
                    StartTime = obj.StartTime,
                    EditorLayer = obj.EditorLayer
                });
            }

            SoundEffects = new List<SoundEffectInfo>();
            foreach (var info in originalSoundEffects)
            {
                if (info.Volume == 100)
                {
                    SoundEffects.Add(new SoundEffectInfo()
                    {
                        StartTime = info.StartTime,
                        Sample = info.Sample,
                        Volume = 0
                    });
                }
                else
                {
                    SoundEffects.Add(info);
                }
            }

            // Save.
            using (var file = File.CreateText(path))
            {
                var serializer = new Serializer();
                serializer.Serialize(file, this);
            }

            // Restore the original lists.
            TimingPoints = originalTimingPoints;
            HitObjects = originalHitObjects;
            SoundEffects = originalSoundEffects;
        }

        /// <summary>
        ///     If the .qua file is actually valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            // If there aren't any HitObjects
            if (HitObjects.Count == 0)
                return false;

            // If there aren't any TimingPoints
            if (TimingPoints.Count == 0)
                return false;

            // Check if the mode is actually valid
            if (!Enum.IsDefined(typeof(GameMode), Mode))
                return false;

            // Check that sound effects are valid.
            foreach (var info in SoundEffects)
            {
                // Sample should be a valid array index.
                if (info.Sample < 1 || info.Sample >= CustomAudioSamples.Count + 1)
                    return false;

                // The sample volume should be between 1 and 100.
                if (info.Volume < 1 || info.Volume > 100)
                    return false;
            }

            // Check that hit objects are valid.
            foreach (var info in HitObjects)
            {
                // LN end times should be > start times.
                if (info.IsLongNote && info.EndTime <= info.StartTime)
                    return false;

                // Check that key sounds are valid.
                foreach (var keySound in info.KeySounds)
                {
                    // Sample should be a valid array index.
                    if (keySound.Sample < 1 || keySound.Sample >= CustomAudioSamples.Count + 1)
                        return false;

                    // The sample volume should be above 0.
                    if (keySound.Volume < 1)
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Does some sorting of the Qua
        /// </summary>
        public void Sort()
        {
            HitObjects = HitObjects.OrderBy(x => x.StartTime).ToList();
            TimingPoints = TimingPoints.OrderBy(x => x.StartTime).ToList();
            SliderVelocities = SliderVelocities.OrderBy(x => x.StartTime).ToList();
            SoundEffects = SoundEffects.OrderBy(x => x.StartTime).ToList();
        }

        /// <summary>
        ///     The average notes per second in the map.
        /// </summary>
        /// <returns></returns>
        public float AverageNotesPerSecond(float rate = 1.0f) => HitObjects.Count / (Length / (1000f * rate));

        /// <summary>
        ///    In Quaver, the key count is defined by the game mode.
        ///    This translates mode to key count.
        /// </summary>
        /// <returns></returns>
        public int GetKeyCount()
        {
            switch (Mode)
            {
                case GameMode.Keys4:
                    return 4;
                case GameMode.Keys7:
                    return 7;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        /// <summary>
        ///     Finds the most common BPM in a Qua object.
        /// </summary>
        /// <returns></returns>
        public float GetCommonBpm()
        {
            if (TimingPoints.Count == 0)
                return 0;

            var lastObject = HitObjects.OrderByDescending(x => x.IsLongNote ? x.EndTime : x.StartTime).First();
            double lastTime = lastObject.IsLongNote ? lastObject.EndTime : lastObject.StartTime;

            var durations = new Dictionary<float, int>();
            for (var i = TimingPoints.Count - 1; i >= 0; i--)
            {
                var point = TimingPoints[i];

                // Make sure that timing points past the last object don't break anything.
                if (point.StartTime > lastTime)
                    continue;

                var duration = (int) (lastTime - (i == 0 ? 0 : point.StartTime));
                lastTime = point.StartTime;

                if (durations.ContainsKey(point.Bpm))
                    durations[point.Bpm] += duration;
                else
                    durations[point.Bpm] = duration;
            }

            if (durations.Count == 0)
                return TimingPoints[0].Bpm; // osu! hangs on loading the map in this case; we return a sensible result.

            return durations.OrderByDescending(x => x.Value).First().Key;
        }

        /// <summary>
        ///     Gets the timing point at a particular time in the map.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public TimingPointInfo GetTimingPointAt(double time)
        {
            var index = TimingPoints.FindLastIndex(x => x.StartTime <= time);

            // If the point can't be found, we want to return either null if there aren't
            // any points, or the first timing point, since it'll be considered as apart of it anyway.
            if (index == -1)
                return TimingPoints.Count == 0 ? null : TimingPoints.First();

            return TimingPoints[index];
        }

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
        ///     Solves the difficulty of the map and returns the data for it.
        /// </summary>
        /// <param name="mods"></param>
        /// <returns></returns>
        public DifficultyProcessor SolveDifficulty(ModIdentifier mods = ModIdentifier.None)
        {
            switch (Mode)
            {
                case GameMode.Keys4:
                    return new DifficultyProcessorKeys(this, new StrainConstantsKeys(), mods);
                case GameMode.Keys7:
                    return new DifficultyProcessorKeys(this, new StrainConstantsKeys(), mods);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public override string ToString() => $"{Artist} - {Title} [{DifficultyName}]";

        /// <summary>
        ///     Replaces long notes with regular notes starting at the same time.
        /// </summary>
        public void ReplaceLongNotesWithRegularNotes()
        {
            for (var i = 0; i < HitObjects.Count; i++)
            {
                var temp = HitObjects[i];
                temp.EndTime = 0;
                HitObjects[i] = temp;
            }
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
            // a really bad idea. I'm setting these to values that will probably work fine for the majority of the
            // cases.
            const int MINIMAL_LN_LENGTH = 36;
            const int MINIMAL_GAP_LENGTH = 36;

            var newHitObjects = new List<HitObjectInfo>();

            // An array indicating whether the currently processed HitObject is the first in its lane.
            var firstInLane = new bool[GetKeyCount()];
            for (var i = 0; i < firstInLane.Length; i++)
                firstInLane[i] = true;

            for (var i = 0; i < HitObjects.Count; i++)
            {
                var currentObject = HitObjects[i];

                // Find the next and second next hit object in the lane.
                HitObjectInfo nextObjectInLane = null, secondNextObjectInLane = null;
                for (var j = i + 1; j < HitObjects.Count; j++)
                {
                    if (HitObjects[j].Lane == currentObject.Lane)
                    {
                        if (nextObjectInLane == null)
                        {
                            nextObjectInLane = HitObjects[j];
                        }
                        else
                        {
                            secondNextObjectInLane = HitObjects[j];
                            break;
                        }
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
                    if ((int) Math.Round(timingPoint.StartTime) == nextObjectInLane.StartTime)
                    {
                        var prevTimingPointIndex = TimingPoints.FindLastIndex(x => x.StartTime < timingPoint.StartTime);

                        // No timing points before the object? Just use the first timing point then, it has the correct
                        // BPM.
                        if (prevTimingPointIndex == -1)
                            prevTimingPointIndex = 0;

                        bpm = TimingPoints[prevTimingPointIndex].Bpm;
                    }
                    else
                    {
                        bpm = timingPoint.Bpm;
                    }

                    // The time gap is quarter of the milliseconds per beat.
                    timeGap = (int?) Math.Max(Math.Round(15000 / bpm), MINIMAL_GAP_LENGTH);
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

                        // If the next object is not an LN and it's the last object in the lane, or if it's an LN and
                        // not the last object in the lane, create a regular object at the next object's start position.
                        if ((secondNextObjectInLane == null) != nextObjectInLane.IsLongNote)
                            currentObject.EndTime = nextObjectInLane.StartTime;

                        // Filter out really short LNs or even negative length resulting from jacks or weird BPM values.
                        if (currentObject.EndTime - currentObject.StartTime < MINIMAL_LN_LENGTH)
                        {
                            // These get skipped entirely.
                            //
                            // Actually, there can be a degenerate pattern of multiple LNs with really short gaps
                            // in between them (less than MINIMAL_LN_LENGTH), which this logic will convert
                            // into nothing. That should be pretty rare though.
                            continue;
                        }
                    }
                }
                else
                {
                    // Regular objects are replaced with LNs starting from their start and ending quarter of a beat
                    // before the next object's start.
                    if (nextObjectInLane == null)
                    {
                        // If this is the last object in lane, though, then it's not included, and instead the previous
                        // LN spans up to this object's StartTime.
                        continue;
                    }

                    currentObject.EndTime = nextObjectInLane.StartTime - timeGap.Value;

                    // If the next object is not an LN and it's the last object in the lane, or if it's an LN and
                    // not the last object in the lane, this LN should span until its start.
                    if ((secondNextObjectInLane == null) == (nextObjectInLane.EndTime == 0))
                    {
                        currentObject.EndTime = nextObjectInLane.StartTime;
                    }

                    // Filter out really short LNs or even negative length resulting from jacks or weird BPM values.
                    if (currentObject.EndTime - currentObject.StartTime < MINIMAL_LN_LENGTH)
                    {
                        // These get converted back into regular objects.
                        currentObject.EndTime = 0;
                    }
                }

                newHitObjects.Add(currentObject);
            }

            // LN conversion can mess up the ordering, so sort it again. See the (this part can mess up the ordering)
            // comment above.
            HitObjects = newHitObjects.OrderBy(x => x.StartTime).ToList();
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
            values.AddRange(Enumerable.Range(0, GetKeyCount()).Select(x => x + 1));

            values.Shuffle(new Random(seed));

            for (var i = 0; i < HitObjects.Count; i++)
            {
                var temp = HitObjects[i];
                temp.Lane = values[temp.Lane - 1];
                HitObjects[i] = temp;
            }
        }

        /// <summary>
        ///     Flips the lanes of the HitObjects
        /// </summary>
        public void MirrorHitObjects()
        {
            for (var i = 0; i < HitObjects.Count; i++)
            {
                var temp = HitObjects[i];
                temp.Lane = GetKeyCount() - temp.Lane + 1;
                HitObjects[i] = temp;
            }
        }

        /// <summary>
        /// </summary>
        public void SortSliderVelocities() => SliderVelocities = SliderVelocities.OrderBy(x => x.StartTime).ToList();

        /// <summary>
        /// </summary>
        public void SortTimingPoints() => TimingPoints = TimingPoints.OrderBy(x => x.StartTime).ToList();

        /// <summary>
        ///     Gets the judgement of a particular hitobject in the map
        /// </summary>
        /// <param name="ho"></param>
        /// <returns></returns>
        public int GetHitObjectJudgementIndex(HitObjectInfo ho)
        {
            var index = -1;

            var total = 0;

            for (var i = 0; i < HitObjects.Count; i++)
            {
                if (HitObjects[i] == ho)
                    return total;

                if (HitObjects[i].IsLongNote)
                    total += 2;
                else
                    total += 1;
            }

            return index;
        }

        /// <summary>
        ///     Gets a hitobject at a particular judgement index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public HitObjectInfo GetHitObjectAtJudgementIndex(int index)
        {
            HitObjectInfo h = null;

            var total = 0;

            for (var i = 0; i < HitObjects.Count; i++)
            {
                total += 1;

                if (total - 1 == index)
                {
                    h = HitObjects[i];
                    break;
                }

                if (HitObjects[i].IsLongNote)
                    total += 1;

                if (total - 1 == index)
                {
                    h = HitObjects[i];
                    break;
                }
            }

            return h;
        }

        /// <summary>
        /// </summary>
        /// <param name="qua"></param>
        private static void RestoreDefaultValues(Qua qua)
        {
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
            if (checkValidity && !qua.IsValid())
                throw new ArgumentException("The .qua file is invalid. It does not have HitObjects, TimingPoints, its Mode is invalid or some hit objects are invalid.");

            // Try to sort the Qua before returning.
            qua.Sort();
        }
    }
}