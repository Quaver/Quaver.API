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
        ///     Is used to interpolate S.Jack Bpm to Difficulty
        /// </summary>
        public Dictionary<float, float> SJackBpmToDifficulty { get; } = new Dictionary<float, float>
        {
            { 50, 1 },
            { 75, 10 },
            { 85, 20 },
            { 90, 25 },
            { 100, 29 },
            { 110, 33 },
            { 120, 35 },
            { 130, 37 },
            { 140, 39 },
            { 150, 41 },
            { 160, 43 },
            { 170, 45 },
            { 180, 47 },
            { 190, 49 },
            { 200, 52 },
            { 220, 54 },
            { 240, 56 },
            { 300, 60 }
        };

        /// <summary>
        ///     Is used to interpolate T.Jack Bpm to Difficulty
        /// </summary>
        public Dictionary<float, float> TJackBpmToDifficulty { get; } = new Dictionary<float, float>
        {
            { 50, 1 },
            { 75, 10 },
            { 85, 20 },
            { 90, 25 },
            { 100, 29 },
            { 110, 33 },
            { 120, 35 },
            { 130, 37 },
            { 140, 39 },
            { 150, 41 },
            { 160, 43 },
            { 170, 45 },
            { 180, 47 },
            { 190, 49 },
            { 200, 52 },
            { 220, 54 },
            { 240, 56 },
            { 300, 60 }
        };

        /// <summary>
        ///     Is used to interpolate Roll Bpm to Difficulty
        /// </summary>
        public Dictionary<float, float> RollBpmToDifficulty { get; } = new Dictionary<float, float>
        {
            { 90, 1 },
            { 140, 10 },
            { 170, 20 },
            { 180, 24 },
            { 190, 26 },
            { 200, 28 },
            { 210, 30 },
            { 220, 32 },
            { 230, 34 },
            { 240, 36 },
            { 250, 38 },
            { 260, 40 },
            { 270, 42 },
            { 280, 44 },
            { 300, 46 },
            { 330, 48 },
            { 350, 50 },
            { 400, 52 },
            { 500, 56 },
            { 600, 60 }
        };

        /// <summary>
        ///     Is used to interpolate Bracket Bpm to Difficulty
        /// </summary>
        public Dictionary<float, float> BracketBpmToDifficulty { get; } = new Dictionary<float, float>
        {
            { 90, 1 },
            { 140, 10 },
            { 170, 20 },
            { 180, 24 },
            { 190, 26 },
            { 200, 28 },
            { 210, 30 },
            { 220, 32 },
            { 230, 34 },
            { 240, 36 },
            { 250, 38 },
            { 260, 40 },
            { 270, 42 },
            { 280, 44 },
            { 300, 46 },
            { 330, 48 },
            { 350, 50 },
            { 400, 52 },
            { 500, 56 },
            { 600, 60 }
        };

        /// <summary>
        ///     Constructor. Create default strain constant values.
        /// </summary>
        public StrainConstantsKeys()
        {
            // TODO: I don't think we need this class. It was originally for optimization,
            // but everything can actually be set manually since the logic is pretty simple

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
