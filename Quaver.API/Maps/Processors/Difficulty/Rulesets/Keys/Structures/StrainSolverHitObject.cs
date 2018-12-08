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
        /// 
        /// </summary>
        public Hand Hand { get; set; } = Hand.Ambiguous; //todo: implement differently

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
        ///     When the Hit Object Starts relative to the song in milliseconds.
        /// </summary>
        public float StartTime => HitObject.StartTime;

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
                    Hand = StrainSolverKeys.LaneToHand4K[hitOb.Lane];
                    break;
                case GameMode.Keys7:
                    FingerState = StrainSolverKeys.LaneToFinger7K[hitOb.Lane];
                    Hand = StrainSolverKeys.LaneToHand7K[hitOb.Lane];
                    break;
                default:
                    throw new Exception("Invalid GameMode used to create StrainSolverHitObject");
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
                StrainValue = 1.25f;
            }
            else if (WristState.NextState != null)
            {
                if (WristState.NextState.WristPair.Equals(WristState.WristPair))
                {
                    if (WristState.NextState.Time - WristState.Time < 94)
                    {
                        StrainValue = 0.25f;
                    }
                    else if (WristState.NextState.Time - WristState.Time < 100)
                    {
                        StrainValue = 0.5f;
                    }
                    else
                    {
                        StrainValue = 1;
                    }
                    //WristState.WristDifficulty = 0;
                    //WristState.WristDifficulty = 1 - (float)(0.75*Math.Pow(Math.Max(95 - (WristState.NextState.Time - WristState.Time), 0) / 95, 0.25f));
                    //StrainValue = WristState.WristDifficulty;
                }
                else
                {
                    StrainValue = 1.1f;
                }
            }
        }
    }
}
