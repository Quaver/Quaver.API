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

        public string Version { get; }

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
        public DifficultyProcessor(Qua map, StrainConstants constants, ModIdentifier mods = ModIdentifier.None) => Map = map;
    }
}
