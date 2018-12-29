/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using Quaver.API.Maps;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Replays
{
    /// <summary>
    ///     Used for constructing autoplay replays.
    ///     This is an individual frame in the replay.
    /// </summary>
    public struct ReplayAutoplayFrame
    {
        /// <summary>
        ///     The type of frame, whether press or release.
        /// </summary>
        public ReplayAutoplayFrameType Type { get; }

        /// <summary>
        ///     The time of the song in which this frame occurred.
        /// </summary>
        public int Time { get; }

        /// <summary>
        ///     The keys pressed in this frame.
        /// </summary>
        public ReplayKeyPressState Keys { get; }

        /// <summary>
        ///     The HitObject in this frame.
        /// </summary>
        public HitObjectInfo HitObject { get; }

        /// <summary>
        ///     Ctor -
        /// </summary>
        /// <param name="hitObject"></param>
        /// <param name="type"></param>
        /// <param name="time"></param>
        /// <param name="keys"></param>
        public ReplayAutoplayFrame(HitObjectInfo hitObject, ReplayAutoplayFrameType type, int time, ReplayKeyPressState keys)
        {
            HitObject = hitObject;
            Type = type;
            Time = time;
            Keys = keys;
        }
    }

    /// <summary>
    ///     Enum that allows us to determine which action to take when constructing autoplay replays.
    /// </summary>
    public enum ReplayAutoplayFrameType
    {
        Press,
        Release
    }
}
