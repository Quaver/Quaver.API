using Quaver.API.Maps.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    public class StrainSolverData
    {
        public List<StrainSolverHitObject> HitObjects { get; set; } = new List<StrainSolverHitObject>();

        public float StartTime { get; set; }
        public float EndTime { get; set; }

        public float ActionStrainCoefficient { get; set; } = 1;
        public float PatternStrainMultiplier { get; set; } = 1;
        public float ManipulationStrainMultiplier { get; set; } = 1;
        public float TotalStrainValue { get; set; } = 0;

        public Hand Hand { get; set; }
        public FingerAction FingerAction { get; set; }
        public string Pattern { get; set; } //todo: replace with enum
        public bool HandChord => HitObjects.Count > 1;
        public int HandChordState { get; set; } = 0;

        public StrainSolverData(StrainSolverHitObject hitOb, float rate = 1)
        {
            StartTime = hitOb.HitObject.StartTime / rate;
            EndTime = hitOb.HitObject.EndTime / rate;
            HitObjects.Add(hitOb);
        }
    }
}
