/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    public class WristStateData
    {
        /// <summary>
        /// 
        /// </summary>
        public float WristDifficulty { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public float NextStateDelta => NextState.Time - Time;

        /// <summary>
        /// 
        /// </summary>
        public WristOrientation WristAction { get; set; } = WristOrientation.None;

        /// <summary>
        /// 
        /// </summary>
        public float Time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Finger WristPair { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public WristStateData NextState { get; }

        /// <summary>
        /// 
        /// </summary>
        public int RepetitionCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public WristStateData(WristStateData next) => NextState = next;
    }
}
