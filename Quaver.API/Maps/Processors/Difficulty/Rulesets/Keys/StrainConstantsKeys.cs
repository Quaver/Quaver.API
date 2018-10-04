using Quaver.API.Maps.Processors.Difficulty.Optimization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys
{
    public class StrainConstantsKeys : StrainConstants
    {
        public float SJackLowerBoundaryMs { get; private set; }
        public float SJackUpperBoundaryMs { get; private set; }
        public float SJackMaxStrainValue { get; private set; }
        public float SJackCurveExponential { get; private set; }

        public float TJackLowerBoundaryMs { get; private set; }
        public float TJackUpperBoundaryMs { get; private set; }
        public float TJackMaxStrainValue { get; private set; }
        public float TJackCurveExponential { get; private set; }

        public float RollLowerBoundaryMs { get; private set; }
        public float RollkUpperBoundaryMs { get; private set; }
        public float RollMaxStrainValue { get; private set; }
        public float RollCurveExponential { get; private set; }

        public float BracketLowerBoundaryMs { get; private set; }
        public float BracketUpperBoundaryMs { get; private set; }
        public float BracketMaxStrainValue { get; private set; }
        public float BracketCurveExponential { get; private set; }

        public override List<ConstantVariable> ConstantVariables { get; set; }

        public override string GetInfoFromVariables() => base.GetInfoFromVariables();

        public override float NewConstant(string name, float value) => base.NewConstant(name, value);

        public StrainConstantsKeys()
        {
            SJackLowerBoundaryMs = NewConstant("SJackLowerBoundaryMs", 30);
            SJackUpperBoundaryMs = NewConstant("SJackUpperBoundaryMs", 280);
            SJackMaxStrainValue = NewConstant("SJackMaxStrainValue", 88);
            SJackCurveExponential = NewConstant("SJackCurveExponential", 1.09f);

            TJackLowerBoundaryMs = NewConstant("TJackLowerBoundaryMs", 20);
            TJackUpperBoundaryMs = NewConstant("TJackUpperBoundaryMs", 280);
            TJackMaxStrainValue = NewConstant("TJackMaxStrainValue", 90);
            TJackCurveExponential = NewConstant("TJackCurveExponential", 1.05f);

            RollLowerBoundaryMs = NewConstant("RollLowerBoundaryMs", 40);
            RollkUpperBoundaryMs = NewConstant("RollkUpperBoundaryMs", 180);
            RollMaxStrainValue = NewConstant("RollMaxStrainValue", 54);
            RollCurveExponential = NewConstant("RollCurveExponential", 0.95f);

            BracketLowerBoundaryMs = NewConstant("BracketLowerBoundaryMs", 40);
            BracketUpperBoundaryMs = NewConstant("BracketUpperBoundaryMs", 180);
            BracketMaxStrainValue = NewConstant("BracketMaxStrainValue", 56);
            BracketCurveExponential = NewConstant("BracketCurveExponential", 0.95f);
    }
    }
}
