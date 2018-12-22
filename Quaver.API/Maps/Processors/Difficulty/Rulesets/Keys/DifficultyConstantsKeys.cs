/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using Quaver.API.Maps.Processors.Difficulty.Optimization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys
{
    public class DifficultyConstantsKeys : DifficultyConstants
    {
        /// <summary>
        ///     Base Difficulty Multiplier
        /// </summary>
        public double DifficultyMultiplier { get; } = 11.2f;

        /// <summary>
        ///     Base Offset Value. Difficulty will be subtracted by this value.
        ///     - Lowest Difficulty possible in HandState is 1. 
        /// </summary>
        public double DifficultyOffset { get; } = -24f;

        /// <summary>
        ///     Determines if the HitObject on the same hand of a specific HitObject will be counted as a Chorded Hit Object.
        /// </summary>
        public double ChordThresholdSameHandMs { get; } = 8f;

        /// <summary>
        ///     Determines if the HitObject on the other hand of this HitObject will be counted as a Chorded Hit Object.
        /// </summary>
        public double ChordThresholdOtherHandMs { get; } = 16f;

        /// <summary>
        ///     Any two states with intervals below this value will be considered vibro.
        ///     - a 1/4 170bpm jack has an interval of 88.235ms
        /// </summary>
        public double VibroActionToleranceMs { get; } = 88.2f;

        /// <summary>
        ///     Any two states with intervals below this value will fall under the Vibro gradient.
        ///     - a 1/4 150bpm jack has an interval of 100ms
        /// </summary>
        public double VibroActionThresholdMs { get; } = 100;

        /// <summary>
        /// Max Absolute Delta between two Simple Jack Actions to be considered as repetition.
        /// </summary>
        public double WristGapDeltaThresholdMs { get; } = 26;

        /// <summary>
        ///     Max Repetition for Simple Jack. Will not count simple jacks that repeat after this.
        /// </summary>
        public int MaxSimpleJackRepetition { get; } = 30;

        /// <summary>
        ///     Max Delta between two objects for stamina calculation.
        /// </summary>
        public double MaxStaminaDelta { get; } = 1000;

        /// <summary>
        ///     Determines the Multiplier for 1/4th Beat Action Length
        /// </summary>
        public double BpmToActionLengthMs { get; } = 15000;

        /// <summary>
        ///     Spline Multiplier for Hand States that have higher Difficulty than the previous predecessor 
        /// </summary>
        public DynamicVariable StaminaIncrementalMultiplier { get; set; }

        /// <summary>
        ///     Spline Multiplier for Hand States that have lower Difficulty than the previous predecessor 
        /// </summary>
        public DynamicVariable StaminaDecrementalMultiplier { get; set; }

        /// <summary>
        ///     Difficulty Multiplier for Anchory actions
        ///     - Anchors are determined when wrist-up is not optimally comfortable
        /// </summary>
        public DynamicVariable WristAnchorMultiplier { get; set; }

        /// <summary>
        ///     Difficulty Multiplier for Technical Jacks.
        ///     - Technical Jacks are Jacks that have different Hand States in succession.
        /// </summary>
        public DynamicVariable WristTechMultiplier { get; set; }

        /// <summary>
        ///     Difficulty Multiplier for Simple Jacks.
        ///     - Simple Jacks are Jacks that repeat same Hand State in succession.
        /// </summary>
        public DynamicVariable WristSimpleJackMultiplier { get; set; }

        /// <summary>
        ///     Difficulty Multiplier that applies exponentially to Simple Jacks that repeat.
        /// </summary>
        public DynamicVariable WristRepetitionMultiplier { get; set; }

        /// <summary>
        ///     Difficulty Multiplier for fast Simple Jacks.
        ///     - Speed is determined by VibroActionToleranceMs.
        /// </summary>
        public DynamicVariable WristVibroMultiplier { get; set; }

        /// <summary>
        ///     Difficulty Multiplier for Gaps in Simple Jacks.
        ///     - Appropriate gaps are determined by WristGapSigmaThresholdMs
        /// </summary>
        public DynamicVariable WristGapMultiplier { get; set; }

        /// <summary>
        ///     Difficulty Multiplier for Two Handed Chords.
        /// </summary>
        public DynamicVariable TwoHandedChordMultiplier { get; set; }

        /// <summary>
        ///     Constructor. Create default strain constant values.
        /// </summary>
        public DifficultyConstantsKeys()
        {
            StaminaIncrementalMultiplier = NewConstant("StaminaIncrementalMultiplier", 0.1607309f);
            StaminaDecrementalMultiplier = NewConstant("StaminaDecrementalMultiplier", 0.6409109f);
            WristRepetitionMultiplier = NewConstant("WristRepetitionMultiplier", 0.9741689f);
            WristAnchorMultiplier = NewConstant("WristAnchorMultiplier", 1.14635f);
            WristTechMultiplier = NewConstant("WristTechMultiplier", 1.053561f);
            WristGapMultiplier = NewConstant("WristGapMultiplier", 1.079208f);
            WristVibroMultiplier = NewConstant("WristVibroMultiplier", 0.9466178f);
            WristSimpleJackMultiplier = NewConstant("WristSimpleJackMultiplier", 0.974332f);
            TwoHandedChordMultiplier = NewConstant("TwoHandedChordMultiplier", 0.949433f);
        }
    }
}
