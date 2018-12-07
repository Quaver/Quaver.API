using Quaver.API.Enums;
using Quaver.API.Maps.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    /// <summary>
    ///     An expanded version of hit object that is used for QSS implementations in API/editor/calculation
    /// </summary>
    public class StrainSolverHitObject
    {
        /// <summary>
        ///     The HitObject this class is referencing
        /// </summary>
        public HitObjectInfo HitObject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public WristState WristState { get; set; }

        /// <summary>
        ///     Current Finger State of the hit object
        /// </summary>
        public FingerState FingerState { get; set; } = FingerState.None;

        /// <summary>
        ///     Current type of layering relating to LN
        /// </summary>
        public LnLayerType LnLayerType { get; set; } = LnLayerType.None;

        /// <summary>
        ///     Strain Multiplier calculated by LN difficulty
        /// </summary>
        public float LnStrainMultiplier { get; set; } = 1;

        /// <summary>
        ///     Strain Multiplier affected by chorded HitObjects
        /// </summary>
        public float ChordMultiplier { get; set; } = 1;

        /// <summary>
        ///     Current Strain Value for this hit object.
        /// </summary>
        public float StrainValue { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="hitOb"></param>
        public StrainSolverHitObject(HitObjectInfo hitOb, GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Keys4:
                    FingerState = StrainSolverKeys.LaneToFinger4K[hitOb.Lane];
                    break;
                case GameMode.Keys7:
                    FingerState = StrainSolverKeys.LaneToFinger7K[hitOb.Lane];
                    break;
                default:
                    throw new Exception("Invalid GameMode used to create StrainSolverHitObject");
                    break;
            }
            HitObject = hitOb;
        }

        /// <summary>
        /// 
        /// </summary>
        public void EvaluateDifficulty()
        {
            StrainValue = 1;
            if (WristState == null)
            {
                StrainValue = 100000000;
            }
        }
    }
}
