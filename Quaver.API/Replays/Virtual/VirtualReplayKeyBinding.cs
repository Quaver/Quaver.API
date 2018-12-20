/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

namespace Quaver.API.Replays.Virtual
{
    public class VirtualReplayKeyBinding
    {
        /// <summary>
        ///     The virtual replay key.
        /// </summary>
        public ReplayKeyPressState Key { get; }

        /// <summary>
        ///     If the key is currently pressed.
        /// </summary>
        public bool Pressed { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        public VirtualReplayKeyBinding(ReplayKeyPressState key) => Key = key;
    }
}
