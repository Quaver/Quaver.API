using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    public class HandStateData
    {
        /// <summary>
        ///     Determines if the HitObject on the same hand of this HitObject will be counted as an object chorded with this HitObject.
        /// </summary>
        public const float CHORD_THRESHOLD_SAMEHAND_MS = 8f;

        /// <summary>
        ///     Determines if the HitObject on the other hand of this HitObject will be counted as an object chorded with this HitObject.
        /// </summary>
        public const float CHORD_THRESHOLD_OTHERHAND_MS = 16f;

        /// <summary>
        /// 
        /// </summary>
        public HandStateData ChordedHand { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HandStateData NextState { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public float Time => HitObjects[0].StartTime;

        /// <summary>
        ///     Determined by the hand this data point focuses on.
        /// </summary>
        public Hand Hand { get; }

        /// <summary>
        ///     Determined by the current state of each finger.
        /// </summary>
        public FingerState FingerState { get; private set; }

        /// <summary>
        ///     Determined by how close the hitobjects are to becoming a perfect chord.
        ///     0 = Perfect Chord
        ///     1 = Furthest from being a perfect chord
        /// </summary>
        public float ChordProximity { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public float StateDifficulty { get; private set; }

        /// <summary>
        ///     All HitObjects referenced for this Hand State
        /// </summary>
        public List<StrainSolverHitObject> HitObjects { get; private set; } = new List<StrainSolverHitObject>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hitObjects"></param>
        public HandStateData(StrainSolverHitObject hitObject)
        {
            Hand = hitObject.Hand;
            AddHitObjectToChord(hitObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hitObjects"></param>
        public void AddHitObjectToChord(StrainSolverHitObject hitObject)
        {
            HitObjects.Add(hitObject);
        }

        /// <summary>
        /// 
        /// </summary>
        public void EvaluateDifficulty(StrainConstantsKeys constants)
        {
            StateDifficulty = 0;
            FingerState = FingerState.None;
            HitObjects.ForEach
            (
                x =>
                {
                    FingerState |= x.FingerState;
                    x.EvaluateDifficulty(constants);
                    StateDifficulty += x.StrainValue;
                }
            );

            StateDifficulty /= HitObjects.Count;

            // Apply Chord Multiplier if Other Hand is chorded with this hand.
            if (ChordedHand != null)
                StateDifficulty *= constants.ChordMultiplier.Value;
        }

        private void CalculateChordProximity()
        {
            // There should not be 0 HitObjects in the Hand State
            if (HitObjects.Count == 0)
                throw new Exception("HitObject List for HandStateData has no HitObjects");

            // Set Finger State to appropriate value and calculate ChordProximity
            var lead = HitObjects[0];
            StrainSolverHitObject furthest = null;
            HitObjects = HitObjects;
            foreach (var ob in HitObjects)
            {
                FingerState |= ob.FingerState;
                if (StrainSolverKeys.LaneToHand4K[ob.HitObject.Lane] != Hand)
                {
                    if (furthest == null || ob.HitObject.StartTime > furthest.HitObject.StartTime)
                    {
                        furthest = ob;
                    }
                }
            }

            // Calculate Chord Proximity from furthest chord
            ChordProximity = furthest == null ? 0 : Math.Abs(furthest.HitObject.StartTime - lead.HitObject.StartTime) / CHORD_THRESHOLD_OTHERHAND_MS;
        }

        /// <summary>
        ///     Get the chord type of the current hand state.
        ///     ChordType is determined by the number of HitObjects in this state.
        /// </summary>
        private ChordType EvaluateChord()
        {
            var otherHand = ChordedHand == null ? 0 : ChordedHand.HitObjects.Count;
            switch (HitObjects.Count + otherHand)
            {
                case 0:
                    return ChordType.None;
                case 1:
                    return ChordType.SingleTap;
                case 2:
                    return ChordType.JumpChord;
                case 3:
                    return ChordType.HandChord;
                case 4:
                    return ChordType.QuadChord;
                default:
                    return ChordType.NChord;
            }
        }
    }
}
