/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2019 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using YamlDotNet.Serialization;

namespace Quaver.API.Maps.Structures
{
    /// <summary>
    ///     SliderVelocities section of the .qua
    /// </summary>
    [Serializable]
    [MoonSharpUserData]
    public class SliderVelocityInfo : IStartTime
    {
        /// <summary>
        ///     The time in milliseconds when the new SliderVelocity section begins
        /// </summary>
        public float StartTime
        {
            get;
            [MoonSharpVisible(false)] set;
        }

        /// <summary>
        ///     The velocity multiplier relative to the current timing section's BPM
        /// </summary>
        public float Multiplier
        {
            get;
            [MoonSharpVisible(false)] set;
        }

        /// <summary>
        ///     Returns if the SV is allowed to be edited in lua scripts
        /// </summary>
        [YamlIgnore]
        public bool IsEditableInLuaScript
        {
            get;
            [MoonSharpVisible(false)] set;
        }

        /// <summary>
        ///     Sets the start time of the SV.
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
        ///     Sets the multiplier of the SV.
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

        /// <summary>
        ///     By-value comparer, auto-generated by Rider.
        /// </summary>
        private sealed class ByValueEqualityComparer : IEqualityComparer<SliderVelocityInfo>
        {
            public bool Equals(SliderVelocityInfo x, SliderVelocityInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;

                return x.StartTime.Equals(y.StartTime) && x.Multiplier.Equals(y.Multiplier);
            }

            public int GetHashCode(SliderVelocityInfo obj)
            {
                unchecked
                {
                    return (obj.StartTime.GetHashCode() * 397) ^ obj.Multiplier.GetHashCode();
                }
            }
        }

        public static IEqualityComparer<SliderVelocityInfo> ByValueComparer { get; } = new ByValueEqualityComparer();
    }
}
