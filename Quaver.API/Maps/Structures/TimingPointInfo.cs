using System;
using YamlDotNet.Serialization;

namespace Quaver.API.Maps.Structures
{
    /// <summary>
    ///     TimingPoints section of the .qua
    /// </summary>
    [Serializable]
    public class TimingPointInfo
    {
        /// <summary>
        ///     The time in milliseconds for when this timing point begins
        /// </summary>
        public float StartTime { get; set; }

        /// <summary>
        ///     The BPM during this timing point
        /// </summary>
        public float Bpm { get; set; }

        /// <summary>
        ///     The amount of milliseconds per beat this one takes up.
        /// </summary>
        [YamlIgnore]
        public float MillisecondsPerBeat => 60000 / Bpm;
    }
}