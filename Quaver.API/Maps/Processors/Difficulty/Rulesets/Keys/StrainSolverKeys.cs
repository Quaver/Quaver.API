using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.API.Maps;
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
    public class StrainSolverKeys : StrainSolver
    {
        //todo: remove this later. TEMP
        public int Roll { get; set; } = 0;
        public int SJack { get; set; } = 0;
        public int TJack { get; set; } = 0;
        public int Bracket { get; set; } = 0;

        /// <summary>
        ///     Overall difficulty of the map
        /// </summary>
        public override float OverallDifficulty { get; internal set; } = 0;

        /// <summary>
        /// TODO: remove this later. TEMPORARY.
        /// </summary>
        public string DebugString { get; private set; } = "";

        /// <summary>
        ///     Size of each graph partition in miliseconds
        /// </summary>
        public const int GRAPH_INTERVAL_SIZE_MS = 500;

        /// <summary>
        ///     Offset between each graph partition in miliseconds
        /// </summary>
        public const int GRAPH_INTERVAL_OFFSET_MS = 100;

        //todo: document. Might change names/logic later
        // threshold on when to ignore LN layering if startTime/endTime between 2 hit objects are too close to eachother
        private const float THRESHOLD_LN_END_MS = 42;
        // threshold on when to ignore notes when merging multiple hit objects into chords
        private const float THRESHOLD_CHORD_CHECK_MS = 8;

        /// <summary>
        ///     Total ammount of milliseconds in a second.
        /// </summary>
        public const float SECONDS_TO_MILLISECONDS = 1000;

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
        ///     const
        /// </summary>
        /// <param name="qua"></param>
        public StrainSolverKeys(Qua map, ModIdentifier mods = ModIdentifier.None) : base(map, mods)
        {
            // Don't bother calculating map difficulty if there's less than 2 hit objects
            if (map.HitObjects.Count < 2)
                return;

            // Solve for difficulty
            CalculateDifficulty(mods);
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
            ComputeNoteDensityData(rate);
            ComputeBaseStrainStates(rate);
            ComputeForChords();
            ComputeFingerActions();
            ComputeActionPatterns();
            CalculateOverallDifficulty();
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
        ///     Get Note Data, and compute the base strain weights
        ///     The base strain weights are affected by LN layering
        /// </summary>
        /// <param name="qssData"></param>
        /// <param name="qua"></param>
        private void ComputeBaseStrainStates(float rate)
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
                        curStrainData.Hand = LaneToHand7K[Map.HitObjects[i].Lane];
                        break;
                }

                // Add Strain Solver Data to list
                StrainSolverData.Add(curStrainData);
            }

            // Solve LN
            // todo: put this in its own method maybe?
            for (var i = 0; i < StrainSolverData.Count - 1; i++)
            {
                var curHitOb = StrainSolverData[i];
                for (var j = i + 1; j < StrainSolverData.Count; j++)
                {
                    // If the target hit object is way outside the current LN end, don't bother iterating through the rest.
                    var nextHitOb = StrainSolverData[j];
                    if (nextHitOb.StartTime > curHitOb.EndTime + THRESHOLD_LN_END_MS)
                        break;

                    // Check to see if the target hitobject is layered inside the current LN
                    if (nextHitOb.Hand == curHitOb.Hand && nextHitOb.StartTime >= curHitOb.StartTime + THRESHOLD_CHORD_CHECK_MS)
                    {
                        // Target hitobject's LN ends after current hitobject's LN end.
                        if (nextHitOb.EndTime > curHitOb.EndTime)
                        {
                            foreach (var k in curHitOb.HitObjects)
                            {
                                k.LnLayerType = LnLayerType.OutsideRelease;
                                k.LnStrainMultiplier = 1.5f; //TEMP STRAIN MULTIPLIER. use constant later.
                            }
                        }

                        // Target hitobject's LN ends before current hitobject's LN end
                        else if (nextHitOb.EndTime > 0)
                        {
                            foreach (var k in curHitOb.HitObjects)
                            {
                                k.LnLayerType = LnLayerType.InsideRelease;
                                k.LnStrainMultiplier = 1.2f; //TEMP STRAIN MULTIPLIER. use constant later.
                            }
                        }

                        // Target hitobject is not an LN
                        else
                        {
                            foreach (var k in curHitOb.HitObjects)
                            {
                                k.LnLayerType = LnLayerType.InsideTap;
                                k.LnStrainMultiplier = 1.05f; //TEMP STRAIN MULTIPLIER. use constant later.
                            }
                        }
                    }
                }
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
                    var msDiff = StrainSolverData[j].StartTime - StrainSolverData[i].StartTime;

                    if (msDiff > THRESHOLD_CHORD_CHECK_MS)
                        break;

                    if (Math.Abs(msDiff) <= THRESHOLD_CHORD_CHECK_MS)
                    {
                        if (StrainSolverData[i].Hand == StrainSolverData[j].Hand)
                        {
                            // There should really only be one hit object for 4k, but maybe more than for 7k
                            foreach (var k in StrainSolverData[j].HitObjects)
                                StrainSolverData[i].HitObjects.Add(k);

                            // Remove chorded object
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
        private void ComputeFingerActions()
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
                        var actionJackFound = ((int)nextHitOb.FingerState & (1 << (int)curHitOb.FingerState - 1)) != 0;

                        // Determined by if a chord is found in either finger state
                        var actionChordFound = curHitOb.HandChord || nextHitOb.HandChord;

                        // Determined by if both fingerstates are exactly the same
                        var actionSameState = curHitOb.FingerState == nextHitOb.FingerState;

                        // Determined by how long the current finger action is
                        var actionDuration = nextHitOb.StartTime - curHitOb.StartTime;

                        //todo: REMOVE. this is for debuggin.
                        //DebugString += (i + " | jack: " + actionJackFound + ", chord: " + actionChordFound + ", samestate: " + actionSameState + ", c-index: " + curHitOb.HandChordState + ", h-diff: " + actionDuration + "\n");

                        // Trill/Roll
                        if (!actionChordFound && !actionSameState)
                        {
                            curHitOb.FingerAction = FingerAction.Roll;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 40, 160, 54, 0.97f); // todo: temp. Apply actual constants later
                            Roll++;
                        }

                        // Simple Jack
                        else if (actionSameState)
                        {
                            curHitOb.FingerAction = FingerAction.SimpleJack;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 30, 280, 91, 1.09f); // todo: temp. Apply actual constants later
                            SJack++;
                        }

                        // Tech Jack
                        else if (actionJackFound)
                        {
                            curHitOb.FingerAction = FingerAction.TechnicalJack;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 20, 280, 94, 1.06f); // todo: temp. Apply actual constants later
                            TJack++;
                        }

                        // Bracket
                        else
                        {
                            curHitOb.FingerAction = FingerAction.Bracket;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 40, 160, 56, 0.97f); // todo: temp. Apply actual constants later
                            Bracket++;
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
        private void ComputeActionPatterns()
        {

        }

        /// <summary>
        ///     Calculates the general difficulty of a map
        /// </summary>
        /// <param name="qssData"></param>
        private void CalculateOverallDifficulty()
        {
            // Calculate the strain value for every data point
            foreach (var data in StrainSolverData)
            {
                data.CalculateStrainValue();
            }

            // Solve for difficulty (temporary)
            // Difficulty is determined by how long each action is and how difficult they are.
            //  - longer actions have more weight due to it taking up more of the maps' length.
            //  - generally shorter actions are harder, but a bunch of hard actions are obviously more difficulty than a single hard action

            // overall difficulty = sum of all actions:(difficulty * action length) / map length
            // todo: action length is currently manually calculated.
            // todo: maybe store action length in StrainSolverData because it already gets calculated earlier?


            // todo: make this look better
            switch (Map.Mode)
            {
                case Enums.GameMode.Keys4:
                    OverallDifficulty = CalculateDifficulty4K();
                    break;

                case Enums.GameMode.Keys7:
                    OverallDifficulty = CalculateDifficulty7K();
                    break;
            }

            // calculate stamina (temp solution)
            // it just uses the map's length.
            // 10 seconds = 0.9x multiplier
            // 100 seconds = 1.0x multiplier
            // 1000 seconds = 1.1x multiplier
            // 10000 seconds = 1.2x multiplier, ect.
            //OverallDifficulty *= (float)(0.5 + Math.Log10(Map.Length / rate) / 10);
        }

        /// <summary>
        ///     Calculate the general difficulty for a 4K map
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        private float CalculateDifficulty4K()
        {
            float calculatedDiff = 0;

            // left hand
            foreach (var data in StrainSolverData)
            {
                if (data.Hand == Hand.Left)
                    calculatedDiff += data.TotalStrainValue;
            }

            // right hand
            foreach (var data in StrainSolverData)
            {
                if (data.Hand == Hand.Right)
                    calculatedDiff += data.TotalStrainValue;
            }

            // Calculate overall 4k difficulty
            calculatedDiff /= StrainSolverData.Count;

            // Get Overall 4k difficulty
            return calculatedDiff;
        }

        /// <summary>
        ///     Calculate the general difficulty for a 7k map
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        private float CalculateDifficulty7K()
        {
            //todo: Implement Ambiguious Hand in calculation
            float calculatedDiff = 0;

            // left hand
            foreach (var data in StrainSolverData)
            {
                if (data.Hand == Hand.Left)
                    calculatedDiff += data.TotalStrainValue;
            }

            // right hand
            foreach (var data in StrainSolverData)
            {
                if (data.Hand == Hand.Right)
                    calculatedDiff += data.TotalStrainValue;
            }

            // Get overall 7k Difficulty
            calculatedDiff /= StrainSolverData.Count;

            // Get Overall 7k difficulty
            return calculatedDiff;
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
