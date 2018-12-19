/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

namespace Quaver.API.Replays
{
    public class ReplayKeyPressInfo
    {
        /// <summary>
        ///     The key that was pressed.
        /// </summary>
        public ReplayKeyPressState Key { get; }

        /// <summary>
        ///     The time the key was pressed.
        /// </summary>
        public float TimePressed { get; set; }

        /// <summary>
        ///     The time the key was released.
        /// </summary>
        public float TimeReleased { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="timePressed"></param>
        /// <param name="timeReleased"></param>
        public ReplayKeyPressInfo(ReplayKeyPressState key, float timePressed, float timeReleased = 0)
        {
            Key = key;
            TimePressed = timePressed;
            TimeReleased = timeReleased;
        }
    }
}
