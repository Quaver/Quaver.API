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
        public const float CHORD_THRESHOLD_OTHERHAND_MS = 32f;

        /// <summary>
        ///     Multiplier that will be applied to chords if other hand is involved.
        /// </summary>
        public const float CHORD_MULTIPLIER = 0.88f;

        /// <summary>
        ///     Determined by the hand this data point focuses on.
        /// </summary>
        public Hand Hand { get; private set; }

        /// <summary>
        ///     Determined by the current state of each finger.
        /// </summary>
        public FingerState FingerState { get; private set; }

        /// <summary>
        ///     Determined by how many HitObjects are in this current state.
        /// </summary>
        public ChordType ChordType { get; private set; }

        /// <summary>
        ///     Determined by how close the hitobjects are to becoming a perfect chord.
        ///     0 = Perfect Chord
        ///     1 = Furthest from being a perfect chord
        /// </summary>
        public float ChordProximity { get; private set; }

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
            AddHitObjectToChord(hitObject);
            /*
            if (hitObjects.Count == 0)
            {
                throw new Exception("HitObject List for HandStateData has no HitObjects");
            }

            // Set Finger State to appropriate value and calculate ChordProximity
            var lead = hitObjects[0];
            StrainSolverHitObject furthest = null;
            HitObjects = hitObjects;
            foreach (var ob in hitObjects)
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
            ChordProximity = furthest == null
                ? 1
                : Math.Abs(furthest.HitObject.StartTime - lead.HitObject.StartTime) / CHORD_THRESHOLD_OTHERHAND_MS;

            // Set ChordType to appropriate value
            switch (hitObjects.Count)
            {
                case 0:
                    ChordType = ChordType.None;
                    return;
                case 1:
                    ChordType = ChordType.SingleTap;
                    return;
                case 2:
                    ChordType = ChordType.JumpChord;
                    return;
                case 3:
                    ChordType = ChordType.HandChord;
                    return;
                case 4:
                    ChordType = ChordType.QuadChord;
                    return;
                default:
                    ChordType = ChordType.NChord;
                    return;
            }*/
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
        public void EvaluateChord()
        {

        }

        /// <summary>
        ///     Evaluate Difficulty of each HitObject
        /// </summary>
        public void CalculateDifficulty()
        {
            foreach(var ob in HitObjects)
            {
                ob.ChordMultiplier = CHORD_MULTIPLIER + (ChordProximity * (1 - CHORD_MULTIPLIER));
                ob.LnStrainMultiplier = 1; // TODO: temporary
                ob.StrainValue = ob.ChordMultiplier * ob.LnStrainMultiplier;
                //ob.StrainValue = 
            }
        }
    }
}
