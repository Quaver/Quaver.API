/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

namespace Quaver.API.Replays
{
    public class ReplayFrame
    {
        /// <summary>
        ///     The time in the replay since the last frame.
        /// </summary>
        public int Time { get; }

        /// <summary>
        ///     The keys that were pressed during this frame.
        /// </summary>
        public ReplayKeyPressState Keys { get; }

        /// <summary>
        ///     Ctor -
        /// </summary>
        /// <param name="time"></param>
        /// <param name="keys"></param>
        public ReplayFrame(int time, ReplayKeyPressState keys)
        {
            Time = time;
            Keys = keys;
        }

        public override string ToString() => $"{Time}|{(int)Keys}";

        public string ToDebugString() => $"{Time}|{Keys}";
    }
}
