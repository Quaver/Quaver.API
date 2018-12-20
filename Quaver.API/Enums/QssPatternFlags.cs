/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Enums
{
    /// <summary>
    ///     Used to display prominent patterns of a map in the client
    /// </summary>
    [Flags]
    public enum QssPatternFlags
    {
        Unknown = 0,
        MiniJack = 1 << 0,
        ChordJack = 1 << 1,
        KoreaJack = 1 << 2,
        LongJack = 1 << 3,
        QuadJack = 1 << 4,
        Rolls = 1 << 5,
        LightStream = 1 << 6,
        JumpStream = 1 << 7,
        HandStream = 1 << 8,
        QuadStream = 1 << 9,
        InverseLN = 1 << 10,
        ReleaseLN = 1 << 11,
        Polyrhythm = 1 << 12,
        JumpTrill = 1 << 13,
        SplitTrill = 1 << 14,
        SimpleVibro = 1 << 15,
        ControlVibro = 1 << 16
    }
}
