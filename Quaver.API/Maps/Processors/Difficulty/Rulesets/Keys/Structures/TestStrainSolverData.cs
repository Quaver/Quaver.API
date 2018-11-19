using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    public class TestStrainSolverData
    {
        /// <summary>
        /// 
        /// </summary>
        public float ActionLength { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float ActionStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float ActionEnd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FingerAction ActionType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float ActionTechValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<FingerState> ActionSequence { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TestStrainSolverData()
        {

        }
    }
}
