using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Quaver.API.Helpers;
using YamlDotNet.Serialization;

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
        /// <summary>
        ///     The color of the layer (default is white)
        /// </summary>
        public string ColorRgb { get; [MoonSharpVisible(false)] set; }

        /// <summary>
        ///     Is the timing group hidden in the editor?
        /// </summary>
        public bool Hidden { get; [MoonSharpVisible(false)] set; }

        /// <summary>
        ///     Converts the stringified color to a System.Drawing color
        /// </summary>
        /// <returns></returns>
        [MoonSharpVisible(false)]
        public Color GetColor() =>
            new Drain<char>(ColorRgb, ',') is var (tr, (tg, (tb, _))) &&
            byte.TryParse(tr, out var r) &&
            byte.TryParse(tg, out var g) &&
            byte.TryParse(tb, out var b)
                ? Color.FromArgb(r, g, b)
                : Color.White;

        /// <summary>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool Equals(TimingGroup other)
        {
            return ColorRgb == other.ColorRgb && Hidden == other.Hidden;
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
            return Equals((TimingGroup)obj);
        }
    }
}