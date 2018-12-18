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

        public float JackSigmaThresholdMs { get; } = 26;

        public int MaxSimpleJackRepetition { get; } = 30;

        public DynamicVariable StaminaIncrementalMultiplier { get; set; }

        public DynamicVariable StaminaDecrementalMultiplier { get; set; }

        public DynamicVariable WristRepetitionMultiplier { get; set; }

        public DynamicVariable WristTechMultiplier { get; set; }

        public DynamicVariable WristGapMultiplier { get; set; }

        public DynamicVariable WristVibroMultiplier { get; set; }

        public DynamicVariable WristSimpleJackMultiplier { get; set; }

        /// <summary>
        ///     Chord Multiplier if Chorded Hit Object is found in the other hand.
        /// </summary>
        public DynamicVariable ChordMultiplier { get; set; }

        /// <summary>
        ///     Constructor. Create default strain constant values.
        /// </summary>
        public StrainConstantsKeys()
        {
            StaminaIncrementalMultiplier = NewConstant("StaminaIncrementalMultiplier", 0.6317447f);
            StaminaDecrementalMultiplier = NewConstant("StaminaDecrementalMultiplier", 0.1270299f);
            WristRepetitionMultiplier = NewConstant("WristRepetitionMultiplier", 0.9925058f);
            WristTechMultiplier = NewConstant("WristTechMultiplier", 1.245176f);
            WristGapMultiplier = NewConstant("WristGapMultiplier", 1.069906f);
            WristVibroMultiplier = NewConstant("WristVibroMultiplier", 0.918826f);
            WristSimpleJackMultiplier = NewConstant("WristSimpleJackMultiplier", 0.9705446f);
            ChordMultiplier = NewConstant("ChordMultiplier", 0.9115574f);
        }
    }
}
