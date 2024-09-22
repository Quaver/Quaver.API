using System.Collections.Generic;
using MoonSharp.Interpreter;
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
        public float InitialScrollVelocity
        {
            get;
            [MoonSharpHidden] set;
        } = 1;

        public List<SliderVelocityInfo> ScrollVelocities
        {
            get;
            [MoonSharpHidden] set;
        } = new List<SliderVelocityInfo>();
    }
}