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

        public float StaminaIncrementalMultiplier { get; set; }

        public float StaminaDecrementalMultiplier { get; set; }

        public float WristRepetitionMultiplier { get; set; }

        /// <summary>
        ///     Constructor. Create default strain constant values.
        /// </summary>
        public StrainConstantsKeys()
        {
            StaminaIncrementalMultiplier = NewConstant("StaminaIncrementalMultiplier", 0.75f);
            StaminaDecrementalMultiplier = NewConstant("StaminaDecrementalMultiplier", 0.25f);
            WristRepetitionMultiplier = NewConstant("WristRepetitionMultiplier", 0.992f);
        }
    }
}
