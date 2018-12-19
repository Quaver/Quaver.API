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
        public static string Version { get; } = "0.0.1";

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
        private Dictionary<int, Hand> LaneToHand4K { get; set; } = new Dictionary<int, Hand>()
        {
            { 1, Hand.Left },
            { 2, Hand.Left },
            { 3, Hand.Right },
            { 4, Hand.Right }
        };

        /// <summary>
        ///     Assumes that the assigned hand will be the one to press that key
        /// </summary>
        private Dictionary<int, Hand> LaneToHand7K { get; set; } = new Dictionary<int, Hand>()
        {
            { 1, Hand.Left },
            { 2, Hand.Left },
            { 3, Hand.Left },
            { 4, Hand.Ambiguous },
            { 5, Hand.Right },
            { 6, Hand.Right },
            { 7, Hand.Right }
        };

        /// <summary>
        ///     Assumes that the assigned finger will be the one to press that key.
        /// </summary>
        private Dictionary<int, FingerState> LaneToFinger4K { get; set; } = new Dictionary<int, FingerState>()
        {
            { 1, FingerState.Middle },
            { 2, FingerState.Index },
            { 3, FingerState.Index },
            { 4, FingerState.Middle }
        };

        /// <summary>
        ///     Assumes that the assigned finger will be the one to press that key.
        /// </summary>
        private Dictionary<int, FingerState> LaneToFinger7K { get; set; } = new Dictionary<int, FingerState>()
        {
            { 1, FingerState.Ring },
            { 2, FingerState.Middle },
            { 3, FingerState.Index },
            { 4, FingerState.Thumb },
            { 5, FingerState.Index },
            { 6, FingerState.Middle },
            { 7, FingerState.Ring }
        };

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
        public DifficultyProcessorKeys(Qua map, StrainConstants constants, ModIdentifier mods = ModIdentifier.None, bool detailedSolve = false) : base(map, constants, mods)
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
            switch (Map.Mode)
            {
                case (GameMode.Keys4):
                    OverallDifficulty = ComputeForOverallDifficulty(rate);
                    break;
                case (GameMode.Keys7):
                    OverallDifficulty = (ComputeForOverallDifficulty(rate, Hand.Left) + ComputeForOverallDifficulty(rate, Hand.Right))/2;
                    break;
            }
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
                var curHitOb = new StrainSolverHitObject(Map.HitObjects[i]);
                var curStrainData = new StrainSolverData(curHitOb, rate);

                // Assign Finger and Hand States
                switch (Map.Mode)
                {
                    case GameMode.Keys4:
                        curHitOb.FingerState = LaneToFinger4K[Map.HitObjects[i].Lane];
                        curStrainData.Hand = LaneToHand4K[Map.HitObjects[i].Lane];
                        break;
                    case GameMode.Keys7:
                        curHitOb.FingerState = LaneToFinger7K[Map.HitObjects[i].Lane];
                        curStrainData.Hand = LaneToHand7K[Map.HitObjects[i].Lane] == Hand.Ambiguous ? assumeHand : LaneToHand7K[Map.HitObjects[i].Lane];
                        break;
                }

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
        ///     Compute and generate Note Density Data.
        /// </summary>
        /// <param name="qssData"></param>
        /// <param name="qua"></param>
        private void ComputeNoteDensityData(float rate)
        {
            //MapLength = Qua.Length;
            AverageNoteDensity = SECONDS_TO_MILLISECONDS * Map.HitObjects.Count / (Map.Length * rate);

            //todo: solve note density graph
            // put stuff here
        }

        /// <summary>
        ///     Used to calculate Coefficient for Strain Difficulty
        /// </summary>
        private float GetCoefficientValue(float duration, float xMin, float xMax, float strainMax, float exp)
        {
            // todo: temp. Linear for now
            // todo: apply cosine curve
            const float lowestDifficulty = 1;

            // calculate ratio between min and max value
            var ratio = Math.Max(0, (duration - xMin) / (xMax - xMin));
                ratio = 1 - Math.Min(1, ratio);

            // compute for difficulty
            return lowestDifficulty + (strainMax - lowestDifficulty) * (float)Math.Pow(ratio, exp);
        }
    }
}
