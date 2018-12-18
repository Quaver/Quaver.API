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
    public class StrainSolverKeys : StrainSolver
    {
        /// <summary>
        ///     Constants used for solving
        /// </summary>
        public StrainConstantsKeys StrainConstants { get; }

        /// <summary>
        ///     Average note density of the map
        /// </summary>
        public float AverageNoteDensity { get; } = 0;

        /// <summary>
        ///     Assumes that the assigned hand will be the one to press that key
        /// </summary>
        public static Dictionary<int, Hand> LaneToHand4K { get;} = new Dictionary<int, Hand>()
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
        public static Dictionary<int, FingerState> LaneToFinger4K { get; } = new Dictionary<int, FingerState>()
        {
            { 1, FingerState.Middle },
            { 2, FingerState.Index },
            { 3, FingerState.Index },
            { 4, FingerState.Middle }
        };

        /// <summary>
        ///     Assumes that the assigned finger will be the one to press that key.
        /// </summary>
        public static Dictionary<int, FingerState> LaneToFinger7K { get; } = new Dictionary<int, FingerState>()
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
        public StrainSolverKeys(Qua map, StrainConstants constants, ModIdentifier mods = ModIdentifier.None, bool detailedSolve = false) : base(map, constants, mods)
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
                //ComputeForPatternFlags();
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
            // Convert to hitobjects
            var hitObjects = ConvertToStrainHitObject(assumeHand);
            var leftHandData = new List<HandStateData>();
            var rightHandData = new List<HandStateData>();
            var allHandData = new List<HandStateData>();

            // Get Initial Handstates
            // - Iterate through hit objects backwards
            hitObjects.Reverse();
            foreach (var data in hitObjects)
            {
                // Determine Reference Hand
                List<HandStateData> refHandData;
                switch (Map.Mode)
                {
                    case GameMode.Keys4:
                        if (LaneToHand4K[data.HitObject.Lane] == Hand.Left)
                            refHandData = leftHandData;
                        else
                            refHandData = rightHandData;
                        break;
                    case GameMode.Keys7:
                        var hand = LaneToHand7K[data.HitObject.Lane];
                        if (hand.Equals(Hand.Left) || (hand.Equals(Hand.Ambiguous) && assumeHand.Equals(Hand.Left)))
                            refHandData = leftHandData;
                        else
                            refHandData = rightHandData;
                        break;
                    default:
                        throw new Exception("Unknown GameMode");
                }

                // Iterate through established handstates for chords
                var chordFound = false;
                foreach (var reference in refHandData)
                {
                    // Break loop after leaving threshold
                    if (reference.Time
                        > data.StartTime + StrainConstants.ChordThresholdSameHandMs)
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
                        reference.AddHitObjectToChord(data);
                        break;
                    }
                }

                // Add new HandStateData to list if no chords are found
                if (!chordFound)
                {
                    refHandData.Add(new HandStateData(data));
                    allHandData.Add(refHandData.Last());
                }
            }

            // Compute for chorded pairs
            ComputeForChordedPairs(allHandData);

            // Compute for wrist action
            ComputeForWristAction(hitObjects);

            // Compute for Stamina Difficulty
            return ComputeForStaminaDifficulty(leftHandData, rightHandData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assumeHand"></param>
        /// <returns></returns>
        private List<StrainSolverHitObject> ConvertToStrainHitObject(Hand assumeHand)
        {
            var hitObjects = new List<StrainSolverHitObject>();
            foreach (var ho in Map.HitObjects)
                hitObjects.Add(new StrainSolverHitObject(ho, Map.Mode, assumeHand));

            return hitObjects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void ComputeForChordedPairs(List<HandStateData> data)
        {
            for (var i = 0; i < data.Count; i++)
            {
                for (var j = i + 1; j < data.Count; j++)
                {
                    if (data[i].Time - data[j].Time > StrainConstants.ChordThresholdOtherHandMs)
                    {
                        break;
                    }

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
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void ComputeForWristAction(List<StrainSolverHitObject> hitObjects)
        {
            WristState laterStateLeft = null;
            WristState laterStateRight = null;
            WristState wrist;
            FingerState state;
            for (var i = 0; i < hitObjects.Count; i++)
            {
                if (hitObjects[i].WristState == null)
                {
                    state = hitObjects[i].FingerState;
                    if (hitObjects[i].Hand == Hand.Left)
                    {
                        wrist = new WristState(laterStateLeft);
                        laterStateLeft = wrist;
                    }
                    else
                    {
                        wrist = new WristState(laterStateRight);
                        laterStateRight = wrist;
                    }

                    for (var j = i + 1; j < hitObjects.Count; j++)
                    {
                        if (hitObjects[j].Hand == hitObjects[i].Hand)
                        {
                            // Break loop upon same finger found
                            if (((int)state & (1 << (int)hitObjects[j].FingerState - 1)) != 0)
                                break;

                            state |= hitObjects[j].FingerState;
                            hitObjects[j].WristState = wrist;
                        }
                    }

                    wrist.WristPair = state;
                    wrist.Time = hitObjects[i].StartTime;

                    // check if wrist state is the same
                    if (!state.Equals(hitObjects[i].FingerState))
                    {
                        wrist.WristAction = WristAction.Up;
                        hitObjects[i].WristState = wrist;
                        //Console.WriteLine($"asd{state}_ {hitObjects[i].FingerState} | {Map.Length}, {hitObjects[i].StartTime}");
                    }
                    // same state jack
                    else if (wrist.NextState != null && wrist.NextState.WristPair.Equals(state))
                    {
                        wrist.WristAction = WristAction.Up;
                        hitObjects[i].WristState = wrist;
                    }
                    else
                    {
                        wrist = null;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private float ComputeForStaminaDifficulty(List<HandStateData> left, List<HandStateData> right)
        {
            // temp calc variables
            var count = 0;
            float total = 0;

            // TEST CALC (OLD)
            for (var z = 0; z <= 1; z++)
            {
                var reference = z == 0 ? left : right;
                float currentDiff = 0;
                for (var i = 0; i < reference.Count - 2; i++)
                {
                    // Interpolate Current Difficulty with Stamina
                    // TODO: put this crap in HandStateData class
                    reference[i].EvaluateDifficulty(StrainConstants);
                    if (reference[i].StateDifficulty < currentDiff)
                    {
                        currentDiff += (reference[i].StateDifficulty - currentDiff)
                            * StrainConstants.StaminaDecrementalMultiplier.Value * Math.Min((reference[i].Time - reference[i + 2].Time) / StrainConstants.MaxStaminaDelta, 1);
                    }
                    else
                    {
                        currentDiff += (reference[i].StateDifficulty - currentDiff)
                            * StrainConstants.StaminaIncrementalMultiplier.Value * Math.Min((reference[i].Time - reference[i + 2].Time / StrainConstants.MaxStaminaDelta), 1);
                    }

                    // Calculate Difficulty of Current Section
                    // TODO: put this crap in HandStateData class
                    if ((reference[i].Time
                        - reference[i + 2].Time) > 0)
                    {
                        count++;
                        total
                            += Math.Max(1, reference[i].StateDifficulty
                            * StrainConstants.DifficultyMultiplier
                            * (float)Math.Sqrt(30000 / (reference[i].Time - reference[i + 2].Time)) + StrainConstants.DifficultyOffset);
                    }
                    else
                    {
                        throw new Exception("HandStateData Action Delta is 0 or negative value.");
                    }
                }
            }

            // temp diff
            var stam = (float)(Math.Log10(count) / 25 + 0.9);
            if (count == 0) return 0;
            return stam * total / count;
        }
    }
}
