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

namespace Quaver.API.Enums
{
    [Flags]
    public enum ModIdentifier
    {
        None = -1,
        NoSliderVelocity = 1 << 0, // No Slider Velocity
        Speed05X = 1 << 1, // Speed 0.5x,
        Speed06X = 1 << 2, // Speed 0.6x
        Speed07X = 1 << 3, // Speed 0.7x
        Speed08X = 1 << 4, // Speed 0.8x
        Speed09X = 1 << 5, // Speed 0.9x
        Speed11X = 1 << 6, // Speed 1.1x
        Speed12X = 1 << 7, // Speed 1.2x
        Speed13X = 1 << 8, // Speed 1.3x
        Speed14X = 1 << 9, // Speed 1.4x
        Speed15X = 1 << 10, // Speed 1.5x
        Speed16X = 1 << 11, // Speed 1.6x
        Speed17X = 1 << 12, // Speed 1.7x
        Speed18X = 1 << 13, // Speed 1.8x
        Speed19X = 1 << 14, // Speed 1.9x
        Speed20X = 1 << 15, // Speed 2.0x
        Strict = 1 << 16, // Makes the accuracy hit windows harder
        Chill = 1 << 17, // Makes the accuracy hit windows easier
        NoPause = 1 << 18, // Disallows pausing.
        Autoplay = 1 << 19, // The game automatically plays it.
        Paused = 1 << 20, // The user paused during gameplay.
        NoFail = 1 << 21, // Unable to fail during gameplay.
        NoLongNotes = 1 << 22, // Converts LNs into regular notes.
        Randomize = 1 << 23, // Randomizes the playfield's lanes.
        Speed055X = 1 << 24, // Speed 0.55x,
        Speed065X = 1 << 25, // Speed 0.65x
        Speed075X = 1 << 26, // Speed 0.75x
        Speed085X = 1 << 27, // Speed 0.85x
        Speed095X = 1 << 28, // Speed 0.95x
    }
}
