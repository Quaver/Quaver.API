/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2019 Swan & The Quaver Team <support@quavergame.com>.
*/

using MoonSharp.Interpreter.Interop;

namespace Quaver.API.Maps.Structures
{
    public interface IStartTime
    {
        public float StartTime { get; [MoonSharpVisible(false)] set; }
    }
}
