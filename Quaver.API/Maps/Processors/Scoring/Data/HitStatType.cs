/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

namespace Quaver.API.Maps.Processors.Scoring.Data
{
    public enum HitStatType
    {
        Hit, // Input was involved in this HitStatType. (User hit, released, or early missed)
        Miss, // User completely missed the note.
    }
}
