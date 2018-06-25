using System;

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
    }
}