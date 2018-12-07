using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    public class TestStrainSolverData
    {
        /// <summary>
        ///     Determined by whether this object needs to have its sequence and hitobjects solved/resolved.
        ///     Mainly will be used by the editor for dynamic difficulty calculation.
        /// </summary>
        public bool DifficultySolved { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float ActionLength => ActionEnd - ActionStart;

        /// <summary>
        /// 
        /// </summary>
        public float ActionStart { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public float ActionEnd { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FingerAction ActionType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public float ActionTechValue { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<HandStateData> ActionSequence { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public TestStrainSolverData(List<HandStateData> sequence)
        {
            ActionSequence = sequence;

            // todo: implement stuff
            EvaluateDifficulty();
        }

        public void EvaluateDifficulty()
        {
            //EvaluateDifficulty
        }
    }
}
