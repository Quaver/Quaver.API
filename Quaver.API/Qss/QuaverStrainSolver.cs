using Quaver.API.Maps;
using Quaver.API.Qss.DataStructures;
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
        /// Compute and returns Qss Data for a map
        /// </summary>
        /// <param name="qua"></param>
        /// <returns>Qss Data</returns>
        public static QssData GetQssData(Qua qua)
        {
            QssData qssData = new QssData();

            ComputeNoteDensityData(qssData, qua);
            ComputeBaseStrainValues(qssData, qua);
            ComputeFingerStates(qssData);
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

            GraphData densityData;
            for (var i = 0; i < qua.HitObjects.Count; i ++)
            {
                densityData = new GraphData()
                {
                    StartTime = qua.HitObjects[i].StartTime
                };
            }
        }

        /// <summary>
        /// Get Note Data, and compute the base strain weights
        /// </summary>
        /// <param name="qssData"></param>
        /// <param name="qua"></param>
        private static void ComputeBaseStrainValues(QssData qssData, Qua qua)
        {
            // Compute LN Weights
        }

        /// <summary>
        /// Computes the finger states for every note in a given beatmap
        /// </summary>
        /// <param name="qssData"></param>
        private static void ComputeFingerStates(QssData qssData)
        {

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
