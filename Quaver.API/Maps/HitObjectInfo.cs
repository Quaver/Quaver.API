using System;
using Quaver.API.Enums;

namespace Quaver.API.Maps
{
    
    /// <summary>
    ///     HitObjects section of the .qua
    /// </summary>
    public class HitObjectInfo : ICloneable
    {
        /// <summary>
        ///     The time in milliseconds when the HitObject is supposed to be hit.
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        ///     The lane the HitObject falls in
        /// </summary>
        public int Lane { get; set; } = 1;

        /// <summary>
        ///     The endtime of the HitObject (if greater than 0, it's considered a hold note.)
        /// </summary>
        public int EndTime { get; set; }

        /// <summary>
        ///     Bitwise combination of hit sounds for this object
        /// </summary>
        public HitSounds HitSound { get; set; } = HitSounds.Normal;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}