using System;

namespace Quaver.API.Maps.Structures
{
    /// <summary>
    ///     SliderVelocities section of the .qua   
    /// </summary>
    [Serializable]
    public class SliderVelocityInfo
    {
        /// <summary>
        ///     The time in milliseconds when the new SliderVelocity section begins
        /// </summary>
        public float StartTime { get; set; }

        /// <summary>
        ///     The velocity multiplier relative to the current timing section's BPM
        /// </summary>
        public float Multiplier { get; set; }
    }
}