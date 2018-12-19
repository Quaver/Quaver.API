using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Quaver.API.Enums;
using Quaver.API.Maps.Parsers;
using Quaver.API.Maps.Processors.Difficulty;
using Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys;
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
        ///     Ctor
        /// </summary>
        public Qua() {}
 
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
            if (!qua.IsValid())
                throw new ArgumentException(".qua file does not have HitObjects, TimingPoints, or Mode invalid");

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
            return Enum.IsDefined(typeof(GameMode), Mode);
        }
        
        /// <summary>
        ///     Does some sorting of the Qua
        /// </summary>
        public void Sort()
        {
            HitObjects = HitObjects.OrderBy(x => x.StartTime).ToList();
            TimingPoints = TimingPoints.OrderBy(x => x.StartTime).ToList();
            SliderVelocities = SliderVelocities.OrderBy(x => x.StartTime).ToList();
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
        /// Finds the most common BPM in a Qua object.
        /// </summary>
        /// <returns></returns>
        public double GetCommonBpm()
        {
            if (TimingPoints.Count == 0)
                return 0;

            return Math.Round(TimingPoints.GroupBy(i => i.Bpm).OrderByDescending(grp => grp.Count())
                .Select(grp => grp.Key).First(), 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        ///     Gets the timing point at a particular time in the map.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public TimingPointInfo GetTimingPointAt(double time)
        {
            var point = TimingPoints.FindLast(x => x.StartTime <= time);
            
            // If the point can't be found, we want to return either null if there aren't
            // any points, or the first timing point, since it'll be considered as apart of it anyway.
            if (point == null)
                return TimingPoints.Count == 0 ? null : TimingPoints.First();

            return point;
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
        /// <param name="constants"></param>
        /// <param name="mods"></param>
        /// <returns></returns>
        public DifficultyProcessor SolveDifficulty(DifficultyConstantsKeys constants, ModIdentifier mods = ModIdentifier.None)
        {
            switch (Mode)
            {
                case GameMode.Keys4:
                    return new DifficultyProcessorKeysNEW(this, constants, mods);
                case GameMode.Keys7:
                    return new DifficultyProcessorKeysNEW(this, constants, mods);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        public override string ToString() => $"{Artist} - {Title} [{DifficultyName}]";
    }
}