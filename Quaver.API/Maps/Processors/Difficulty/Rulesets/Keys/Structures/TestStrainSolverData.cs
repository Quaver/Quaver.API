using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    public class TestStrainSolverData
    {
        public float ActionLength { get; set; }

        public float ActionStart { get; set; }

        public float ActionEnd { get; set; }

        public FingerAction ActionType { get; set; }

        public float ActionTechValue { get; set; }

        public List<FingerState> ActionSequence { get; set; }

        public TestStrainSolverData()
        {

        }
    }
}
