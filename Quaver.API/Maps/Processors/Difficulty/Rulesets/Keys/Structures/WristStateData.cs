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
    /// <summary>
    /// Used to determine the wrist's state during gameplay for optimal difficulty calculation.
    /// </summary>
    public class WristStateData
    {
        /// <summary>
        ///     Difficulty Multiplier affected by this Wrist State.
        /// </summary>
        public double Difficulty { get; set; } = 1;

        /// <summary>
        ///     Delta between Next State and Current State.
        /// </summary>
        public double NextStateDelta => NextState.Time - Time;

        /// <summary>
        ///     Determined by this Wrist State's Orientation.
        /// </summary>
        public WristOrientation WristOrientation { get; set; } = WristOrientation.None;

        /// <summary>
        ///     Determines this Wrist State's time of effect.
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        ///     Determined by which fingers affect this Wrist State.
        /// </summary>
        public Finger WristPair { get; set; }

        /// <summary>
        ///     Next Wrist State Reference.
        /// </summary>
        public WristStateData NextState { get; }

        /// <summary>
        ///     Repetition Count for Simple Jacks.
        /// </summary>
        public int RepetitionCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public WristStateData(WristStateData next) => NextState = next;
    }
}
