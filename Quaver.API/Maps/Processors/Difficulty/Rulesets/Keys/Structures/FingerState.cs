/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    /// <summary>
    ///     Is determined by the finger state of a hand at a given moment
    /// </summary>
    [Flags]
    public enum FingerState
    {
        None = 0,
        Index = 1 << 0,
        Middle = 1 << 1,
        Ring = 1 << 2,
        Pinkie = 1 << 3,
        Thumb = 1 << 4
    }
}
