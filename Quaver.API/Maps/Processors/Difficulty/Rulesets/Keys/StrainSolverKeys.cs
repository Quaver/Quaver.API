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

        private const float THRESHOLD_LN_END_MS = 42;
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
        public StrainSolverKeys(Qua map, float rate = 1) : base(map, rate)
        {
            // Don't bother calculating map difficulty if there's less than 2 hit objects
            if (map.HitObjects.Count < 2) return;

            // Solve for difficulty
            CalculateDifficulty(rate);
        }

        /// <summary>
        ///     Calculate difficulty of a map with given rate
        /// </summary>
        /// <param name="rate"></param>
        public void CalculateDifficulty(float rate = 1)
        {
            // If map does not exist, ignore calculation.
            if (Map == null) return;

            ComputeNoteDensityData(rate);
            ComputeBaseStrainStates(rate);
            ComputeForChords();
            ComputeFingerActions();
            ComputeActionPatterns();
            CalculateOverallDifficulty(rate);
        }

        /// <summary>
        ///     Compute and generate Note Density Data.
        /// </summary>
        /// <param name="qssData"></param>
        /// <param name="qua"></param>
        private void ComputeNoteDensityData(float rate = 1)
        {
            //MapLength = Qua.Length;
            AverageNoteDensity = SECONDS_TO_MILLISECONDS * Map.HitObjects.Count / (Map.Length * rate);

            //todo: solve note density graph
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
                    case Enums.GameMode.Keys4:
                        curHitOb.FingerState = LaneToFinger4K[Map.HitObjects[i].Lane];
                        curStrainData.Hand = LaneToHand4K[Map.HitObjects[i].Lane];
                        break;
                    case Enums.GameMode.Keys7:
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
                            foreach (var k in curHitOb.HitObjects)
                            {
                                k.LnLayerType = LnLayerType.OutsideRelease;
                                k.LnStrainMultiplier = 1.5f; //TEMP STRAIN MULTIPLIER. use constant later.
                            }

                        // Target hitobject's LN ends before current hitobject's LN end
                        else if (nextHitOb.EndTime > 0)
                            foreach (var k in curHitOb.HitObjects)
                            {
                                k.LnLayerType = LnLayerType.InsideRelease;
                                k.LnStrainMultiplier = 1.2f; //TEMP STRAIN MULTIPLIER. use constant later.
                            }

                        // Target hitobject is not an LN
                        else
                            foreach (var k in curHitOb.HitObjects)
                            {
                                k.LnLayerType = LnLayerType.InsideTap;
                                k.LnStrainMultiplier = 1.05f; //TEMP STRAIN MULTIPLIER. use constant later.
                            }
                    }
                }
            }
        }

        /// <summary>
        ///     Iterate through the HitObject list and assign HitObjects to each chord
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

            // Calculate HandChord State State Index (Value of the keys pressed on a single hand)
            for (var i = 0; i < StrainSolverData.Count; i++)
            {
                foreach (var j in StrainSolverData[i].HitObjects)
                    StrainSolverData[i].HandChordState += (int)j.FingerState;
            }
        }

        //todo: remove this later. TEMP
        public int Roll { get; set; } = 0;
        public int SJack { get; set; } = 0;
        public int TJack { get; set; } = 0;
        public int Bracket { get; set; } = 0;

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
                        var actionJackFound = (nextHitOb.HandChordState & (1 << curHitOb.HandChordState - 1)) != 0;

                        // Determined by if a chord is found in either finger state
                        var actionChordFound = curHitOb.HandChord || nextHitOb.HandChord;

                        // Determined by if both fingerstates are exactly the same
                        var actionSameState = curHitOb.HandChordState == nextHitOb.HandChordState;

                        // Determined by how long the current finger action is
                        var actionDuration = nextHitOb.StartTime - curHitOb.StartTime;

                        //todo: REMOVE. this is for debuggin.
                        //DebugString += (i + " | jack: " + actionJackFound + ", chord: " + actionChordFound + ", samestate: " + actionSameState + ", c-index: " + curHitOb.HandChordState + ", h-diff: " + actionDuration + "\n");

                        // Trill/Roll
                        if (!actionChordFound && !actionSameState)
                        {
                            curHitOb.FingerAction = FingerAction.Roll;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 20, 360, 57, 1.04f); // todo: temp. Apply actual constants later
                            Roll++;
                        }

                        // Simple Jack
                        else if (actionSameState)
                        {
                            curHitOb.FingerAction = FingerAction.SimpleJack;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 50, 560, 60, 1.12f); // todo: temp. Apply actual constants later
                            SJack++;
                        }

                        // Tech Jack
                        else if (actionJackFound)
                        {
                            curHitOb.FingerAction = FingerAction.TechnicalJack;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 50, 560, 59, 1.10f); // todo: temp. Apply actual constants later
                            TJack++;
                        }

                        // Bracket
                        else
                        {
                            curHitOb.FingerAction = FingerAction.Bracket;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 20, 360, 60, 1.02f); // todo: temp. Apply actual constants later
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
        ///     Calculates the general difficulty of a beatmap
        /// </summary>
        /// <param name="qssData"></param>
        private void CalculateOverallDifficulty(float rate = 1)
        {
            //todo: temporary.
            //OverallDifficulty = AverageNoteDensity * 3.25f;
            foreach (var i in StrainSolverData)
            {
                OverallDifficulty += i.ActionStrainCoefficient;
            }
            OverallDifficulty /= StrainSolverData.Count;

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
                // 4k difficulty calculation
                case Enums.GameMode.Keys4:
                    #region 4k calc
                    // left hand
                    for (var i = 0; i < StrainSolverData.Count - 1; i++)
                    {
                        if (StrainSolverData[i].Hand == Hand.Left)
                        {
                            //find next left hand
                            for (var j = i + 1; j < StrainSolverData.Count; j++)
                            {
                                if (StrainSolverData[j].Hand == Hand.Left)
                                {
                                    OverallDifficulty += StrainSolverData[i].ActionStrainCoefficient * (StrainSolverData[j].StartTime - StrainSolverData[i].StartTime);
                                    break;
                                }
                            }
                        }
                    }

                    // right hand
                    for (var i = 0; i < StrainSolverData.Count - 1; i++)
                    {
                        if (StrainSolverData[i].Hand == Hand.Right)
                        {
                            //find next left hand
                            for (var j = i + 1; j < StrainSolverData.Count; j++)
                            {
                                if (StrainSolverData[j].Hand == Hand.Right)
                                {
                                    OverallDifficulty += StrainSolverData[i].ActionStrainCoefficient * (StrainSolverData[j].StartTime - StrainSolverData[i].StartTime);
                                    break;
                                }
                            }
                        }
                    }

                    OverallDifficulty /= 2 * Map.Length / rate;
                    break;
                    #endregion

                // 7k difficulty calculation is more longer because we have to solve for the ambiguious hand.
                // to get difficulty, we must calcualte difficulty twice and assigning ambiguious hand is either hand in both calculations.
                case Enums.GameMode.Keys7:
                    #region 7k calc
                    var ambiguiousHandOnLeftDifficulty = 0f;
                    var ambiguiousHandOnRightDifficulty = 0f;

                    // ambiguious hand on left
                    //left hand
                    for (var i = 0; i < StrainSolverData.Count - 1; i++)
                    {
                        if (StrainSolverData[i].Hand == Hand.Left || StrainSolverData[i].Hand == Hand.Ambiguous)
                        {
                            //find next left hand
                            for (var j = i + 1; j < StrainSolverData.Count; j++)
                            {
                                if (StrainSolverData[j].Hand == Hand.Left || StrainSolverData[j].Hand == Hand.Ambiguous)
                                {
                                    ambiguiousHandOnLeftDifficulty += StrainSolverData[i].ActionStrainCoefficient * (StrainSolverData[j].StartTime - StrainSolverData[i].StartTime);
                                    break;
                                }
                            }
                        }
                    }

                    // right hand
                    for (var i = 0; i < StrainSolverData.Count - 1; i++)
                    {
                        if (StrainSolverData[i].Hand == Hand.Right)
                        {
                            //find next left hand
                            for (var j = i + 1; j < StrainSolverData.Count; j++)
                            {
                                if (StrainSolverData[j].Hand == Hand.Right)
                                {
                                    ambiguiousHandOnLeftDifficulty += StrainSolverData[i].ActionStrainCoefficient * (StrainSolverData[j].StartTime - StrainSolverData[i].StartTime);
                                    break;
                                }
                            }
                        }
                    }

                    ambiguiousHandOnLeftDifficulty /= 2 * Map.Length / rate;

                    // ambiguious hand on right
                    //left hand
                    for (var i = 0; i < StrainSolverData.Count - 1; i++)
                    {
                        if (StrainSolverData[i].Hand == Hand.Left)
                        {
                            //find next left hand
                            for (var j = i + 1; j < StrainSolverData.Count; j++)
                            {
                                if (StrainSolverData[j].Hand == Hand.Left)
                                {
                                    ambiguiousHandOnRightDifficulty += StrainSolverData[i].ActionStrainCoefficient * (StrainSolverData[j].StartTime - StrainSolverData[i].StartTime);
                                    break;
                                }
                            }
                        }
                    }

                    // right hand
                    for (var i = 0; i < StrainSolverData.Count - 1; i++)
                    {
                        if (StrainSolverData[i].Hand == Hand.Right || StrainSolverData[i].Hand == Hand.Ambiguous)
                        {
                            //find next left hand
                            for (var j = i + 1; j < StrainSolverData.Count; j++)
                            {
                                if (StrainSolverData[j].Hand == Hand.Right || StrainSolverData[j].Hand == Hand.Ambiguous)
                                {
                                    ambiguiousHandOnRightDifficulty += StrainSolverData[i].ActionStrainCoefficient * (StrainSolverData[j].StartTime - StrainSolverData[i].StartTime);
                                    break;
                                }
                            }
                        }
                    }

                    ambiguiousHandOnRightDifficulty /= 2 * Map.Length / rate;

                    //get 7k diff
                    OverallDifficulty = (ambiguiousHandOnLeftDifficulty + ambiguiousHandOnRightDifficulty) / 2f;

                    break;
                    #endregion
            }

            // calculate stamina (temp solution)
            // it just uses the map's length.
            // 10 seconds = 0.9x multiplier
            // 100 seconds = 1.0x multiplier
            // 1000 seconds = 1.1x multiplier
            // 10000 seconds = 1.2x multiplier, ect.
            OverallDifficulty *= (float)(0.5 + Math.Log10(Map.Length / rate) / 10);
        }

        /// <summary>
        ///     Used to calculate Coefficient for Strain Difficulty
        /// </summary>
        private float GetCoefficientValue(float duration, float xMin, float xMax, float strainMax, float exp)
        {
            //todo: temp. Linear for now
            //todo: apply cosine curve
            const float lowestDifficulty = 1;
            return lowestDifficulty + (strainMax - lowestDifficulty) * (float)Math.Pow(1 - Math.Min(1, Math.Max(0, (duration - xMin) / (xMax - xMin))), exp);
        }
    }
}
