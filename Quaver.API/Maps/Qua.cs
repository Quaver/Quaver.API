using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quaver.API.Enums;
using Quaver.API.Maps.Parsers;
using YamlDotNet.Serialization;

namespace Quaver.API.Maps
{
    public class Qua
    {
        /// <summary>
        ///     The name of the audio file
        /// </summary>
        public string AudioFile { get; private set; }

        /// <summary>
        ///     Time in milliseconds of the song where the preview starts
        /// </summary>
        public int SongPreviewTime { get; private set; }

        /// <summary>
        ///     The name of the background file
        /// </summary>
        public string BackgroundFile { get; private set; }

        /// <summary>
        ///     The unique Map Identifier (-1 if not submitted)
        /// </summary>
        public int MapId { get; private set; } = -1;

        /// <summary>
        ///     The unique Map Set identifier (-1 if not submitted)
        /// </summary>
        public int MapSetId { get; private set; } = -1;

        /// <summary>
        ///     The game mode for this map
        /// </summary>
        public GameMode Mode { get; private set; }

        /// <summary>
        ///     The title of the song
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        ///     The artist of the song
        /// </summary>
        public string Artist { get; private set; }

        /// <summary>
        ///     The source of the song (album, mixtape, etc.)
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        ///     Any tags that could be used to help find the song.
        /// </summary>
        public string Tags { get; private set; }

        /// <summary>
        ///     The creator of the map
        /// </summary>
        public string Creator { get; private set; }

        /// <summary>
        ///     The difficulty name of the map.
        /// </summary>
        public string DifficultyName { get; private set; }

        /// <summary>
        ///     A description about this map.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        ///     TimingPoint .qua data
        /// </summary>
        public List<TimingPointInfo> TimingPoints { get; private set; }
        
        /// <summary>
        ///     Slider Velocity .qua data
        /// </summary>
        public List<SliderVelocityInfo> SliderVelocities { get; set; }

        /// <summary>
        ///     HitObject .qua data
        /// </summary>
        public List<HitObjectInfo> HitObjects { get; private set; }

        /// <summary>
        ///     Is the Qua actually valid?
        /// </summary>
        [YamlIgnore]
        public bool IsValidQua { get; set; }

        /// <summary>
        ///     Takes in a path to a .qua file and attempts to parse it.
        ///     Will throw an error if unable to be parsed.
        /// </summary>
        /// <param name="path"></param>
        public static Qua Parse(string path)
        {
            Qua qua;

            using (var file = File.OpenText(path))
            {
                var serializer = new Deserializer();
                qua = (Qua)serializer.Deserialize(file, typeof(Qua));
            }

            // Check the Qua object's validity.
            qua.IsValidQua = CheckQuaValidity(qua);

            // Try to sort the Qua before returning.
            qua.Sort();

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

            // Save
            using (var file = File.CreateText(path))
            {
                var serializer = new Serializer();
                serializer.Serialize(file, this);
            }
        }

        /// <summary>
        ///     Placeholder
        /// </summary>
        /// <returns></returns>
        public float CalculateFakeDifficulty(float rate = 1.0f)
        {
            return HitObjects.Count / (FindSongLength(this) / (1000f * rate));
        }

        /// <summary>
        ///     In Quaver, the key count is defined by the mode.
        ///     See: GameModes.cs
        /// </summary>
        /// <returns></returns>
        public int FindKeyCountFromMode()
        {
            switch (Mode)
            {
                case GameMode.Keys4:
                    return 4;
                case GameMode.Keys7:
                    return 7;
                default:
                    return -1;
            }
        }

        /// <summary>
        /// Finds the most common BPM in a Qua object.
        /// </summary>
        /// <param name="qua"></param>
        /// <returns></returns>
        public static double FindCommonBpm(Qua qua)
        {
            if (qua.TimingPoints.Count == 0)
                return 0;

            return Math.Round(qua.TimingPoints.GroupBy(i => i.Bpm).OrderByDescending(grp => grp.Count())
                .Select(grp => grp.Key).First(), 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        ///     Finds the length of the map
        /// </summary>
        /// <param name="qua"></param>
        /// <returns></returns>
        public static int FindSongLength(Qua qua)
        {
            return qua.HitObjects.Count == 0 ? 0 : qua.HitObjects.Max(x => Math.Max(x.StartTime, x.EndTime));
        }

        /// <summary>
        ///     Checks a Qua object's validity.
        /// </summary>
        /// <param name="qua"></param>
        /// <returns></returns>
        public static bool CheckQuaValidity(Qua qua)
        {
            // If there aren't any HitObjects
            if (qua.HitObjects.Count == 0)
                return false;

            // If there aren't any TimingPoints
            if (qua.TimingPoints.Count == 0)
                return false;

            // Check if the mode is actually valid
            if (!Enum.IsDefined(typeof(GameMode), qua.Mode))
                return false;

            return true;
        }

        /// <summary>
        ///     Does some sorting of the Qua
        /// </summary>
        private void Sort()
        {
            try
            {
                HitObjects = HitObjects.OrderBy(x => x.StartTime).ToList();
                TimingPoints = TimingPoints.OrderBy(x => x.StartTime).ToList();
                SliderVelocities = SliderVelocities.OrderBy(x => x.StartTime).ToList();
            }
            catch (Exception e)
            {
                IsValidQua = false;
            }
        }

        /// <summary>
        ///     Converts an .osu file into a Qua object
        /// </summary>
        /// <param name="osu"></param>
        /// <returns></returns>
        public static Qua ConvertOsuBeatmap(PeppyBeatmap osu)
        {
            // Init Qua with general information
            var qua = new Qua()
            {
                AudioFile = osu.AudioFilename,
                SongPreviewTime = osu.PreviewTime,
                BackgroundFile = osu.Background,
                MapId = -1,
                MapSetId = -1,
                Title = osu.Title,
                Artist = osu.Artist,
                Source = osu.Source,
                Tags = osu.Tags,
                Creator = osu.Creator,
                DifficultyName = osu.Version,
                Description = $"This is a Quaver converted version of {osu.Creator}'s map."
            };

            // Get the correct game mode based on the amount of keys the map has.
            switch (osu.KeyCount)
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

            // Initialize lists
            qua.TimingPoints = new List<TimingPointInfo>();
            qua.SliderVelocities = new List<SliderVelocityInfo>();
            qua.HitObjects = new List<HitObjectInfo>();

            // Get Timing Info
            foreach (var tp in osu.TimingPoints)
                if (tp.Inherited == 1)
                    qua.TimingPoints.Add(new TimingPointInfo { StartTime = tp.Offset, Bpm = 60000 / tp.MillisecondsPerBeat });

            // Get SliderVelocity Info
            foreach (var tp in osu.TimingPoints)
                if (tp.Inherited == 0)
                    qua.SliderVelocities.Add(new SliderVelocityInfo { StartTime = tp.Offset, Multiplier = (float)Math.Round(0.10 / ((tp.MillisecondsPerBeat / -100) / 10), 2) });

            // Get HitObject Info
            foreach (var hitObject in osu.HitObjects)
            {
                // Get the keyLane the hitObject is in
                var keyLane = 0;

                if (hitObject.Key1)
                    keyLane = 1;
                else if (hitObject.Key2)
                    keyLane = 2;
                else if (hitObject.Key3)
                    keyLane = 3;
                else if (hitObject.Key4)
                    keyLane = 4;
                else if (hitObject.Key5)
                    keyLane = 5;
                else if (hitObject.Key6)
                    keyLane = 6;
                else if (hitObject.Key7)
                    keyLane = 7;

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (hitObject.Type)
                {
                    // Add HitObjects to the list depending on the object type
                    case 1:
                    case 5:
                        qua.HitObjects.Add(new HitObjectInfo
                        {
                            StartTime = hitObject.StartTime,
                            Lane = keyLane,
                            EndTime = 0,
                            HitSound = (HitSounds) hitObject.HitSound
                        });
                        break;
                    case 128:
                    case 22:
                        qua.HitObjects.Add(new HitObjectInfo
                        {
                            StartTime = hitObject.StartTime,
                            Lane = keyLane,
                            EndTime = hitObject.EndTime,
                            HitSound = (HitSounds) hitObject.HitSound
                        });
                        break;
                }
            }

            // Do a validity check and some final sorting.
            qua.IsValidQua = CheckQuaValidity(qua);
            qua.Sort();

            return qua;
        }

        /// <summary>
        ///     Converts a StepMania file object into a list of Qua.
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        public static List<Qua> ConvertStepManiaChart(StepManiaFile sm)
        {
            var maps = new List<Qua>();

            foreach (var chart in sm.Charts)
            {
                var baseQua = new Qua()
                {
                    Artist = sm.Artist,
                    Title = sm.Title,
                    AudioFile = sm.Music,
                    BackgroundFile = sm.Background.Replace(".jpeg", ".jpg"),
                    Creator = sm.Credit,
                    Description = "This map was converted from StepMania",
                    Mode = GameMode.Keys4,
                    DifficultyName = chart.Difficulty,
                    Source = "StepMania",
                    Tags = "StepMania",
                    TimingPoints = new List<TimingPointInfo>(),
                    HitObjects = new List<HitObjectInfo>(),
                    SliderVelocities = new List<SliderVelocityInfo>(),
                    SongPreviewTime = (int)sm.SampleStart
                };
                
                // Convert BPM to Quaver Timing Points
                var totalBpmTrackTime = 0f;

                for (var i = 0; i < sm.Bpms.Count; i++)
                {
                    // Handle the first BPM point
                    if (sm.Bpms[i].Beats == 0 && i == 0)
                    {
                        totalBpmTrackTime += 60000 / sm.Bpms[i].BeatsPerMinute * sm.Bpms[i].Beats - (sm.Offset * 1000);
                       
                        // Add the first timing point
                        baseQua.TimingPoints.Add(new TimingPointInfo { StartTime = totalBpmTrackTime, Bpm = sm.Bpms[i].BeatsPerMinute });
                    }

                    // Add the timing point ahead of it if it exists
                    if (sm.Bpms.Count <= i + 1)
                        continue;

                    totalBpmTrackTime += 60000 / sm.Bpms[i].BeatsPerMinute * Math.Abs(sm.Bpms[i + 1].Beats - sm.Bpms[i].Beats);
                    baseQua.TimingPoints.Add(new TimingPointInfo { StartTime = totalBpmTrackTime, Bpm = sm.Bpms[i + 1].BeatsPerMinute });
                }

                // Convert StepMania note rows to Quaver HitObjects
                var currentBeat = 0;
                var totalBeatTrackTime = -sm.Offset * 1000f;
                foreach (var measure in chart.Measures)
                {
                    foreach (var beat in measure.NoteRows)
                    {
                        currentBeat++;

                        // Get the amount of milliseconds for this particular measure
                        var msPerNote = 60000 / sm.Bpms[StepManiaFile.GetBpmIndexFromBeat(sm, currentBeat)].BeatsPerMinute * 4 / measure.NoteRows.Count;

                        // If we're on the first beat, then the current track time should be the offset of the map.
                        totalBeatTrackTime += msPerNote;

                        // Convert all Lane's HitObjects
                        StepManiaFile.ConvertLaneToHitObject(baseQua.HitObjects, totalBeatTrackTime, 1, beat.Lane1);
                        StepManiaFile.ConvertLaneToHitObject(baseQua.HitObjects, totalBeatTrackTime, 2, beat.Lane2);
                        StepManiaFile.ConvertLaneToHitObject(baseQua.HitObjects, totalBeatTrackTime, 3, beat.Lane3);
                        StepManiaFile.ConvertLaneToHitObject(baseQua.HitObjects, totalBeatTrackTime, 4, beat.Lane4);
                    }
                }

                maps.Add(baseQua);
            }

            return maps;
        }
    }
}