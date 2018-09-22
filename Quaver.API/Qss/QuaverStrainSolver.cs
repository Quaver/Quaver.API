using Quaver.API.Maps;
using Quaver.API.Qss.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss
{
    public class QuaverStrainSolver
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
        /// Max Lane Count. Will not check hit object indexes below the current index value subtracted by this value.
        /// </summary>
        public const int MAX_LANE_CHECK = 7;

        /// <summary>
        /// If 2 hit objects are within miliseconds apart, they will be considered a chorded pair.
        /// </summary>
        public const int CHORD_THRESHOLD_MS = 10;

        /// <summary>
        /// Compute and returns Qss Data for a map
        /// </summary>
        /// <param name="qua"></param>
        /// <returns>Qss Data</returns>
        public static QssData GetQssData(Qua qua)
        {
            var qssData = new QssData();

            ComputeNoteDensityData(qssData, qua);
            ComputeBaseStrainStates(qssData, qua);
            ComputeForChords(qssData);
            ComputeFingerActions(qssData);
            ComputeActionPatterns(qssData);
            CalculateOverallDifficulty(qssData);

            return qssData;
        }

        /// <summary>
        /// Compute and generate Note Density Data.
        /// </summary>
        /// <param name="qssData"></param>
        /// <param name="qua"></param>
        private static void ComputeNoteDensityData(QssData qssData, Qua qua)
        {
            qssData.MapLength = qua.Length;

        }

        /// <summary>
        /// Get Note Data, and compute the base strain weights
        /// The base strain weights are affected by LN layering
        /// </summary>
        /// <param name="qssData"></param>
        /// <param name="qua"></param>
        private static void ComputeBaseStrainStates(QssData qssData, Qua qua)
        {
            qssData.MapLength = qua.Length;

            // Add hit objects from qua map to qssData
            HitObjectData hitObjectData;
            for (var i = 0; i < qua.HitObjects.Count; i++)
            {
                hitObjectData = new HitObjectData()
                {
                    StartTime = qua.HitObjects[i].StartTime,
                    EndTime = qua.HitObjects[i].EndTime,
                    Lane = qua.HitObjects[i].Lane
                };

                // Calculate LN Multiplier (note: doesn't check for same hand
                // Also look for chords
                for (var j = i - MAX_LANE_CHECK < 0 ? 0 : i - MAX_LANE_CHECK; j < qua.HitObjects.Count; j++)
                {
                    if (qua.HitObjects[j].StartTime > hitObjectData.EndTime)
                    {
                        break;
                    }
                    else if (qua.HitObjects[j].StartTime > hitObjectData.StartTime)
                    {
                        // Target hitobject's LN ends after current hitobject's LN
                        if (qua.HitObjects[j].EndTime > hitObjectData.EndTime)
                        {
                            hitObjectData.LnStrainMultiplier *= 1.2f; //TEMP STRAIN MULTIPLIER. use constant later.
                        }
                        // Target hitobject's LN ends before current hitobject's LN
                        else if (qua.HitObjects[j].EndTime > 0)
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

                // Assign Finger States
                // Mania 4k
                if (qua.Mode == Enums.GameMode.Keys4)
                {
                    switch (hitObjectData.Lane)
                    {
                        case 1:
                            hitObjectData.FingerState = FingerState.Middle;
                            hitObjectData.Hand = Hand.Left;
                            break;
                        case 2:
                            hitObjectData.FingerState = FingerState.Index;
                            hitObjectData.Hand = Hand.Left;
                            break;
                        case 3:
                            hitObjectData.FingerState = FingerState.Index;
                            hitObjectData.Hand = Hand.Right;
                            break;
                        case 4:
                            hitObjectData.FingerState = FingerState.Middle;
                            hitObjectData.Hand = Hand.Right;
                            break;
                        default:
                            break;
                    }
                }

                // Mania 7k
                else if (qua.Mode == Enums.GameMode.Keys7)
                {
                    switch (hitObjectData.Lane)
                    {
                        case 1:
                            hitObjectData.FingerState = FingerState.Ring;
                            hitObjectData.Hand = Hand.Left;
                            break;
                        case 2:
                            hitObjectData.FingerState = FingerState.Middle;
                            hitObjectData.Hand = Hand.Left;
                            break;
                        case 3:
                            hitObjectData.FingerState = FingerState.Index;
                            hitObjectData.Hand = Hand.Left;
                            break;
                        case 4:
                            hitObjectData.FingerState = FingerState.Thumb;
                            hitObjectData.Hand = Hand.Ambiguous;
                            break;
                        case 5:
                            hitObjectData.FingerState = FingerState.Index;
                            hitObjectData.Hand = Hand.Right;
                            break;
                        case 6:
                            hitObjectData.FingerState = FingerState.Middle;
                            hitObjectData.Hand = Hand.Right;
                            break;
                        case 7:
                            hitObjectData.FingerState = FingerState.Ring;
                            hitObjectData.Hand = Hand.Right;
                            break;
                        default:
                            break;
                    }
                }

                qssData.HitObjects.Add(hitObjectData);
            }
        }

        private static void ComputeForChords(QssData qssData)
        {
            float msDiff;
            for (var i = 0; i < qssData.HitObjects.Count; i++)
            {
                for (var j = i - MAX_LANE_CHECK < 0 ? 0 : i - MAX_LANE_CHECK; j < qssData.HitObjects.Count; j++)
                {
                    msDiff = qssData.HitObjects[j].StartTime - qssData.HitObjects[i].StartTime;
                    if (msDiff < CHORD_THRESHOLD_MS)
                    {
                        if (msDiff > -CHORD_THRESHOLD_MS)
                        {
                            qssData.HitObjects[i].LinkedChordedHitObjects.Add(qssData.HitObjects[j]);
                        }
                    }

                    // Stop the loop if target hitobject's starttime is above the threshold
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Scans every finger state, and determines its action (JACK/TRILL/TECH, ect).
        /// Action-Strain multiplier is applied in computation.
        /// </summary>
        /// <param name="qssData"></param>
        private static void ComputeFingerActions(QssData qssData)
        {

        }

        /// <summary>
        /// Scans every finger action and compute a pattern multiplier.
        /// Pattern manipulation, and inflated patterns are factored into calculation.
        /// </summary>
        /// <param name="qssData"></param>
        private static void ComputeActionPatterns(QssData qssData)
        {

        }

        /// <summary>
        /// Calculates the general difficulty of a beatmap
        /// </summary>
        /// <param name="qssData"></param>
        private static void CalculateOverallDifficulty(QssData qssData)
        {
            qssData.OverallDifficulty = 0;
        }
    }
}
