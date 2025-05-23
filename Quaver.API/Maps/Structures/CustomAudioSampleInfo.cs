/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2019 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;

namespace Quaver.API.Maps.Structures
{
    /// <summary>
    ///     CustomAudioSamples section of the .qua
    /// </summary>
    [Serializable]
    public class CustomAudioSampleInfo
    {
        /// <summary>
        ///     The path to the audio sample.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        ///     If true, the audio sample is always played back at 1.0x speed, regardless of the rate.
        /// </summary>
        public bool UnaffectedByRate { get; set; }

        /// <summary>
        ///     By-value comparer, auto-generated by Rider.
        /// </summary>
        private sealed class ByValueEqualityComparer : IEqualityComparer<CustomAudioSampleInfo>
        {
            public bool Equals(CustomAudioSampleInfo x, CustomAudioSampleInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;

                return string.Equals(x.Path, y.Path) && x.UnaffectedByRate == y.UnaffectedByRate;
            }

            public int GetHashCode(CustomAudioSampleInfo obj)
            {
                unchecked
                {
                    return ((obj.Path != null ? obj.Path.GetHashCode() : 0) * 397) ^ obj.UnaffectedByRate.GetHashCode();
                }
            }
        }

        public static IEqualityComparer<CustomAudioSampleInfo> ByValueComparer { get; } = new ByValueEqualityComparer();
    }
}
