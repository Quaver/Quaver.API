using Quaver.API.Maps;
using Quaver.API.Maps.Processors.Difficulty.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Maps.Processors.Difficulty
{
    /// <summary>
    ///     Will be used to solve Strain Rating.
    /// </summary>
    public class StrainSolverKeys : StrainSolver
    {
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
        ///     Map that will be referenced for calculation
        /// </summary>
        public Qua Qua { get; private set; }

        /// <summary>
        ///     Overall difficulty of the map
        /// </summary>
        public float OverallDifficulty { get; private set; } = 0;

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
        public StrainSolverKeys(Qua qua)
        {
            // Assign reference qua
            Qua = qua;

            // Don't bother calculating map difficulty if there's less than 2 hit objects
            if (Qua.HitObjects.Count < 2) return;

            // Solve for difficulty
            ComputeNoteDensityData();
            ComputeBaseStrainStates();
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
        private void ComputeNoteDensityData()
        {
            //MapLength = Qua.Length;
            AverageNoteDensity = SECONDS_TO_MILLISECONDS * Qua.HitObjects.Count / Qua.Length;

            //todo: solve note density graph
        }

        /// <summary>
        ///     Get Note Data, and compute the base strain weights
        ///     The base strain weights are affected by LN layering
        /// </summary>
        /// <param name="qssData"></param>
        /// <param name="qua"></param>
        private void ComputeBaseStrainStates()
        {
            // Add hit objects from qua map to qssData
            for (var i = 0; i < Qua.HitObjects.Count; i++)
            {
                var curHitOb = new StrainSolverHitObject(Qua.HitObjects[i]);
                var curStrainData = new StrainSolverData(curHitOb);

                // Assign Finger and Hand States
                switch (Qua.Mode)
                {
                    case Enums.GameMode.Keys4:
                        curHitOb.FingerState = LaneToFinger4K[Qua.HitObjects[i].Lane];
                        curStrainData.Hand = LaneToHand4K[Qua.HitObjects[i].Lane];
                        break;
                    case Enums.GameMode.Keys7:
                        curHitOb.FingerState = LaneToFinger7K[Qua.HitObjects[i].Lane];
                        curStrainData.Hand = LaneToHand7K[Qua.HitObjects[i].Lane];
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
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 20, 560, 51, 0.91f); // todo: temp. Apply actual constants later
                            Roll++;
                        }

                        // Simple Jack
                        else if (actionSameState)
                        {
                            curHitOb.FingerAction = FingerAction.SimpleJack;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 50, 560, 55, 0.86f); // todo: temp. Apply actual constants later
                            SJack++;
                        }

                        // Tech Jack
                        else if (actionJackFound)
                        {
                            curHitOb.FingerAction = FingerAction.TechnicalJack;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 30, 560, 54, 0.85f); // todo: temp. Apply actual constants later
                            TJack++;
                        }

                        // Bracket
                        else
                        {
                            curHitOb.FingerAction = FingerAction.Bracket;
                            curHitOb.ActionStrainCoefficient = GetCoefficientValue(actionDuration, 20, 560, 55, 0.82f); // todo: temp. Apply actual constants later
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
        private void CalculateOverallDifficulty()
        {
            //todo: temporary.
            //OverallDifficulty = AverageNoteDensity * 3.25f;
            foreach (var i in StrainSolverData)
            {
                OverallDifficulty += i.ActionStrainCoefficient;
            }
            OverallDifficulty /= StrainSolverData.Count;
        }

        /// <summary>
        ///     Used to calculate Coefficient for Strain Difficulty
        /// </summary>
        private float GetCoefficientValue(float duration, float xMin, float xMax, float strainMax, float exp)
        {
            //todo: temp. Linear for now
            //todo: apply cosine curve
            return 1 + (strainMax - 1) * (float)Math.Pow((1 - Math.Min(Math.Max(0, duration - xMin)/(xMax - xMin), 1)), exp);
        }
    }
}
