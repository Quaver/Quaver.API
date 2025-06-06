/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.API.Maps;
using Quaver.API.Maps.Processors.Difficulty.Optimization;
using Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys
{
    /// <summary>
    ///     Will be used to solve Strain Rating.
    /// </summary>
    public class DifficultyProcessorKeys : DifficultyProcessor
    {
        /// <summary>
        ///     The version of the processor.
        /// </summary>
        public static string Version { get; } = "0.0.5";

        /// <summary>
        ///     Constants used for solving
        /// </summary>
        public StrainConstantsKeys StrainConstants { get; private set; }

        /// <summary>
        ///     Average note density of the map
        /// </summary>
        public float AverageNoteDensity { get; private set; } = 0;

        /// <summary>
        ///     Hit objects in the map used for solving difficulty
        /// </summary>
        public List<StrainSolverData> StrainSolverData { get; private set; } = new List<StrainSolverData>();

        /// <summary>
        ///     Assumes that the assigned hand will be the one to press that key
        /// </summary>
        private static Hand LaneToHand(int lane, int keyCount)
        {
            if (lane < 1 || lane > keyCount)
                throw new ArgumentOutOfRangeException(nameof(lane), "Lane must be between 1 and keyCount");

            var half = keyCount / 2;

            if (keyCount % 2 == 0)
            {
                if (lane <= half)
                    return Hand.Left;
                else
                    return Hand.Right;
            }
            else
            {
                if (lane <= half)
                    return Hand.Left;

                if (lane == half + 1)
                    return Hand.Ambiguous;
                else
                    return Hand.Right;
            }
        }

        /// <summary>
        ///     Assumes that the assigned finger will be the one to press that key.
        /// </summary>
        private static FingerState LaneToFinger(int lane, int keyCount)
        {
            if (lane < 1 || lane > keyCount)
                throw new ArgumentOutOfRangeException(nameof(lane), "Lane must be between 1 and keyCount");

            var half = keyCount / 2;

            if (keyCount <= 9)
            {
                // even key count
                if (keyCount % 2 == 0)
                {
                    if (lane <= half)
                        return (FingerState)(1 << (half - lane));
                    else
                        return (FingerState)(1 << (lane - (half + 1)));
                }

                // odd key count
                if (lane <= half)
                    return (FingerState)(1 << (half - lane));
                if (lane == half + 1)
                    return FingerState.Thumb;
                return (FingerState)(1 << (lane - (half + 2)));
            }

            if (keyCount == 10)
            {
                if (lane <= half - 1)
                    return (FingerState)(1 << (half - 1 - lane));
                if (lane == half || lane == half + 1)
                    return FingerState.Thumb;
                return (FingerState)(1 << (lane - (half + 2)));
            }

            throw new ArgumentOutOfRangeException(nameof(keyCount), "Key count must be between 1 and 10");
        }

        /// <summary>
        ///     Value of confidence that there's vibro manipulation in the calculated map.
        /// </summary>
        private float VibroInaccuracyConfidence { get; set; }

        /// <summary>
        ///     Value of confidence that there's roll manipulation in the calculated map.
        /// </summary>
        private float RollInaccuracyConfidence { get; set; }

        /// <summary>
        ///     Solves the difficulty of a .qua file
        /// </summary>
        /// <param name="map"></param>
        /// <param name="constants"></param>
        /// <param name="mods"></param>
        /// <param name="detailedSolve"></param>
        public DifficultyProcessorKeys(Qua map, StrainConstants constants, ModIdentifier mods = ModIdentifier.None,
            bool detailedSolve = false) : base(map, constants, mods)
        {
            // Cast the current Strain Constants Property to the correct type.
            StrainConstants = (StrainConstantsKeys)constants;

            // Don't bother calculating map difficulty if there's less than 2 hit objects
            if (map.HitObjects.Count < 2)
                return;

            // Solve for difficulty
            CalculateDifficulty(mods);

            // If detailed solving is enabled, expand calculation
            if (detailedSolve)
            {
                // ComputeNoteDensityData();
                ComputeForPatternFlags();
            }
        }

        /// <summary>
        ///     Calculate difficulty of a map with given rate
        /// </summary>
        /// <param name="rate"></param>
        public void CalculateDifficulty(ModIdentifier mods)
        {
            // If map does not exist, ignore calculation.
            if (Map == null) return;

            // Get song rate from selected mods
            var rate = ModHelper.GetRateFromMods(mods);

            // Compute for overall difficulty
            if (ModeHelper.ToKeyCount(Map.Mode) % 2 == 0)
                OverallDifficulty = ComputeForOverallDifficulty(rate);
            else
                OverallDifficulty = (ComputeForOverallDifficulty(rate, Hand.Left) + ComputeForOverallDifficulty(rate, Hand.Right)) / 2;
        }

        /// <summary>
        ///     Calculate overall difficulty of a map. "AssumeHand" is used for odd-numbered keymodes.
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="assumeHand"></param>
        /// <returns></returns>
        private float ComputeForOverallDifficulty(float rate, Hand assumeHand = Hand.Right)
        {
            ComputeNoteDensityData(rate);
            ComputeBaseStrainStates(rate, assumeHand);
            ComputeForChords();
            ComputeForFingerActions();
            // todo: use ComputeForActionPatterns();
            ComputeForRollManipulation();
            ComputeForJackManipulation();
            ComputeForLnMultiplier();
            return CalculateOverallDifficulty();
        }

        /// <summary>
        ///     Get Note Data, and compute the base strain weights
        ///     The base strain weights are affected by LN layering
        /// </summary>
        /// <param name="qssData"></param>
        /// <param name="qua"></param>
        /// <param name="assumeHand"></param>
        private void ComputeBaseStrainStates(float rate, Hand assumeHand)
        {
            // Add hit objects from qua map to qssData
            for (var i = 0; i < Map.HitObjects.Count; i++)
            {
                if (Map.HasScratchKey && Map.HitObjects[i].Lane == Map.GetKeyCount())
                    continue;

                var curHitOb = new StrainSolverHitObject(Map.HitObjects[i]);
                var curStrainData = new StrainSolverData(curHitOb, rate);

                // Assign Finger and Hand States
                var keyCount = ModeHelper.ToKeyCount(Map.Mode);

                curHitOb.FingerState = LaneToFinger(Map.HitObjects[i].Lane, keyCount);

                var hand = LaneToHand(Map.HitObjects[i].Lane, keyCount);
                curStrainData.Hand = hand == Hand.Ambiguous ? assumeHand : hand;

                // Add Strain Solver Data to list
                StrainSolverData.Add(curStrainData);
            }
        }

        /// <summary>
        ///     Iterate through the HitObject list and merges the chords together into one data point
        /// </summary>
        private void ComputeForChords()
        {
            // Search through whole hit object list and find chords
            for (var i = 0; i < StrainSolverData.Count - 1; i++)
            {
                for (var j = i + 1; j < StrainSolverData.Count; j++)
                {
                    // Check if next hit object is way past the tolerance
                    var msDiff = StrainSolverData[j].StartTime - StrainSolverData[i].StartTime;
                    if (msDiff > StrainConstants.ChordClumpToleranceMs)
                        break;

                    // Check if the next and current hit objects are chord-able
                    if (Math.Abs(msDiff) <= StrainConstants.ChordClumpToleranceMs)
                    {
                        if (StrainSolverData[i].Hand == StrainSolverData[j].Hand)
                        {
                            // Search through every hit object for chords
                            foreach (var k in StrainSolverData[j].HitObjects)
                            {
                                // Check if the current data point will have duplicate finger state to prevent stacked notes
                                var sameStateFound = false;
                                foreach (var l in StrainSolverData[i].HitObjects)
                                {
                                    if (l.FingerState == k.FingerState)
                                    {
                                        sameStateFound = true;
                                    }
                                }

                                // Add hit object to chord list if its not stacked
                                if (!sameStateFound)
                                    StrainSolverData[i].HitObjects.Add(k);
                            }

                            // Remove un-needed data point because it has been merged with the current point
                            StrainSolverData.RemoveAt(j);
                        }
                    }
                }
            }

            // Solve finger state of every object once chords have been found and applied.
            for (var i = 0; i < StrainSolverData.Count; i++)
            {
                StrainSolverData[i].SolveFingerState();
            }
        }

        /// <summary>
        ///     Scans every finger state, and determines its action (JACK/TRILL/TECH, ect).
        ///     Action-Strain multiplier is applied in computation.
        /// </summary>
        /// <param name="qssData"></param>
        private void ComputeForFingerActions()
        {
            // Solve for Finger Action
            for (var i = 0; i < StrainSolverData.Count - 1; i++)
            {
                var curHitOb = StrainSolverData[i];

                // Find the next Hit Object in the current Hit Object's Hand
                for (var j = i + 1; j < StrainSolverData.Count; j++)
                {
                    var nextHitOb = StrainSolverData[j];
                    if (curHitOb.Hand == nextHitOb.Hand && nextHitOb.StartTime > curHitOb.StartTime)
                    {
                        // Determined by if there's a minijack within 2 set of chords/single notes
                        var actionJackFound = (curHitOb.FingerState & nextHitOb.FingerState) != 0;

                        // Determined by if a chord is found in either finger state
                        var actionChordFound = curHitOb.HandChord || nextHitOb.HandChord;

                        // Determined by if both fingerstates are exactly the same
                        var actionSameState = curHitOb.FingerState == nextHitOb.FingerState;

                        // Determined by how long the current finger action is
                        var actionDuration = nextHitOb.StartTime - curHitOb.StartTime;

                        // Apply the "NextStrainSolverDataOnCurrentHand" value on the current hit object and also apply action duration.
                        curHitOb.NextStrainSolverDataOnCurrentHand = nextHitOb;
                        curHitOb.FingerActionDurationMs = actionDuration;

                        // Trill/Roll
                        if (!actionChordFound && !actionSameState)
                        {
                            curHitOb.FingerAction = FingerAction.Roll;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration,
                                StrainConstants.RollLowerBoundaryMs,
                                StrainConstants.RollUpperBoundaryMs,
                                StrainConstants.RollMaxStrainValue,
                                StrainConstants.RollCurveExponential);
                        }

                        // Simple Jack
                        else if (actionSameState)
                        {
                            curHitOb.FingerAction = FingerAction.SimpleJack;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration,
                                StrainConstants.SJackLowerBoundaryMs,
                                StrainConstants.SJackUpperBoundaryMs,
                                StrainConstants.SJackMaxStrainValue,
                                StrainConstants.SJackCurveExponential);
                        }

                        // Tech Jack
                        else if (actionJackFound)
                        {
                            curHitOb.FingerAction = FingerAction.TechnicalJack;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration,
                                StrainConstants.TJackLowerBoundaryMs,
                                StrainConstants.TJackUpperBoundaryMs,
                                StrainConstants.TJackMaxStrainValue,
                                StrainConstants.TJackCurveExponential);
                        }

                        // Bracket
                        else
                        {
                            curHitOb.FingerAction = FingerAction.Bracket;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration,
                                StrainConstants.BracketLowerBoundaryMs,
                                StrainConstants.BracketUpperBoundaryMs,
                                StrainConstants.BracketMaxStrainValue,
                                StrainConstants.BracketCurveExponential);
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Scans every finger action and compute a pattern multiplier.
        ///     Pattern manipulation, and inflated patterns are factored into calculation.
        /// </summary>
        /// <param name="qssData"></param>
        private void ComputeForActionPatterns()
        {
        }

        /// <summary>
        ///     Scans for roll manipulation. "Roll Manipulation" is definced as notes in sequence
        ///     "A -> B -> A" with one action at least twice as long as the other.
        /// </summary>
        private void ComputeForRollManipulation()
        {
            var manipulationIndex = 0;

            foreach (var data in StrainSolverData)
            {
                // Reset manipulation found
                var manipulationFound = false;

                // Check to see if the current data point has two other following points
                if (data.NextStrainSolverDataOnCurrentHand != null &&
                    data.NextStrainSolverDataOnCurrentHand.NextStrainSolverDataOnCurrentHand != null)
                {
                    var middle = data.NextStrainSolverDataOnCurrentHand;
                    var last = data.NextStrainSolverDataOnCurrentHand.NextStrainSolverDataOnCurrentHand;

                    if (data.FingerAction == FingerAction.Roll && middle.FingerAction == FingerAction.Roll)
                    {
                        if (data.FingerState == last.FingerState)
                        {
                            // Get action duration ratio from both actions
                            var durationRatio = Math.Max(data.FingerActionDurationMs / middle.FingerActionDurationMs, middle.FingerActionDurationMs / data.FingerActionDurationMs);

                            // If the ratio is above this threshold, count it as a roll manipulation
                            if (durationRatio >= StrainConstants.RollRatioToleranceMs)
                            {
                                // Apply multiplier
                                // todo: catch possible arithmetic error (division by 0)
                                var durationMultiplier = 1 / (1 + (durationRatio - 1) * StrainConstants.RollRatioMultiplier);

                                var manipulationFoundRatio = 1 - manipulationIndex / StrainConstants.RollMaxLength * (1 - StrainConstants.RollLengthMultiplier);
                                data.RollManipulationStrainMultiplier = durationMultiplier * manipulationFoundRatio;

                                // Count manipulation
                                manipulationFound = true;
                                RollInaccuracyConfidence++;

                                if (manipulationIndex < StrainConstants.RollMaxLength)
                                    manipulationIndex++;
                            }
                        }
                    }
                }

                // subtract manipulation index if manipulation was not found
                if (!manipulationFound && manipulationIndex > 0)
                    manipulationIndex--;
            }
        }

        /// <summary>
        ///     Scans for jack manipulation. "Jack Manipulation" is defined as a succession of simple jacks. ("A -> A -> A")
        /// </summary>
        private void ComputeForJackManipulation()
        {
            var longJackSize = 0;

            foreach (var data in StrainSolverData)
            {
                // Reset manipulation found
                var manipulationFound = false;

                // Check to see if the current data point has a following data point
                if (data.NextStrainSolverDataOnCurrentHand != null)
                {
                    var next = data.NextStrainSolverDataOnCurrentHand;

                    if (data.FingerAction == FingerAction.SimpleJack && next.FingerAction == FingerAction.SimpleJack)
                    {
                        // Apply multiplier
                        // todo: catch possible arithmetic error (division by 0)
                        // note:    83.3ms = 180bpm 1/4 vibro
                        //          88.2ms = 170bpm 1/4 vibro
                        //          93.7ms = 160bpm 1/4 vibro

                        // 35f = 35ms tolerance before hitting vibro point (88.2ms, 170bpm vibro)
                        var durationValue = Math.Min(1, Math.Max(0, (StrainConstants.VibroActionDurationMs + StrainConstants.VibroActionToleranceMs - data.FingerActionDurationMs) / StrainConstants.VibroActionToleranceMs));

                        var durationMultiplier = 1 - durationValue * (1 - StrainConstants.VibroMultiplier);
                        var manipulationFoundRatio = 1 - longJackSize / StrainConstants.VibroMaxLength * (1 - StrainConstants.VibroLengthMultiplier);
                        data.RollManipulationStrainMultiplier = durationMultiplier * manipulationFoundRatio;

                        // Count manipulation
                        manipulationFound = true;
                        VibroInaccuracyConfidence++;

                        if (longJackSize < StrainConstants.VibroMaxLength)
                            longJackSize++;
                    }
                }

                // Reset manipulation count if manipulation was not found
                if (!manipulationFound)
                    longJackSize = 0;
            }
        }

        /// <summary>
        ///     Scans for LN layering and applies a multiplier
        /// </summary>
        private void ComputeForLnMultiplier()
        {
            foreach (var data in StrainSolverData)
            {
                // Check if data is LN
                if (data.EndTime > data.StartTime)
                {
                    var durationValue = 1 - Math.Min(1, Math.Max(0, (StrainConstants.LnLayerThresholdMs + StrainConstants.LnLayerToleranceMs - (data.EndTime - data.StartTime)) / StrainConstants.LnLayerToleranceMs));
                    var baseMultiplier = 1 + durationValue * StrainConstants.LnBaseMultiplier;

                    foreach (var k in data.HitObjects)
                        k.LnStrainMultiplier = baseMultiplier;

                    // Check if next data point exists on current hand
                    var next = data.NextStrainSolverDataOnCurrentHand;

                    if (next != null)
                    {
                        // Check to see if the target hitobject is layered inside the current LN
                        if (next.StartTime < data.EndTime - StrainConstants.LnEndThresholdMs)
                        {
                            if (next.StartTime >= data.StartTime + StrainConstants.LnEndThresholdMs)
                            {
                                // Target hitobject's LN ends after current hitobject's LN end.
                                if (next.EndTime > data.EndTime + StrainConstants.LnEndThresholdMs)
                                {
                                    foreach (var k in data.HitObjects)
                                    {
                                        k.LnLayerType = LnLayerType.OutsideRelease;
                                        k.LnStrainMultiplier *= StrainConstants.LnReleaseAfterMultiplier;
                                    }
                                }

                                // Target hitobject's LN ends before current hitobject's LN end
                                else if (next.EndTime > 0)
                                {
                                    foreach (var k in data.HitObjects)
                                    {
                                        k.LnLayerType = LnLayerType.InsideRelease;
                                        k.LnStrainMultiplier *= StrainConstants.LnReleaseBeforeMultiplier;
                                    }
                                }

                                // Target hitobject is not an LN
                                else
                                {
                                    foreach (var k in data.HitObjects)
                                    {
                                        k.LnLayerType = LnLayerType.InsideTap;
                                        k.LnStrainMultiplier *= StrainConstants.LnTapMultiplier;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Checks to see if the map rating is inacurrate due to vibro/rolls
        /// </summary>
        private void ComputeForPatternFlags()
        {
            // If 10% or more of the map has longjack manip, flag it as vibro map
            if (VibroInaccuracyConfidence / StrainSolverData.Count > 0.10)
                QssPatternFlags |= QssPatternFlags.SimpleVibro;

            // If 15% or more of the map has roll manip, flag it as roll map
            if (RollInaccuracyConfidence / StrainSolverData.Count > 0.15)
                QssPatternFlags |= QssPatternFlags.Rolls;
        }

        /// <summary>
        ///     Calculate the general difficulty of the given map
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        private float CalculateOverallDifficulty()
        {
            // When the map has only scratch key notes, StrainSolverData would be empty, so we return 0
            if (StrainSolverData.Count == 0)
                return 0;

            float calculatedDiff;

            // Solve strain value of every data point
            foreach (var data in StrainSolverData)
                data.CalculateStrainValue();

            calculatedDiff = StrainSolverData.Where(s => s.Hand == Hand.Left || s.Hand == Hand.Right)
                .Average(s => s.TotalStrainValue);

            var bins = new List<float>();
            const int binSize = 1000;

            var mapStart = StrainSolverData.Min(s => s.StartTime);
            var mapEnd = StrainSolverData.Max(s => Math.Max(s.StartTime, s.EndTime));

            var leftIndex = 0;
            var rightIndex = 0;
            var useFallback = Map.GetKeyCount(false) % 2 == 1;
            while (leftIndex < StrainSolverData.Count && StrainSolverData[leftIndex].StartTime < mapStart)
                leftIndex++;
            for (var i = mapStart; i < mapEnd; i += binSize)
            {
                List<StrainSolverData> valuesInBin;
                if (useFallback)
                {
                    valuesInBin = StrainSolverData.Where(s => s.StartTime >= i && s.StartTime < i + binSize)
                        .ToList();
                }
                else
                {
                    while (rightIndex < StrainSolverData.Count - 1 && StrainSolverData[rightIndex + 1].StartTime < i + binSize)
                        rightIndex++;

                    if (leftIndex >= StrainSolverData.Count)
                    {
                        bins.Add(0);
                        continue;
                    }

                    valuesInBin = StrainSolverData.GetRange(leftIndex, rightIndex - leftIndex + 1);
                }

                var averageRating = valuesInBin.Count > 0 ? valuesInBin.Average(s => s.TotalStrainValue) : 0;

                bins.Add(averageRating);
                leftIndex = rightIndex + 1;
            }

            if (!bins.Any(strain => strain > 0)) return 0;

            /*
             * Having a section where notes are relatively easy, compared to the hardest sections of the map, drops the rating way more than it should.
             *
             * This part computes a "continuity" value, the higher it is the less "easy" notes it has.
             * Maps with a higher continuity should get nerfed, since they are overrated right now.
             *
             * The continuity is computed by taking a comparison value, set as the average of the hardest 40% of all sections.
             * All sections are compared to that value, the average is the continuity.
             * A square root is used to decrease the gap between easy sections and hard sections and considered the core of this adjustment.
             */

            // Average of the hardest 40% of the map
            var cutoffPos = (int)Math.Floor(bins.Count * 0.4);
            var top40 = bins.OrderByDescending(s => s).Take(cutoffPos);
            var enumerable = top40 as float[] ?? top40.ToArray();
            var easyRatingCutoff = enumerable.Any() ? enumerable.Average() : 0;

            // We do not consider sections without notes, since there are no "easy notes". Those sections have barely
            // affected the rating in the old difficulty calculator.
            var continuity = (float)bins.Where(strain => strain > 0)
                .Average(strain => Math.Sqrt(strain / easyRatingCutoff));

            // The average continuity of maps in our dataset has been observed to be around 0.85, so we take that
            // as the reference value to keep the rating the same.
            const float maxContinuity = 1.00f;
            const float avgContinuity = 0.85f;
            const float minContinuity = 0.60f;

            const float maxAdjustment = 1.05f;
            const float avgAdjustment = 1.00f;
            const float minAdjustment = 0.90f;

            float continuityAdjustment;

            if (continuity > avgContinuity)
            {
                var continuityFactor = 1 - (continuity - avgContinuity) / (maxContinuity - avgContinuity);
                continuityAdjustment = Math.Min(avgAdjustment, Math.Max(minAdjustment, continuityFactor * (avgAdjustment - minAdjustment) + minAdjustment
                ));
            }
            else
            {
                var continuityFactor = 1 - (continuity - minContinuity) / (avgContinuity - minContinuity);
                continuityAdjustment = Math.Min(maxAdjustment, Math.Max(avgAdjustment, continuityFactor * (maxAdjustment - avgAdjustment) + avgAdjustment));
            }

            calculatedDiff *= continuityAdjustment;

            /*
             * Nerf short maps below 60 seconds of true drain time (time where a player is actually playing hard/moderate parts)
             * We derive the "true drain time" from the continuity.
             * There is no need to measure map length by taking the first/last note or strain data, since the bin size is already known.
             */
            const float maxShortMapAdjustment = 0.75f;
            const float shortMapThreshold = 60 * SECONDS_TO_MILLISECONDS; // Start applying the nerf once a map is shorter

            var trueDrainTime = bins.Count * continuity * binSize;
            var shortMapAdjustment = (float)Math.Min(1, Math.Max(maxShortMapAdjustment, 0.25 * Math.Sqrt(trueDrainTime / shortMapThreshold) + 0.75));

            calculatedDiff *= shortMapAdjustment;

            return calculatedDiff;
        }

        /// <summary>
        ///     Compute and generate Note Density Data.
        /// </summary>
        /// <param name="qssData"></param>
        /// <param name="qua"></param>
        private void ComputeNoteDensityData(float rate)
        {
            //MapLength = Qua.Length;
            AverageNoteDensity = SECONDS_TO_MILLISECONDS * Map.HitObjects.Count / (Map.Length * (-.5f * rate + 1.5f));
        }

        /// <summary>
        ///     Used to calculate Coefficient for Strain Difficulty
        /// </summary>
        private float GetCoefficientValue(float duration, float xMin, float xMax, float strainMax, float exp)
        {
            // todo: temp. Linear for now
            // todo: apply cosine curve
            const float lowestDifficulty = 1;
            const float densityMultiplier = .266f;
            const float densityDifficultyMin = .4f;

            // calculate ratio between min and max value
            var ratio = Math.Max(0, 1 - (duration - xMin) / (xMax - xMin));

            //if ratio is too big and map isnt a beginner map (nps > 4) scale based on nps instead
            if (ratio == 0 && AverageNoteDensity < 4)
            {
                //if note density is too low dont bother calculating for density either
                if (AverageNoteDensity < 1)
                    return densityDifficultyMin;

                return AverageNoteDensity * densityMultiplier + .134f;
            }

            // compute for difficulty
            return lowestDifficulty + (strainMax - lowestDifficulty) * (float)Math.Pow(ratio, exp);
        }
    }
}