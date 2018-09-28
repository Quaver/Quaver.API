using Quaver.API.Maps.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    /// <summary>
    ///     Data point that is represented by a group of hitobjects.
    ///     Used for calculating strain value at a given time for a given hand.
    /// </summary>
    public class StrainSolverData
    {
        /// <summary>
        ///     Chorded Hit Objects at the current start time
        /// </summary>
        public List<StrainSolverHitObject> HitObjects { get; set; } = new List<StrainSolverHitObject>();

        /// <summary>
        ///     When the current action/pattern starts
        /// </summary>
        public float StartTime { get; set; }

        /// <summary>
        ///     When the longest LN in self.HitObjects ends
        /// </summary>
        public float EndTime { get; set; }

        /// <summary>
        ///     Base strain value calculated by its action
        /// </summary>
        public float ActionStrainCoefficient { get; set; } = 1;

        /// <summary>
        ///     Strain multiplier determined by pattern difficulty
        /// </summary>
        public float PatternStrainMultiplier { get; set; } = 1;

        /// <summary>
        ///     Multiplier that gets added to any pattern that could be manipulated
        ///     Note: Manipulation in this case means being able to "chord/hand spam multiple keys"
        /// </summary>
        public float ManipulationStrainMultiplier { get; set; } = 1;

        /// <summary>
        ///     Total strain value for this data point
        /// </summary>
        public float TotalStrainValue { get; private set; } = 0;

        /// <summary>
        ///     Hand that this data point represents
        /// </summary>
        public Hand Hand { get; set; }

        /// <summary>
        ///     Finger Action that this data point represents
        /// </summary>
        public FingerAction FingerAction { get; set; }

        /// <summary>
        ///     Pattern that this data point represents.
        ///     TODO: replace with enum
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        ///     Is determined by if this data point has more than one hit object (per hand)
        /// </summary>
        public bool HandChord => HitObjects.Count > 1;

        /// <summary>
        ///     Is an index value of this hand's finger state. (Determined by every finger's state)
        /// </summary>
        public int HandChordState { get; private set; } = 0;

        /// <summary>
        ///     Data used to represent a point in time and other variables that influence difficulty.
        /// </summary>
        /// <param name="hitOb"></param>
        /// <param name="rate"></param>
        public StrainSolverData(StrainSolverHitObject hitOb, float rate = 1)
        {
            StartTime = hitOb.HitObject.StartTime / rate;
            EndTime = hitOb.HitObject.EndTime / rate;
            HitObjects.Add(hitOb);
        }

        /// <summary>
        ///     Calculate the strain value of this current point.
        /// </summary>
        public void CalculateStrainValue()
        {
            // Calculate the strain value of each individual object and add to total
            foreach (var hitOb in HitObjects)
            {
                hitOb.StrainValue = ActionStrainCoefficient * PatternStrainMultiplier * ManipulationStrainMultiplier * hitOb.LnStrainMultiplier;
                TotalStrainValue += hitOb.StrainValue;
            }

            // Average the strain value between the two objects
            TotalStrainValue /= HitObjects.Count;
        }

        public void SolveFingerState()
        {
            HandChordState = 0;
            foreach (var hitOb in HitObjects) HandChordState += (int)hitOb.FingerState;
        }
    }
}
