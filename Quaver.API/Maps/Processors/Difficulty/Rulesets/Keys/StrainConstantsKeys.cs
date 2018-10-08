using Quaver.API.Maps.Processors.Difficulty.Optimization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys
{
    public class StrainConstantsKeys : StrainConstants
    {
        /// <summary>
        ///     When Long Notes start/end after this threshold, it will be considered for a specific multiplier.
        ///     Non-Dyanmic Constant. Do not use for optimization.
        /// </summary>
        public float LnEndThresholdMs { get; set; } = 42;

        /// <summary>
        ///     When seperate notes are under this threshold, it will count as a chord.
        ///     Non-Dyanmic Constant. Do not use for optimization.
        /// </summary>
        public float ChordClumpToleranceMs { get; set; } = 8;

        /// <summary>
        ///     Size of each graph partition in miliseconds.
        ///     Non-Dyanmic Constant. Do not use for optimization.
        /// </summary>
        public int GraphIntervalSizeMs { get; set; } = 500;

        /// <summary>
        ///     Offset between each graph partition in miliseconds.
        ///     Non-Dyanmic Constant. Do not use for optimization.
        /// </summary>
        public int GraphIntervalOffsetMs { get; set; } = 100;

        // Simple Jacks
        public float SJackLowerBoundaryMs { get; private set; }
        public float SJackUpperBoundaryMs { get; private set; }
        public float SJackMaxStrainValue { get; private set; }
        public float SJackCurveExponential { get; private set; }

        // Tech Jacks
        public float TJackLowerBoundaryMs { get; private set; }
        public float TJackUpperBoundaryMs { get; private set; }
        public float TJackMaxStrainValue { get; private set; }
        public float TJackCurveExponential { get; private set; }

        // Rolls
        public float RollLowerBoundaryMs { get; private set; }
        public float RollUpperBoundaryMs { get; private set; }
        public float RollMaxStrainValue { get; private set; }
        public float RollCurveExponential { get; private set; }

        // Brackets
        public float BracketLowerBoundaryMs { get; private set; }
        public float BracketUpperBoundaryMs { get; private set; }
        public float BracketMaxStrainValue { get; private set; }
        public float BracketCurveExponential { get; private set; }

        // LN
        public float LnBaseMultiplier { get; private set; }
        public float LnLayerToleranceMs { get; private set; }
        public float LnLayerThresholdMs { get; private set; }
        public float LnReleaseAfterMultiplier { get; private set; }
        public float LnReleaseBeforeMultiplier { get; private set; }
        public float LnTapMultiplier { get; private set; }

        // LongJack Manipulation
        public float VibroActionDurationMs { get; set; }
        public float VibroActionToleranceMs { get; set; }
        public float VibroMultiplier { get; set; }
        public float VibroLengthMultiplier { get; set; }
        public float VibroMaxLength { get; set; }

        // Roll Manipulation
        public float RollRatioToleranceMs { get; set; }
        public float RollRatioMultiplier { get; set; }
        public float RollLengthMultiplier { get; set; }
        public float RollMaxLength { get; set; }

        /// <summary>
        ///     Constructor. Create default strain constant values.
        /// </summary>
        public StrainConstantsKeys()
        {
            // Simple Jack
            SJackLowerBoundaryMs = NewConstant("SJackLowerBoundaryMs", 40);
            SJackUpperBoundaryMs = NewConstant("SJackUpperBoundaryMs", 330);
            SJackMaxStrainValue = NewConstant("SJackMaxStrainValue", 83);
            SJackCurveExponential = NewConstant("SJackCurveExponential", 1.17f);

            // Tech Jack
            TJackLowerBoundaryMs = NewConstant("TJackLowerBoundaryMs", 40);
            TJackUpperBoundaryMs = NewConstant("TJackUpperBoundaryMs", 340);
            TJackMaxStrainValue = NewConstant("TJackMaxStrainValue", 85);
            TJackCurveExponential = NewConstant("TJackCurveExponential", 1.14f);

            // Roll/Trill
            RollLowerBoundaryMs = NewConstant("RollLowerBoundaryMs", 30);
            RollUpperBoundaryMs = NewConstant("RollUpperBoundaryMs", 230);
            RollMaxStrainValue = NewConstant("RollMaxStrainValue", 56);
            RollCurveExponential = NewConstant("RollCurveExponential", 1.07f);

            // Bracket
            BracketLowerBoundaryMs = NewConstant("BracketLowerBoundaryMs", 30);
            BracketUpperBoundaryMs = NewConstant("BracketUpperBoundaryMs", 230);
            BracketMaxStrainValue = NewConstant("BracketMaxStrainValue", 57);
            BracketCurveExponential = NewConstant("BracketCurveExponential", 1.07f);

            // LN
            LnBaseMultiplier = NewConstant("LnBaseMultiplier", 0.6f);
            LnLayerToleranceMs = NewConstant("LnLayerToleranceMs", 40f);
            LnLayerThresholdMs = NewConstant("LnLayerThresholdMs", 93.7f);
            LnReleaseAfterMultiplier = NewConstant("LnReleaseAfterMultiplier", 1.75f);
            LnReleaseBeforeMultiplier = NewConstant("LnReleaseBeforeMultiplier", 1.35f);
            LnTapMultiplier = NewConstant("LnTapMultiplier", 1.05f);

            // LongJack Manipulation
            VibroActionDurationMs = NewConstant("VibroActionDurationMs", 88.2f);
            VibroActionToleranceMs = NewConstant("VibroActionToleranceMs", 22f);
            VibroMultiplier = NewConstant("VibroMultiplier", 0.48f);
            VibroLengthMultiplier = NewConstant("VibroLengthMultiplier", 0.3f);
            VibroMaxLength = NewConstant("VibroMaxLength", 6);

            // Roll Manipulation
            RollRatioToleranceMs = NewConstant("RollRatioToleranceMs", 2);
            RollRatioMultiplier = NewConstant("RollRatioMultiplier", 0.25f);
            RollLengthMultiplier = NewConstant("RollLengthMultiplier", 0.6f);
            RollMaxLength = NewConstant("RollMaxLength", 14);
        }
    }
}
