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
    public enum HitSounds
    {
        Normal = 1 << 0, // This is 1, but Normal should be played regardless if it's 0 or 1.
        Whistle = 1 << 1, // 2
        Finish = 1 << 2, // 4
        Clap = 1 << 3 // 8
    }
}
