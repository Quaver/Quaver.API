using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    public class HandStateData
    {
        /// <summary>
        /// 
        /// </summary>
        public FingerState HandState { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ChordType ChordType { get; private set; }

        /// <summary>
        ///     Determined by how close the hitobjects are to becoming a chord
        /// </summary>
        public float ChordProximity { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<StrainSolverHitObject> HitObjects { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hitObjects"></param>
        public HandStateData(List<StrainSolverHitObject> hitObjects)
        {
            HitObjects = hitObjects;
            foreach (var ob in hitObjects)
            {
                HandState |= ob.FingerState;
            }
        }
    }
}
