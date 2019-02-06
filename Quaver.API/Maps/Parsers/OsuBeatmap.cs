/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2019 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.Parsers
{
    public class OsuBeatmap
    {
        /// <summary>
        ///     The original file name of the .osu
        /// </summary>
        public string OriginalFileName { get; set; }

        /// <summary>
        ///     Is the peppy beatmap valid?
        /// </summary>
        public bool IsValid { get; set; }

        public string PeppyFileFormat { get; set; }

        // [General]
        public string AudioFilename { get; set; }
        public int AudioLeadIn { get; set; }
        public int PreviewTime { get; set; }
        public int Countdown { get; set; }
        public string SampleSet { get; set; }
        public float StackLeniency { get; set; }
        public int Mode { get; set; }
        public int LetterboxInBreaks { get; set; }
        public int SpecialStyle { get; set; }
        public int WidescreenStoryboard { get; set; }

        // [Editor]
        public string Bookmarks { get; set; }
        public float DistanceSpacing { get; set; }
        public int BeatDivisor { get; set; }
        public int GridSize { get; set; }
        public float TimelineZoom { get; set; }

        // [Metadata]
        public string Title { get; set; }
        public string TitleUnicode { get; set; }
        public string Artist { get; set; }
        public string ArtistUnicode { get; set; }
        public string Creator { get; set; }
        public string Version { get; set; }
        public string Source { get; set; }
        public string Tags { get; set; }
        public int BeatmapID { get; set; }
        public int BeatmapSetID { get; set; }

        // [Difficulty]
        public float HPDrainRate { get; set; }
        public int KeyCount { get; set; }
        public float OverallDifficulty { get; set; }
        public float ApproachRate { get; set; }
        public float SliderMultiplier { get; set; }
        public float SliderTickRate { get; set; }

        // [Events]
        public string Background { get; set; }

        // [TimingPoints]
        public List<OsuTimingPoint> TimingPoints { get; set; } = new List<OsuTimingPoint>();

        // [HitObjects]
        public List<OsuHitObject> HitObjects { get; set; } = new List<OsuHitObject>();

        /// <summary>
        ///     Ctor - Automatically parses a Peppy beatmap
        /// </summary>
        public OsuBeatmap(string filePath)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            if (!File.Exists(filePath.Trim()))
            {
                IsValid = false;
                return;
            }

            // Create a new beatmap object and default the validity to true.
            IsValid = true;
            OriginalFileName = filePath;

            // This will hold the section of the beatmap that we are parsing.
            var section = "";

            try
            {
                foreach (var raw_line in File.ReadAllLines(filePath))
                {
                    // Skip empty lines and comments.
                    if (string.IsNullOrWhiteSpace(raw_line)
                        || raw_line.StartsWith("//", StringComparison.Ordinal)
                        || raw_line.StartsWith(" ", StringComparison.Ordinal)
                        || raw_line.StartsWith("_", StringComparison.Ordinal))
                        continue;

                    var line = StripComments(raw_line);

                    switch (line.Trim())
                    {
                        case "[General]":
                            section = "[General]";
                            break;
                        case "[Editor]":
                            section = "[Editor]";
                            break;
                        case "[Metadata]":
                            section = "[Metadata]";
                            break;
                        case "[Difficulty]":
                            section = "[Difficulty]";
                            break;
                        case "[Events]":
                            section = "[Events]";
                            break;
                        case "[TimingPoints]":
                            section = "[TimingPoints]";
                            break;
                        case "[HitObjects]":
                            section = "[HitObjects]";
                            break;
                        case "[Colours]":
                            section = "[Colours]";
                            break;
                        default:
                            break;
                    }

                    // Parse Peppy file format
                    if (line.StartsWith("osu file format"))
                        PeppyFileFormat = line;

                    // Parse [General] Section
                    if (section.Equals("[General]"))
                    {
                        if (line.Contains(":"))
                        {
                            var key = line.Substring(0, line.IndexOf(':'));
                            var value = line.Split(':').Last().Trim();

                            switch (key.Trim())
                            {
                                case "AudioFilename":
                                    AudioFilename = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                                    break;
                                case "AudioLeadIn":
                                    AudioLeadIn = int.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "PreviewTime":
                                    PreviewTime = int.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "Countdown":
                                    Countdown = int.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "SampleSet":
                                    SampleSet = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value)); ;
                                    break;
                                case "StackLeniency":
                                    StackLeniency = float.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "Mode":
                                    Mode = int.Parse(value, CultureInfo.InvariantCulture);
                                    if (Mode != 3)
                                        IsValid = false;
                                    break;
                                case "LetterboxInBreaks":
                                    LetterboxInBreaks = int.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "SpecialStyle":
                                    SpecialStyle = int.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "WidescreenStoryboard":
                                    WidescreenStoryboard = int.Parse(value, CultureInfo.InvariantCulture);
                                    break;

                            }

                        }

                    }

                    // Parse [Editor] Data
                    if (section.Equals("[Editor]"))
                    {
                        if (line.Contains(":"))
                        {
                            var key = line.Substring(0, line.IndexOf(':'));
                            var value = line.Split(':').Last();

                            switch (key.Trim())
                            {
                                case "Bookmarks":
                                    Bookmarks = value;
                                    break;
                                case "DistanceSpacing":
                                    DistanceSpacing = float.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "BeatDivisor":
                                    BeatDivisor = int.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "GridSize":
                                    GridSize = int.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "TimelineZoom":
                                    TimelineZoom = float.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                            }
                        }

                    }

                    // Parse [Editor] Data
                    if (section.Equals("[Metadata]"))
                    {
                        if (line.Contains(":"))
                        {
                            var key = line.Substring(0, line.IndexOf(':'));
                            var value = line.Split(':').Last();

                            switch (key.Trim())
                            {
                                case "Title":
                                    Title = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                                    break;
                                case "TitleUnicode":
                                    TitleUnicode = value;
                                    break;
                                case "Artist":
                                    Artist = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                                    break;
                                case "ArtistUnicode":
                                    ArtistUnicode = value;
                                    break;
                                case "Creator":
                                    Creator = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                                    break;
                                case "Version":
                                    Version = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                                    break;
                                case "Source":
                                    Source = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                                    break;
                                case "Tags":
                                    Tags = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                                    break;
                                case "BeatmapID":
                                    BeatmapID = int.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "BeatmapSetID":
                                    BeatmapSetID = int.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                default:
                                    break;
                            }
                        }

                    }

                    // Parse [Difficulty] Data
                    if (section.Equals("[Difficulty]"))
                    {
                        if (line.Contains(":"))
                        {
                            var key = line.Substring(0, line.IndexOf(':'));
                            var value = line.Split(':').Last();

                            switch (key.Trim())
                            {
                                case "HPDrainRate":
                                    HPDrainRate = float.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "CircleSize":
                                    KeyCount = int.Parse(value, CultureInfo.InvariantCulture);
                                    if (KeyCount != 4 && KeyCount != 7)
                                        IsValid = false;
                                    break;
                                case "OverallDifficulty":
                                    OverallDifficulty = float.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "ApproachRate":
                                    ApproachRate = float.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "SliderMultiplier":
                                    SliderMultiplier = float.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "SliderTickRate":
                                    SliderTickRate = float.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                            }
                        }

                    }

                    // Parse [Events] Data
                    if (section.Equals("[Events]"))
                    {
                        // We only care about parsing the background path,
                        // So there's no need to parse the storyboard data.
                        if (line.ToLower().Contains("png") || line.ToLower().Contains("jpg") || line.ToLower().Contains("jpeg"))
                        {
                            var values = line.Split(',');
                            Background = values[2].Replace("\"", "");
                        }
                    }

                    try
                    {
                        // Parse [TimingPoints] Data
                        if (section.Equals("[TimingPoints]"))
                        {
                            if (line.Contains(","))
                            {
                                var values = line.Split(',');

                                var timingPoint = new OsuTimingPoint
                                {
                                    Offset = float.Parse(values[0], CultureInfo.InvariantCulture),
                                    MillisecondsPerBeat = float.Parse(values[1], CultureInfo.InvariantCulture),
                                    Meter = int.Parse(values[2], CultureInfo.InvariantCulture),
                                    SampleType = int.Parse(values[3], CultureInfo.InvariantCulture),
                                    SampleSet = int.Parse(values[4], CultureInfo.InvariantCulture),
                                    Volume = int.Parse(values[5], CultureInfo.InvariantCulture),
                                    Inherited = int.Parse(values[6], CultureInfo.InvariantCulture),
                                    KiaiMode = int.Parse(values[7], CultureInfo.InvariantCulture)
                                };

                                TimingPoints.Add(timingPoint);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        IsValid = false;
                    }

                    // Parse [HitObjects] Data
                    if (section.Equals("[HitObjects]"))
                    {
                        if (line.Contains(","))
                        {
                            var values = line.Split(',');

                            // We'll need to parse LNs differently than normal HitObjects,
                            // as they have a different syntax. 128 in the object's type
                            // signifies that it is an LN
                            var osuHitObject = new OsuHitObject
                            {
                                X = int.Parse(values[0], CultureInfo.InvariantCulture),
                                Y = int.Parse(values[1], CultureInfo.InvariantCulture),
                                StartTime = int.Parse(values[2], CultureInfo.InvariantCulture),
                                Type = (HitObjectType) int.Parse(values[3], CultureInfo.InvariantCulture),
                                HitSound = int.Parse(values[4], CultureInfo.InvariantCulture),
                                Additions = "0:0:0:0:"
                            };

                            // If it's an LN, we'll want to add the object's EndTime as well.
                            if (osuHitObject.Type.HasFlag(HitObjectType.Hold))
                            {
                                var endTime = values[5].Substring(0, values[5].IndexOf(":", StringComparison.Ordinal));
                                osuHitObject.EndTime = int.Parse(endTime, CultureInfo.InvariantCulture);
                            }

                            HitObjects.Add(osuHitObject);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                IsValid = false;
            }
        }

        /// <summary>
        ///     Strips comments from a line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static string StripComments(string line)
        {
            var index = line.IndexOf("//", StringComparison.Ordinal);
            if (index > 0)
                return line.Substring(0, index);
            return line;
        }

        /// <summary>
        ///     Converts an .osu file into a Qua object
        /// </summary>
        /// <returns></returns>
        public Qua ToQua()
        {
            // Init Qua with general information
            var qua = new Qua()
            {
                AudioFile = AudioFilename,
                SongPreviewTime = PreviewTime,
                BackgroundFile = Background,
                MapId = -1,
                MapSetId = -1,
                Title = Title,
                Artist = Artist,
                Source = Source,
                Tags = Tags,
                Creator = Creator,
                DifficultyName = Version,
                Description = $"This is a Quaver converted version of {Creator}'s map."
            };

            // Get the correct game mode based on the amount of keys the map has.
            switch (KeyCount)
            {
                case 4:
                    qua.Mode = GameMode.Keys4;
                    break;
                case 7:
                    qua.Mode = GameMode.Keys7;
                    break;
                default:
                    qua.Mode = (GameMode)(-1);
                    break;
            }

            // Get Timing Info
            foreach (var tp in TimingPoints)
                if (tp.Inherited == 1)
                    qua.TimingPoints.Add(new TimingPointInfo
                    {
                        StartTime = tp.Offset,
                        Bpm = 60000 / tp.MillisecondsPerBeat
                    });

            // Get SliderVelocity Info
            foreach (var tp in TimingPoints)
                if (tp.Inherited == 0)
                    qua.SliderVelocities.Add(new SliderVelocityInfo
                    {
                        StartTime = tp.Offset,
                        Multiplier = (-100 / tp.MillisecondsPerBeat).Clamp(0.1f, 10)
                    });

            // Get HitObject Info
            foreach (var hitObject in HitObjects)
            {
                // Get the keyLane the hitObject is in
                var keyLane = (int) (hitObject.X / (512d / KeyCount)).Clamp(0, KeyCount - 1) + 1;

                // Add HitObjects to the list depending on the object type
                if (hitObject.Type.HasFlag(HitObjectType.Circle))
                {
                    qua.HitObjects.Add(new HitObjectInfo
                    {
                        StartTime = hitObject.StartTime,
                        Lane = keyLane,
                        EndTime = 0,
                        HitSound = HitSounds.Normal
                    });
                }
                else if (hitObject.Type.HasFlag(HitObjectType.Hold))
                {
                    qua.HitObjects.Add(new HitObjectInfo
                    {
                        StartTime = hitObject.StartTime,
                        Lane = keyLane,
                        EndTime = hitObject.EndTime,
                        HitSound = HitSounds.Normal
                    });
                }
            }

            // Do a validity check and some final sorting.
            if (!qua.IsValid())
                throw new ArgumentException();

            qua.Sort();

            return qua;
        }
    }

    /// <summary>
    ///     Enumeration of the hit object types.
    /// </summary>
    [Flags]
    public enum HitObjectType
    {
        Circle = 1 << 0,
        Slider = 1 << 1,
        NewCombo = 1 << 2,
        Spinner = 1 << 3,
        ComboOffset = 1 << 4 | 1 << 5 | 1 << 6,
        Hold = 1 << 7
    }

    /// <summary>
    ///     Struct for the timing point data.
    /// </summary>
    public struct OsuTimingPoint
    {
        public float Offset { get; set; }
        public float MillisecondsPerBeat { get; set; }
        public int Meter { get; set; }
        public int SampleType { get; set; }
        public int SampleSet { get; set; }
        public int Volume { get; set; }
        public int Inherited { get; set; }
        public int KiaiMode { get; set; }
    }

    /// <summary>
    ///  Struct for all the hit object data.
    /// </summary>
    public struct OsuHitObject
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int StartTime { get; set; }
        public HitObjectType Type { get; set; }
        public int HitSound { get; set; }
        public int EndTime { get; set; }
        public string Additions { get; set; }
        public bool Key1 { get; set; }
        public bool Key2 { get; set; }
        public bool Key3 { get; set; }
        public bool Key4 { get; set; }
        public bool Key5 { get; set; }
        public bool Key6 { get; set; }
        public bool Key7 { get; set; }
    }
}
