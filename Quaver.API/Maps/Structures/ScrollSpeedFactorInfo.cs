using System;
using System.Collections.Generic;
using System.Text;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using YamlDotNet.Serialization;

namespace Quaver.API.Maps.Structures
{
    /// <summary>
    ///     ScrollSpeedFactors section of the .qua
    /// </summary>
    [Serializable]
    [MoonSharpUserData]
    public class ScrollSpeedFactorInfo : IStartTime
    {
        /// <summary>
        ///     The time in milliseconds at which the scroll speed factor will be exactly <see cref="Multiplier"/>
        /// </summary>
        public float StartTime { get; [MoonSharpHidden] set; }

        /// <summary>
        ///     The multiplier given to the scroll speed.
        ///     It will be lerped to the next scroll speed factor like keyframes, unless this is the last factor change.
        /// </summary>
        public float Multiplier { get; [MoonSharpHidden] set; }

        /// <summary>
        ///     Returns if the SSF is allowed to be edited in lua scripts
        /// </summary>
        [YamlIgnore]
        public bool IsEditableInLuaScript
        {
            get;
            [MoonSharpVisible(false)]
            set;
        }

        /// <summary>
        ///     Sets the start time of the SSF.
        ///     FOR USE IN LUA SCRIPTS ONLY.
        /// </summary>
        /// <param name="time"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetStartTime(float time)
        {
            ThrowUneditableException();
            StartTime = time;
        }

        /// <summary>
        ///     Sets the multiplier of the SSF.
        ///     FOR USE IN LUA SCRIPTS ONLY.
        /// </summary>
        /// <param name="multiplier"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetMultiplier(float multiplier)
        {
            ThrowUneditableException();
            Multiplier = multiplier;
        }

        /// <summary>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void ThrowUneditableException()
        {
            if (!IsEditableInLuaScript)
                throw new InvalidOperationException("Value is not allowed to be edited in lua scripts.");
        }

        private sealed class StartTimeRelationalComparer : IComparer<ScrollSpeedFactorInfo>
        {
            public int Compare(ScrollSpeedFactorInfo x, ScrollSpeedFactorInfo y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;
                return x.StartTime.CompareTo(y.StartTime);
            }
        }

        public static IComparer<ScrollSpeedFactorInfo> StartTimeComparer { get; } = new StartTimeRelationalComparer();

        /// <summary>
        ///     By-value comparer, auto-generated by Rider.
        /// </summary>
        private sealed class ByValueEqualityComparer : IEqualityComparer<ScrollSpeedFactorInfo>
        {
            public bool Equals(ScrollSpeedFactorInfo x, ScrollSpeedFactorInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.StartTime.Equals(y.StartTime) && x.Multiplier.Equals(y.Multiplier);
            }

            public int GetHashCode(ScrollSpeedFactorInfo obj)
            {
                return HashCode.Combine(obj.StartTime, obj.Multiplier);
            }
        }

        public static IEqualityComparer<ScrollSpeedFactorInfo> ByValueComparer { get; } = new ByValueEqualityComparer();
    }
}