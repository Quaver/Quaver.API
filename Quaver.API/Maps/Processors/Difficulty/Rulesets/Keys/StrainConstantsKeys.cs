using Quaver.API.Maps.Processors.Difficulty.Optimization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys
{
    public class StrainConstantsKeys : StrainConstants
    {
        public float DifficultyMultiplier { get; } = 4.8f;

        public float DifficultyOffset { get; } = -22f;

        public float VibroActionTolerance { get; } = 94;

        public float VibroActionThreshold { get; } = 100;

        public int MaxSimpleJackRepetition { get; } = 30;

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
            StaminaIncrementalMultiplier = NewConstant("StaminaIncrementalMultiplier", 0.8f);
            StaminaDecrementalMultiplier = NewConstant("StaminaDecrementalMultiplier", 0.4f);
            WristRepetitionMultiplier = NewConstant("WristRepetitionMultiplier", 0.97f);
            WristTechMultiplier = NewConstant("WristTechMultiplier", 1.3f);
            WristGapMultiplier = NewConstant("WristGapMultiplier", 1f);
            WristVibroMultiplier = NewConstant("WristVibroMultiplier", 0.91f);
            WristSimpleJackMultiplier = NewConstant("WristSimpleJackMultiplier", 0.98f);
        }
    }
}
