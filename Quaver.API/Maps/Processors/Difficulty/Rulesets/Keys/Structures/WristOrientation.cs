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
    /// Will Represent Wrist Action
    /// - Up = Easier to play when wrist is up. (Think: Mashing)
    /// - Down = Player forced to have wrist down due to LN
    /// </summary>
    public enum WristOrientation
    {
        None,
        Up,
        Down
    }
}
