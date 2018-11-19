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
        ///     Determined by how close the hitobjects are to becoming a perfect chord
        /// </summary>
        public float ChordProximity { get; private set; }

        /// <summary>
        ///     All HitObjects referenced for this Hand State
        /// </summary>
        public List<StrainSolverHitObject> HitObjects { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hitObjects"></param>
        public HandStateData(List<StrainSolverHitObject> hitObjects)
        {
            // Set Finger State to appropriate value and calculate ChordProximity
            HitObjects = hitObjects;
            foreach (var ob in hitObjects)
            {
                FingerState |= ob.FingerState;
            }

            //todo: calculate chord proximity

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
            }
        }
    }
}
