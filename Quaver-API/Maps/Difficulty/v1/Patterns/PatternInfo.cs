using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Maps.Difficulty.v1.Patterns
{
    public interface IPattern
    {
        /// <summary>
        ///     The total time this pattern takes
        /// </summary>
        int TotalTime { get; set; }

        /// <summary>
        ///     The time of the hit object where this pattern begins
        /// </summary>
        int StartingObjectTime { get; set; }

        /// <summary>
        ///     The list of hitobjects in this pattern
        /// </summary>
        List<HitObjectInfo> HitObjects { get; set; }
    }

    public enum PatternType
    {
        Vibro,
        Jack,
        Chord,
        Stream
    }

    public struct PatternList
    {
        /// <summary>
        ///     Jack Patterns
        /// </summary>
        public List<JackPatternInfo> Jacks { get; set; }

        /// <summary>
        ///     Vibro Patterns
        /// </summary>
        public List<JackPatternInfo> Vibro { get; set; }

        /// <summary>
        ///     Chord Patterns
        /// </summary>
        public List<ChordPatternInfo> Chords { get; set; }

        /// <summary>
        ///     Stream patterns
        /// </summary>
        public List<StreamPatternInfo> Streams { get; set; }
    }

    public class ChordPatternInfo : IPattern
    {
        /// <summary>
        ///     The total time the pattern takes
        /// </summary>
        public int TotalTime { get; set; }

        /// <summary>
        ///     The time the pattern starts
        /// </summary>
        public int StartingObjectTime { get; set; }

        /// <summary>
        ///     The list of objects in the pattern
        /// </summary>
        public List<HitObjectInfo> HitObjects { get; set; }

        /// <summary>
        ///     The type of chord this pattern is
        /// </summary>
        public ChordType ChordType { get; set; }
    }

    public enum ChordType
    {
        Jump, // 2 Note Chord
        Hand, // 3 Note Chord
        Quad, // 4 Note Chord
        FivePlus // 5 Plus note chord (7k, or stacked notes)
    }

    public class JackPatternInfo : IPattern
    {
        /// <summary>
        ///     The total time this pattern takes
        /// </summary>
        public int TotalTime { get; set; }

        /// <summary>
        ///     The time of the hit object where the pattern begins
        /// </summary>
        public int StartingObjectTime { get; set; }

        /// <summary>
        ///     The lane the pattern takes place in
        /// </summary>
        public int Lane { get; set; }

        /// <summary>
        ///     The list of HitObjects in this pattern
        /// </summary>
        public List<HitObjectInfo> HitObjects { get; set; }
    }

    public class StreamPatternInfo : IPattern
    {
        /// <summary>
        ///     The amount of time the pattern lasts
        /// </summary>
        public int TotalTime { get; set; }

        /// <summary>
        ///     The time of the hit object where the pattern begins
        /// </summary>
        public int StartingObjectTime { get; set; }

        /// <summary>
        ///     The list of HitObjects in this vibro pattern
        /// </summary>
        public List<HitObjectInfo> HitObjects { get; set; }
    }
}
