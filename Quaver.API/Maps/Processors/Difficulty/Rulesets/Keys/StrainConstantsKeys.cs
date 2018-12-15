using Quaver.API.Maps.Processors.Difficulty.Optimization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys
{
    public class StrainConstantsKeys : StrainConstants
    {
        public float DifficultyMultiplier { get; } = 4.75f;

        public float DifficultyOffset { get; } = -22f;

        public float VibroActionTolerance { get; } = 94;

        public float VibroActionThreshold { get; } = 100;

        public ConstantVariable StaminaIncrementalMultiplier { get; set; }

        public ConstantVariable StaminaDecrementalMultiplier { get; set; }

        public ConstantVariable WristRepetitionMultiplier { get; set; }

        public ConstantVariable WristTechMultiplier { get; set; }

        public ConstantVariable WristGapMultiplier { get; set; }

        public ConstantVariable WristVibroMultiplier { get; set; }

        public ConstantVariable WristSimpleJackMultiplier { get; set; }

        /// <summary>
        ///     Constructor. Create default strain constant values.
        /// </summary>
        public StrainConstantsKeys()
        {
            StaminaIncrementalMultiplier = NewConstant("StaminaIncrementalMultiplier", 0.75f);
            StaminaDecrementalMultiplier = NewConstant("StaminaDecrementalMultiplier", 0.025f);
            WristRepetitionMultiplier = NewConstant("WristRepetitionMultiplier", 0.992f);
            WristTechMultiplier = NewConstant("WristTechMultiplier", 1.35f);
            WristGapMultiplier = NewConstant("WristGapMultiplier", 1f);
            WristVibroMultiplier = NewConstant("WristVibroMultiplier", 0.88f);
            WristSimpleJackMultiplier = NewConstant("WristSimpleJackMultiplier", 0.97f);
        }
    }
}
