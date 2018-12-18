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
        public Hand Hand { get; set; }

        /// <summary>
        ///     Strain Multiplier affected by chorded HitObjects
        /// </summary>
        public float ChordMultiplier { get; set; } = 1;

        /// <summary>
        ///     Determined by how repetitive the jack action is (if a jack follows up.)
        /// </summary>
        public float RepetitionMultiplier { get; set; } = 1;

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
        public StrainSolverHitObject(HitObjectInfo hitOb, GameMode mode, Hand assumeHand)
        {
            HitObject = hitOb;

            // Determine Hand of this hit object
            switch (mode)
            {
                case GameMode.Keys4:
                    FingerState = StrainSolverKeys.LaneToFinger4K[hitOb.Lane];
                    Hand = StrainSolverKeys.LaneToHand4K[hitOb.Lane];
                    break;
                case GameMode.Keys7:
                    FingerState = StrainSolverKeys.LaneToFinger7K[hitOb.Lane];
                    Hand = StrainSolverKeys.LaneToHand7K[hitOb.Lane].Equals(Hand.Ambiguous) ? assumeHand : StrainSolverKeys.LaneToHand7K[hitOb.Lane];
                    break;
                default:
                    throw new Exception("Invalid GameMode used to create StrainSolverHitObject");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void EvaluateDifficulty(StrainConstantsKeys constants)
        {
            StrainValue = 1;
            if (WristState == null)
            {
                StrainValue = constants.WristTechMultiplier.Value;
            }
            else if (WristState.NextState != null)
            {
                if (WristState.NextState.WristPair.Equals(WristState.WristPair))
                {
                    if (WristState.NextState.NextState != null)
                    {
                        var delta = Math.Abs(WristState.NextState.NextStateDelta - WristState.NextStateDelta);
                        if (delta < constants.JackSigmaThresholdMs)
                        {
                            if (WristState.NextStateDelta < constants.VibroActionTolerance)
                            {
                                WristState.WristDifficulty = WristState.NextState.WristDifficulty * constants.WristVibroMultiplier.Value;
                            }
                            else if (WristState.NextStateDelta >= constants.VibroActionThreshold)
                            {
                                WristState.WristDifficulty = (constants.WristSimpleJackMultiplier.Value + WristState.NextState.WristDifficulty) / 2;
                            }
                            else
                            {
                                var diff = constants.WristSimpleJackMultiplier.Value - constants.WristVibroMultiplier.Value;
                                var interval = constants.VibroActionThreshold - constants.VibroActionTolerance;
                                WristState.WristDifficulty = WristState.NextState.WristDifficulty + diff * (delta - constants.VibroActionTolerance) / interval;
                            }
                        }
                        else
                        {
                            WristState.WristDifficulty = constants.WristGapMultiplier.Value;
                        }
                    }

                    DetermineRepetition(constants);
                    StrainValue = WristState.WristDifficulty;
                }
                else
                {
                    StrainValue = 1;
                }
            }
        }

        /// <summary>
        ///     Determine and Apply Repetition Multiplier to Simple Jacks
        /// </summary>
        /// <param name="constants"></param>
        private void DetermineRepetition(StrainConstantsKeys constants)
        {
            if (WristState.NextState != null && WristState.NextState.WristPair.Equals(WristState.WristPair))
                WristState.RepetitionCount = WristState.NextState.RepetitionCount + 1;

            RepetitionMultiplier = (float)Math.Pow(constants.WristRepetitionMultiplier.Value, Math.Min(WristState.RepetitionCount, constants.MaxSimpleJackRepetition));
            WristState.WristDifficulty *= RepetitionMultiplier;
        }
    }
}
