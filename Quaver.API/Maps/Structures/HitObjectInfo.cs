/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2019 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Quaver.API.Enums;
using YamlDotNet.Serialization;

namespace Quaver.API.Maps.Structures
{

    /// <summary>
    ///     HitObjects section of the .qua
    /// </summary>
    [Serializable]
    public struct HitObjectInfo
    {
        /// <summary>
        ///     The time in milliseconds when the HitObject is supposed to be hit.
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        ///     The lane the HitObject falls in
        /// </summary>
        public int Lane { get; set; }

        /// <summary>
        ///     The endtime of the HitObject (if greater than 0, it's considered a hold note.)
        /// </summary>
        public int EndTime { get; set; }

        /// <summary>
        ///     Bitwise combination of hit sounds for this object
        /// </summary>
        public HitSounds HitSound { get; set; }

        /// <summary>
        ///     If the object is a long note. (EndTime > 0)
        /// </summary>
        [YamlIgnore]
        public bool IsLongNote => EndTime > 0;

        /// <summary>
        ///     Gets the timing point this object is in range of.
        /// </summary>
        /// <returns></returns>
        public TimingPointInfo GetTimingPoint(List<TimingPointInfo> timingPoints)
        {
            // Search through the entire list for the correct point
            for (var i = timingPoints.Count - 1; i >= 0; i--)
            {
                if (StartTime >= timingPoints[i].StartTime)
                    return timingPoints[i];
            }

            return timingPoints.First();
        }
    }
}
