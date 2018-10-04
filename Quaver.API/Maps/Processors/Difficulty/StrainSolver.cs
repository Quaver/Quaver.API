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
    public abstract class StrainSolver
    {
        /// <summary>
        ///     Current map for difficulty calculation
        /// </summary>
        internal Qua Map { get; set; }

        /// <summary>
        ///     Overall Difficulty of a map
        /// </summary>
        public abstract float OverallDifficulty { get; internal set; }

        /// <summary>
        ///     Constants used for solving
        /// </summary>
        public abstract StrainConstants StrainConstants { get; internal set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="map"></param>
        public StrainSolver(Qua map, ModIdentifier mods = ModIdentifier.None) => Map = map;
    }
}
