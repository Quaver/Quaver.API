using Quaver.API.Maps.Processors.Difficulty.Optimization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys
{
    public class StrainConstantsKeys : StrainConstants
    {
        public float SJackLowerBoundaryMs { get; private set; }
        public float SJackUpperBoundaryMS { get; private set; }

        public override List<ConstantVariable> ConstantVariables { get; set; }

        public override string GetInfoFromVariables() => base.GetInfoFromVariables();

        public override float NewConstant(string name, float value) => base.NewConstant(name, value);

        public StrainConstantsKeys()
        {
            SJackLowerBoundaryMs = NewConstant("SJackLowerBoundaryMs", 12.2f);
            SJackUpperBoundaryMS = NewConstant("SJackUpperBoundaryMS", 12.2f);
        }
    }
}
