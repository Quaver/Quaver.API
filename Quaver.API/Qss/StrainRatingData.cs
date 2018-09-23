using Quaver.API.Maps;
using Quaver.API.Qss.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss
{
    /// <summary>
    ///     Will be used to solve Strain Rating.
    /// </summary>
    public class StrainRatingData
    {
        /// <summary>
        /// Size of each graph partition in miliseconds
        /// </summary>
        public const int GRAPH_INTERVAL_SIZE_MS = 500;

        /// <summary>
        /// Offset between each graph partition in miliseconds
        /// </summary>
        public const int GRAPH_INTERVAL_OFFSET_MS = 100;

        /// <summary>
        /// Total ammount of milliseconds in a second.
        /// </summary>
        public const float SECONDS_TO_MILLISECONDS = 1000;

        /// <summary>
        /// Max Lane Count. Will not check hit object indexes below the current index value subtracted by this value.
        /// </summary>
        public const int MAX_LANE_CHECK = 7;

        /// <summary>
        /// Map that will be referenced for calculation
        /// </summary>
        public Qua Qua { get; private set; }

        /// <summary>
        /// If 2 hit objects are within miliseconds apart, they will be considered a chorded pair.
        /// </summary>
        public const int CHORD_THRESHOLD_MS = 10;

        /// <summary>
        /// Overall difficulty of the map
        /// </summary>
        public float OverallDifficulty { get; private set; } = 0;

        /// <summary>
        /// Average note density of the map
        /// </summary>
        public float AverageNoteDensity { get; private set; } = 0;

        /// <summary>
        /// Hit objects in the map used for solving difficulty
        /// </summary>
        public List<HitObjectData> HitObjects { get; private set; } = new List<HitObjectData>();

        /// <summary>
        /// Hit objects that will be played with the left hand
        /// </summary>
        public List<HitObjectData> LeftHandObjects { get; private set; } = new List<HitObjectData>();

        /// <summary>
        /// Hit objects that will be played with the right hand
        /// </summary>
        public List<HitObjectData> RightHandObjects { get; private set; } = new List<HitObjectData>();

        /// <summary>
        /// Hit objects that can be played with either right or left hand. Used for even keyed keymodes (5K/7K)
        /// </summary>
        public List<HitObjectData> AmbiguousHandObjects { get; private set; } = new List<HitObjectData>();

        /// <summary>
        /// Assumes that the assigned hand will be the one to press that key
        /// </summary>
        private Dictionary<int, Hand> LaneToHand4K { get; set; } = new Dictionary<int, Hand>()
        {
            { 1, Hand.Left },
            { 2, Hand.Left },
            { 3, Hand.Right },
            { 4, Hand.Right }
        };

        /// <summary>
        /// Assumes that the assigned hand will be the one to press that key
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
        /// Assumes that the assigned finger will be the one to press that key.
        /// </summary>
        private Dictionary<int, FingerState> LaneToFinger4K { get; set; } = new Dictionary<int, FingerState>()
        {
            { 1, FingerState.Middle },
            { 2, FingerState.Index },
            { 3, FingerState.Index },
            { 4, FingerState.Middle }
        };

        /// <summary>
        /// Assumes that the assigned finger will be the one to press that key.
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
        public StrainRatingData(Qua qua)
        {
            Qua = qua;
            ComputeNoteDensityData();
            ComputeBaseStrainStates();
            //ComputeForChords();
            ComputeFingerActions();
            ComputeActionPatterns();
            CalculateOverallDifficulty();
        }

        /// <summary>
        /// Compute and generate Note Density Data.
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
        /// Get Note Data, and compute the base strain weights
        /// The base strain weights are affected by LN layering
        /// </summary>
        /// <param name="qssData"></param>
        /// <param name="qua"></param>
        private void ComputeBaseStrainStates()
        {
            // Add hit objects from qua map to qssData
            HitObjectData hitObjectData;
            for (var i = 0; i < Qua.HitObjects.Count; i++)
            {
                hitObjectData = new HitObjectData()
                {
                    StartTime = Qua.HitObjects[i].StartTime,
                    EndTime = Qua.HitObjects[i].EndTime,
                    Lane = Qua.HitObjects[i].Lane
                };

                // Calculate LN Multiplier (note: doesn't check for same hand
                // Also look for chords
                for (var j = i - MAX_LANE_CHECK < 0 ? 0 : i - MAX_LANE_CHECK; j < Qua.HitObjects.Count; j++)
                {
                    if (Qua.HitObjects[j].StartTime > hitObjectData.EndTime)
                    {
                        break;
                    }
                    else if (Qua.HitObjects[j].StartTime > hitObjectData.StartTime)
                    {
                        // Target hitobject's LN ends after current hitobject's LN
                        if (Qua.HitObjects[j].EndTime > hitObjectData.EndTime)
                        {
                            hitObjectData.LnStrainMultiplier *= 1.2f; //TEMP STRAIN MULTIPLIER. use constant later.
                        }
                        // Target hitobject's LN ends before current hitobject's LN
                        else if (Qua.HitObjects[j].EndTime > 0)
                        {
                            hitObjectData.LnStrainMultiplier *= 1.2f; //TEMP STRAIN MULTIPLIER. use constant later.
                        }
                        // Target hitobject is not an LN
                        else
                        {
                            hitObjectData.LnStrainMultiplier *= 1.2f; //TEMP STRAIN MULTIPLIER. use constant later.
                        }
                    }
                }

                // Assign Finger and Hand States
                switch (Qua.Mode)
                {
                    case Enums.GameMode.Keys4:
                        hitObjectData.FingerState = LaneToFinger4K[hitObjectData.Lane];
                        hitObjectData.Hand = LaneToHand4K[hitObjectData.Lane];
                        break;
                    case Enums.GameMode.Keys7:
                        hitObjectData.FingerState = LaneToFinger7K[hitObjectData.Lane];
                        hitObjectData.Hand = LaneToHand7K[hitObjectData.Lane];
                        break;
                }

                HitObjects.Add(hitObjectData);
            }
        }

        private void ComputeForChords()
        {
            // search through whole hit object list and find chords
            /*
            float msDiff;
            for (var i = 0; i < Qua.HitObjects.Count; i++)
            {
                for (var j = i - MAX_LANE_CHECK < 0 ? 0 : i - MAX_LANE_CHECK; j < Qua.HitObjects.Count; j++)
                {
                    msDiff = Qua.HitObjects[j].StartTime - Qua.HitObjects[i].StartTime;
                    if (msDiff < CHORD_THRESHOLD_MS)
                    {
                        if (msDiff > -CHORD_THRESHOLD_MS)
                        {
                            HitObjects[i].LinkedChordedHitObjects.Add(HitObjects[j]);
                        }
                    }

                    // Stop the loop if target hitobject's starttime is above the threshold
                    else
                    {
                        break;
                    }
                }
            }
            */
        }

        /// <summary>
        /// Scans every finger state, and determines its action (JACK/TRILL/TECH, ect).
        /// Action-Strain multiplier is applied in computation.
        /// </summary>
        /// <param name="qssData"></param>
        private void ComputeFingerActions()
        {

        }

        /// <summary>
        /// Scans every finger action and compute a pattern multiplier.
        /// Pattern manipulation, and inflated patterns are factored into calculation.
        /// </summary>
        /// <param name="qssData"></param>
        private void ComputeActionPatterns()
        {

        }

        /// <summary>
        /// Calculates the general difficulty of a beatmap
        /// </summary>
        /// <param name="qssData"></param>
        private void CalculateOverallDifficulty()
        {
            //todo: temporary.
            OverallDifficulty = AverageNoteDensity * 3.25f;
        }
    }
}
