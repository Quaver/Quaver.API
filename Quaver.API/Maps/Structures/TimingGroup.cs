using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;

namespace Quaver.API.Maps.Structures
{
    /// <summary>
    ///     This is a base class that should not be serialized directly.
    ///     A timing group has custom properties that can be applied to an entire group of <see cref="HitObjectInfo"/>s.
    ///     You can add a <see cref="HitObjectInfo"/> to the group by setting <see cref="HitObjectInfo.TimingGroup"/>
    ///     to the <see cref="Id"/> of the desired <see cref="TimingGroup"/>.
    /// </summary>
    [MoonSharpUserData]
    public abstract class TimingGroup
    {
    }
}