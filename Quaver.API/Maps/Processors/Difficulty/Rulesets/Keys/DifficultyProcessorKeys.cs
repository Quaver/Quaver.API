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
using Quaver.API.Maps.Structures;
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
        ///     Constants used for solving
        /// </summary>
        public DifficultyConstantsKeys Constants { get; }

        /// <summary>
        ///     Assumes that the assigned hand will be the one to press that key
        /// </summary>
        public static Dictionary<int, Hand> LaneToHand4K { get; } = new Dictionary<int, Hand>()
        {
            { 1, Hand.Left },
            { 2, Hand.Left },
            { 3, Hand.Right },
            { 4, Hand.Right }
        };

        /// <summary>
        ///     Assumes that the assigned hand will be the one to press that key
        /// </summary>
        public static Dictionary<int, Hand> LaneToHand7K { get; } = new Dictionary<int, Hand>()
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
        public static Dictionary<int, Finger> LaneToFinger4K { get; } = new Dictionary<int, Finger>()
        {
            { 1, Finger.Middle },
            { 2, Finger.Index },
            { 3, Finger.Index },
            { 4, Finger.Middle }
        };

        /// <summary>
        ///     Assumes that the assigned finger will be the one to press that key.
        /// </summary>
        public static Dictionary<int, Finger> LaneToFinger7K { get; } = new Dictionary<int, Finger>()
        {
            { 1, Finger.Ring },
            { 2, Finger.Middle },
            { 3, Finger.Index },
            { 4, Finger.Thumb },
            { 5, Finger.Index },
            { 6, Finger.Middle },
            { 7, Finger.Ring }
        };

        /// <summary>
        ///     Solves the difficulty of a .qua file
        /// </summary>
        /// <param name="map"></param>
        /// <param name="constants"></param>
        /// <param name="mods"></param>
        /// <param name="detailedSolve"></param>
        public DifficultyProcessorKeys(Qua map, DifficultyConstantsKeys constants, ModIdentifier mods = ModIdentifier.None, bool detailedSolve = false) : base(map, constants, mods)
        {
            // Cast the current Strain Constants Property to the correct type.
            Constants = constants;

            // Don't bother calculating map difficulty if there's less than 2 HitObjects
            if (map.HitObjects.Count < 2)
                return;

            // Solve for difficulty
            CalculateDifficulty(mods);

            // If detailed solving is enabled, expand calculation
            if (detailedSolve)
            {
                // todo: solve graphs and stuff you would see in Song Select Screen/Editor.
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
                    OverallDifficulty = (ComputeForOverallDifficulty(rate, Hand.Left) + ComputeForOverallDifficulty(rate, Hand.Right)) / 2;
                    break;
            }
        }

        /// <summary>
        ///     Calculate overall difficulty of a map. "AssumeHand" is used for odd-numbered keymodes.
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="assumeHand"></param>
        /// <returns></returns>
        private float ComputeForOverallDifficulty(float rate, Hand assumeHand = Hand.Right)
        {
            // Convert to hitobjects. The Algorithm iterates through the HitObjects backwards.
            var hitObjects = ConvertToStrainHitObject(assumeHand);
            hitObjects.Reverse();

            // Get States
            var data = ComputeForInitialStates(hitObjects, assumeHand);
            var left = data.left;
            var right = data.right;
            var all = data.all;

            // Compute for chorded pairs.
            ComputeForDifferentHandChords(all);

            // Compute for wrist action.
            ComputeForWristAction(hitObjects);

            // Compute for Stamina Difficulty.
            return ComputeForStaminaDifficulty(left, right);
        }

        /// <summary>
        ///     Convert Regular HitObjects to Strain HitObjects.
        ///     - Strain HitObject is also used for dynamic difficulty calculation in Map Editor.
        /// </summary>
        /// <param name="assumeHand"></param>
        /// <returns></returns>
        private List<DPHitObject> ConvertToStrainHitObject(Hand assumeHand)
        {
            var hitObjects = new List<DPHitObject>();
            foreach (var ho in Map.HitObjects)
                hitObjects.Add(new DPHitObject(ho, Map.Mode, assumeHand));

            return hitObjects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hitObjects"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private (List<HandStateData> left, List<HandStateData> right, List<HandStateData> all) ComputeForInitialStates(List<DPHitObject> hitObjects, Hand assumeHand)
        {
            var left = new List<HandStateData>();
            var right = new List<HandStateData>();
            var all = new List<HandStateData>();

            foreach (var data in hitObjects)
            {
                // Determine Reference Hand
                List<HandStateData> refHand;
                switch (Map.Mode)
                {
                    case GameMode.Keys4:
                        if (LaneToHand4K[data.HitObject.Lane] == Hand.Left)
                            refHand = left;
                        else
                            refHand = right;
                        break;
                    case GameMode.Keys7:
                        var hand = LaneToHand7K[data.HitObject.Lane];
                        if (hand.Equals(Hand.Left) || (hand.Equals(Hand.Ambiguous) && assumeHand.Equals(Hand.Left)))
                            refHand = left;
                        else
                            refHand = right;
                        break;
                    default:
                        throw new Exception("Unknown GameMode");
                }

                // Iterate through established handstates for Same-Hand Chords
                var chordFound = false;
                foreach (var reference in refHand)
                {
                    // Break loop after leaving threshold
                    if (reference.Time
                        < data.StartTime + Constants.ChordThresholdSameHandMs)
                        break;

                    // Check for finger overlap
                    chordFound = true;
                    foreach (var check in reference.HitObjects)
                    {
                        if (check.HitObject.Lane == data.HitObject.Lane)
                        {
                            chordFound = false;
                            break;
                        }
                    }

                    // Add HitObject to Chord if no fingers overlap
                    if (chordFound)
                    {
                        //Console.WriteLine("chordfound");
                        reference.AddHitObjectToChord(data);
                        break;
                    }
                }

                // Add new HandStateData to list if no chords are found
                if (!chordFound)
                {
                    refHand.Add(new HandStateData(data));
                    all.Add(refHand.Last());
                }
            }

            return (left, right, all); ;
        }

        /// <summary>
        ///     Solve and assign HandStateData to other HandStateData as chorded pairs.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void ComputeForDifferentHandChords(List<HandStateData> data)
        {
            for (var i = 0; i < data.Count; i++)
            {
                if (data[i].Hand.Equals(Hand.Ambiguous))
                    throw new Exception("Ambiguous Hand Found");

                for (var j = i + 1; j < data.Count; j++)
                {
                    if (data[i].Time - data[j].Time > Constants.ChordThresholdOtherHandMs)
                        break;

                    if (data[j].ChordedHand == null)
                    {
                        if (!data[i].Hand.Equals(data[j].Hand) && !data[j].Hand.Equals(Hand.Ambiguous))
                        {
                            data[j].ChordedHand = data[i];
                            data[i].ChordedHand = data[j];
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Solve and assign Wrist States to every HitObject if necessary.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void ComputeForWristAction(List<DPHitObject> hitObjects)
        {
            WristStateData laterStateLeft = null;
            WristStateData laterStateRight = null;
            for (var i = 0; i < hitObjects.Count; i++)
            {
                // Ignore HitObjects with already solved Wrist States.
                if (hitObjects[i].WristState == null)
                {
                    // Assign Wrist State and reference appropriate preceding State.
                    WristStateData wrist;
                    var state = hitObjects[i].FingerState;
                    if (hitObjects[i].Hand == Hand.Left)
                    {
                        wrist = new WristStateData(laterStateLeft);
                        laterStateLeft = wrist;
                    }
                    else
                    {
                        wrist = new WristStateData(laterStateRight);
                        laterStateRight = wrist;
                    }

                    // Update Current State.
                    for (var j = i + 1; j < hitObjects.Count; j++)
                    {
                        if (hitObjects[j].Hand == hitObjects[i].Hand)
                        {
                            // Break loop upon same finger found.
                            if (((int)state & (1 << (int)hitObjects[j].FingerState - 1)) != 0)
                                break;

                            state |= hitObjects[j].FingerState;
                            hitObjects[j].WristState = wrist;
                        }
                    }

                    // Update Wrist State.
                    wrist.WristPair = state;
                    wrist.Time = hitObjects[i].StartTime;

                    // Check if Wrist Manipulation is involved. (Example: Rolls).
                    if (!state.Equals(hitObjects[i].FingerState))
                    {
                        wrist.WristAction = WristOrientation.Up;
                        hitObjects[i].WristState = wrist;
                    }
                    // Check for Simple Jacks.
                    else if (wrist.NextState != null && wrist.NextState.WristPair.Equals(state))
                    {
                        wrist.WristAction = WristOrientation.Up;
                        hitObjects[i].WristState = wrist;
                    }
                    // Anchor / Control is involved,
                    // Do not remove this despite what ReSharper says!
                    else
                        wrist = null;
                }
            }
        }

        /// <summary>
        ///     Compute Overall Difficulty with Stamina Interpolation.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private float ComputeForStaminaDifficulty(List<HandStateData> left, List<HandStateData> right)
        {
            float total = 0;
            for (var z = 0; z <= 1; z++)
            {
                float currentDiff = 0;
                var reference = z == 0 ? left : right;
                for (var i = 0; i < reference.Count - 2; i++)
                {
                    reference[i].EvaluateDifficulty(Constants, reference[i].Time - reference[i + 2].Time);
                    if (reference[i].StateDifficulty > currentDiff)
                        currentDiff += (reference[i].StateDifficulty - currentDiff) * Constants.StaminaIncrementalMultiplier.Value;
                    else
                        currentDiff += (reference[i].StateDifficulty - currentDiff) * Constants.StaminaDecrementalMultiplier.Value;

                    total += currentDiff;
                }
            }

            // Stamina Multiplier Bonus,
            var count = left.Count + right.Count;
            var stamina = (float)(Math.Log10(count) / 25 + 0.9);

            // Overall Difficulty,
            if (count == 0) return 0;
            return stamina * total / count;
        }

        /// <summary>
        ///     Count Total Chords in the Map.
        ///     Todo: This is unused for now, but may be implemented for in-game later.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<ChordType, int> CountTotalChords(List<HandStateData> data)
        {
            // Initialize Dictionary
            var count = new Dictionary<ChordType, int>()
            {
                {ChordType.None, 0},
                {ChordType.Single, 0},
                {ChordType.Jump, 0},
                {ChordType.Hand, 0},
                {ChordType.Quad, 0},
                {ChordType.NChord, 0},
            };

            // Count Chords
            foreach (var state in data)
            {
                var otherHand = state.ChordedHand == null ? 0 : state.ChordedHand.HitObjects.Count;
                switch (state.HitObjects.Count + otherHand)
                {
                    case 0:
                        count[ChordType.None]++;
                        break;
                    case 1:
                        count[ChordType.Single]++;
                        break;
                    case 2:
                        count[ChordType.Jump]++;
                        break;
                    case 3:
                        count[ChordType.Hand]++;
                        break;
                    case 4:
                        count[ChordType.Quad]++;
                        break;
                    default:
                        count[ChordType.NChord]++;
                        break;
                }
            }

            return count;
        }
    }
}
