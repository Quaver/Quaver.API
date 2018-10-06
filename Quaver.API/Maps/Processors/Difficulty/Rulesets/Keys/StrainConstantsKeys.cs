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
        /// </summary>
        public const float THRESHOLD_LN_END_MS = 42;

        /// <summary>
        ///     When seperate notes are under this threshold, it will count as a chord.
        /// </summary>
        public const float THRESHOLD_CHORD_CHECK_MS = 8;

        /// <summary>
        ///     Size of each graph partition in miliseconds.
        /// </summary>
        public const int GRAPH_INTERVAL_SIZE_MS = 500;

        /// <summary>
        ///     Offset between each graph partition in miliseconds.
        /// </summary>
        public const int GRAPH_INTERVAL_OFFSET_MS = 100;

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

        /// <summary>
        ///     Constructor. Create default strain constant values.
        /// </summary>
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
            RollUpperBoundaryMs = NewConstant("RollUpperBoundaryMs", 180);
            RollMaxStrainValue = NewConstant("RollMaxStrainValue", 54);
            RollCurveExponential = NewConstant("RollCurveExponential", 0.95f);

            BracketLowerBoundaryMs = NewConstant("BracketLowerBoundaryMs", 40);
            BracketUpperBoundaryMs = NewConstant("BracketUpperBoundaryMs", 180);
            BracketMaxStrainValue = NewConstant("BracketMaxStrainValue", 56);
            BracketCurveExponential = NewConstant("BracketCurveExponential", 0.95f);
    }
    }
}
