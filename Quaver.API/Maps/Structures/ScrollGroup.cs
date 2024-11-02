using System.Collections.Generic;
using System.Linq;
using MoonSharp.Interpreter;
using Quaver.API.Helpers;
using YamlDotNet.Serialization;

namespace Quaver.API.Maps.Structures
{
    /// <summary>
    ///     When a <see cref="HitObjectInfo"/> is added to this group,
    ///     it will go through the following <see cref="ScrollVelocities"/> instead of the default one
    ///     (<see cref="Qua.SliderVelocities"/>)
    /// </summary>
    [MoonSharpUserData]
    public class ScrollGroup : TimingGroup
    {
        public float InitialScrollVelocity { get; [MoonSharpHidden] set; } = 1;

        public List<SliderVelocityInfo> ScrollVelocities { get; [MoonSharpHidden] set; } =
            new List<SliderVelocityInfo>();

        public SliderVelocityInfo GetScrollVelocityAt(double time)
        {
            return ScrollVelocities.AtTime((float)time);
        }

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool Equals(ScrollGroup other)
        {
            return base.Equals(other) && InitialScrollVelocity.Equals(other.InitialScrollVelocity) &&
                   ScrollVelocities.SequenceEqual(other.ScrollVelocities, SliderVelocityInfo.ByValueComparer);
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ScrollGroup)obj);
        }
    }
}