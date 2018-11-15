using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    public class ActionData
    {
        /// <summary>
        ///     HitObjects that this data set references
        /// </summary>
        public List<StrainSolverHitObject> HitObjects { get; set; }

        /// <summary>
        ///     Current Action performed
        /// </summary>
        public FingerAction FingerAction { get; set; } = FingerAction.None;

        /// <summary>
        ///     Duration of this data set's action
        /// </summary>
        public float ActionDuration { get; set; }

        /// <summary>
        ///     Tech ratio
        /// </summary>
        public float ActionTechRatio { get; set; }
        //public float ActionDifficulty { get; set; }
    }
}
