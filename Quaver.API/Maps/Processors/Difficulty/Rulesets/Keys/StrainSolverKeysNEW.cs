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
    public class StrainSolverKeysNEW : StrainSolver
    {
        /// <summary>
        ///     Constants used for solving
        /// </summary>
        public StrainConstantsKeys StrainConstants { get; private set; }

        /// <summary>
        ///     Average note density of the map
        /// </summary>
        public float AverageNoteDensity { get; private set; } = 0;

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
        public StrainSolverKeysNEW(Qua map, StrainConstants constants, ModIdentifier mods = ModIdentifier.None, bool detailedSolve = false) : base(map, constants, mods)
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
            var wristStateData = new List<WristState>();

            // Get Initial Handstates
            // - Iterate through hit objects backwards
            hitObjects.Reverse();
            List<HandStateData> refHandData;
            for (var i = 0; i < hitObjects.Count; i++)
            {
                // Determine Reference Hand
                switch (Map.Mode)
                {
                    case GameMode.Keys4:
                        if (LaneToHand4K[hitObjects[i].HitObject.Lane] == Hand.Left)
                            refHandData = leftHandData;
                        else
                            refHandData = rightHandData;
                        break;
                    case GameMode.Keys7:
                        var hand = LaneToHand7K[hitObjects[i].HitObject.Lane];
                        if (hand == Hand.Left || (hand == Hand.Ambiguous && assumeHand == Hand.Left))
                            refHandData = leftHandData;
                        else
                            refHandData = rightHandData;
                        break;
                    default:
                        throw new Exception("Unknown GameMode");
                }

                // Iterate through established handstates for chords
                var chordFound = false;
                for (var j = 0; j < refHandData.Count; j++)
                {
                    // Break loop after leaving threshold
                    if (refHandData[j].HitObjects[0].HitObject.StartTime
                        > hitObjects[i].HitObject.StartTime + HandStateData.CHORD_THRESHOLD_SAMEHAND_MS)
                        break;

                    // Check for finger overlap
                    chordFound = true;
                    for (var k = 0; k < refHandData[j].HitObjects.Count; k++)
                    {
                        if (refHandData[j].HitObjects[k].HitObject.Lane == hitObjects[i].HitObject.Lane)
                        {
                            chordFound = false;
                            break;
                        }
                    }

                    // Add HitObject to Chord if no fingers overlap
                    if (chordFound)
                    {
                        refHandData[j].AddHitObjectToChord(hitObjects[i]);
                        break;
                    }
                }

                // Add new HandStateData to list if no chords are found
                if (!chordFound)
                    refHandData.Add(new HandStateData(hitObjects[i]));

                //Console.WriteLine(chordFound);
            }

            // Compute for wrist action
            // maybe solve this first?
            for (var i = 0; i < hitObjects.Count; i++)
            {
                if (hitObjects[i].WristState == null)
                {
                    var state = hitObjects[i].FingerState;
                    var wrist = new WristState();
                    for (var j = i + 1; j < hitObjects.Count; j++)
                    {
                        if (((int)hitObjects[j].FingerState & (1 << (int)state - 1)) != 0)
                        {
                            break;
                        }

                        state |= hitObjects[j].FingerState;
                        hitObjects[j].WristState = wrist;
                    }

                    wrist.WristPair = state;

                    if (state != hitObjects[i].FingerState)
                    {
                        hitObjects[i].WristState = wrist;
                    }
                }
                //Console.WriteLine(hitObjects[i].WristState == null);
            }

            // TEST CALC
            var count = 0;
            float total = 0;
            refHandData = leftHandData;
            for (var i = 0; i < refHandData.Count - 2; i++)
            {
                refHandData[i].EvaluateDifficulty();
                if ((refHandData[i].HitObjects[0].HitObject.StartTime
                    - refHandData[i + 2].HitObjects[0].HitObject.StartTime) != 0)
                {
                    count++;
                    total
                        += refHandData[i].StateDifficulty * 1500 / (refHandData[i].HitObjects[0].HitObject.StartTime
                        - refHandData[i + 2].HitObjects[0].HitObject.StartTime);
                }
            }

            refHandData = rightHandData;
            for (var i = 0; i < refHandData.Count - 2; i++)
            {
                refHandData[i].EvaluateDifficulty();
                if ((refHandData[i].HitObjects[0].HitObject.StartTime
                    - refHandData[i + 2].HitObjects[0].HitObject.StartTime) != 0)
                {
                    count++;
                    total
                        += refHandData[i].StateDifficulty * 1500 / (refHandData[i].HitObjects[0].HitObject.StartTime
                        - refHandData[i + 2].HitObjects[0].HitObject.StartTime);
                }
            }

            // temp diff
            if (count == 0) return 0;
            return 2 * total / count;
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
            {
                hitObjects.Add(new StrainSolverHitObject(ho, Map.Mode));
                // todo: implement assume hand
            }
            return hitObjects;
        }

        /*
        /// <summary>
        ///     Get Note Data, and compute the base strain weights
        ///     The base strain weights are affected by LN layering
        /// </summary>
        /// <param name="qssData"></param>
        /// <param name="qua"></param>
        /// <param name="assumeHand"></param>
        private void ConvertToStrainHitObjects(float rate, Hand assumeHand)
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
        }*/

        /// <summary>
        ///     This method will interpolate Bpm to Strain Difficulty using a reference Dictionary.
        /// </summary>
        /// <param name="bpm"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        private static float InterpolateBpmToDifficulty(float bpm, Dictionary<float, float> reference)
        {
            // Return highest difficulty possible if bpm is greater than the last index's bpm.
            if (bpm >= reference.Keys.Last())
                return reference.Values.Last();

            // Interpolate between two bpm points and return difficulty
            // TODO: This method is incredibly inefficient. Either generate a curve procedurally or find better looping method
            for (var i = 0; i < reference.Count; i++)
            {
                if (reference.Keys.ElementAt(i) >= bpm)
                {
                    if (i == 0)
                        break;

                    var nextBpm = reference.Keys.ElementAt(i);
                    var nextDiff = reference.Values.ElementAt(i);
                    var prevBpm = reference.Keys.ElementAt(i - 1);
                    var prevDiff = reference.Values.ElementAt(i - 1);

                    return (((bpm - prevBpm) / (nextBpm - prevBpm)) * (nextDiff - prevDiff)) + prevDiff;
                }
            }

            // Return first value if bpm is less than the lowest Key.
            return reference.Values.First();
        }
    }
}
