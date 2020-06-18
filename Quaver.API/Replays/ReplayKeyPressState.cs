/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;

namespace Quaver.API.Replays
{
    /// <summary>
    ///     Bitwise combination of the keys that were pressed in a given replay frame.
    /// </summary>
    [Flags]
    public enum ReplayKeyPressState
    {
        K1 = 1 << 0,
        K2 = 1 << 1,
        K3 = 1 << 2,
        K4 = 1 << 3,
        K5 = 1 << 4,
        K6 = 1 << 5,
        K7 = 1 << 6,
        K8 = 1 << 7,
        K9 = 1<< 8 // Scratch Lane Second Key on 7K+1
    }
}
