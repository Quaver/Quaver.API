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
        ///     Constructor
        /// </summary>
        /// <param name="map"></param>
        public StrainSolver(Qua map, float rate = 1) => Map = map;
    }
}
