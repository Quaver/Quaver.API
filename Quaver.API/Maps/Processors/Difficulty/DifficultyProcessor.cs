/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using Quaver.API.Enums;
using Quaver.API.Maps.Processors.Difficulty.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Maps.Processors.Difficulty
{
    /// <summary>
    ///     Handles Difficulty Solving + Data
    /// </summary>
    public abstract class DifficultyProcessor
    {
        /// <summary>
        ///     Current map for difficulty calculation
        /// </summary>
        internal Qua Map { get; set; }

        /// <summary>
        ///     Version of the Difficulty Processor
        /// </summary>
        public static string Version;

        /// <summary>
        ///     Overall Difficulty of a map
        /// </summary>
        public float OverallDifficulty { get; set; }

        /// <summary>
        ///     Used to display prominent patterns of a map in the client
        /// </summary>
        public QssPatternFlags QssPatternFlags { get; set; }

        /// <summary>
        ///     Total ammount of milliseconds in a second.
        /// </summary>
        public const float SECONDS_TO_MILLISECONDS = 1000;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="map"></param>
        public DifficultyProcessor(Qua map, DifficultyConstants constants, ModIdentifier mods = ModIdentifier.None) => Map = map;

        /// <summary>
        ///     Calculate Play Rating according to accuracy.
        /// </summary>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public static float CalculatePlayRating(float difficulty, float accuracy) => difficulty * (float)Math.Pow(accuracy, 3);
        {

        }
    }
}
