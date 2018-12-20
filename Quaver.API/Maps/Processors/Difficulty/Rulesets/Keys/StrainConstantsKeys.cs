using Quaver.API.Maps.Processors.Difficulty.Optimization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys
{
    public class StrainConstantsKeys : DifficultyConstants
    {
        public float DifficultyMultiplier { get; } = 4.5f;

        public float DifficultyOffset { get; } = -22f;

        public float VibroActionTolerance { get; } = 94;

        public float VibroActionThreshold { get; } = 100;

        public DynamicVariable StaminaIncrementalMultiplier { get; set; }

        public DynamicVariable StaminaDecrementalMultiplier { get; set; }

        public DynamicVariable WristRepetitionMultiplier { get; set; }

        public DynamicVariable WristTechMultiplier { get; set; }

        public DynamicVariable WristGapMultiplier { get; set; }

        public DynamicVariable WristVibroMultiplier { get; set; }

        public DynamicVariable WristSimpleJackMultiplier { get; set; }

        /// <summary>
        ///     Constructor. Create default strain constant values.
        /// </summary>
        public StrainConstantsKeys()
        {
            StaminaIncrementalMultiplier = NewConstant("StaminaIncrementalMultiplier", 0.5818259f);
            StaminaDecrementalMultiplier = NewConstant("StaminaDecrementalMultiplier", 0.3847841f);
            WristRepetitionMultiplier = NewConstant("WristRepetitionMultiplier", 0.9904839f);
            WristTechMultiplier = NewConstant("WristTechMultiplier", 1.179059f);
            WristGapMultiplier = NewConstant("WristGapMultiplier", 1.103457f);
            WristVibroMultiplier = NewConstant("WristVibroMultiplier", 0.9440251f);
            WristSimpleJackMultiplier = NewConstant("WristSimpleJackMultiplier", 1.05868f);
        }
    }
}
