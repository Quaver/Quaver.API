using Quaver.API.Maps.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss.Structures
{
    public class StrainSolverData
    {
        public List<StrainSolverHitObject> HitObjects { get; set; } = new List<StrainSolverHitObject>();

        public float StartTime { get; set; }
        public float EndTime { get; set; }

        public float ActionStrainMultiplier { get; set; } = 1;
        public float PatternStrainMultiplier { get; set; } = 1;
        public float ManipulationStrainMultiplier { get; set; } = 1;
        public float StrainValue { get; set; } = 0;

        public Hand Hand { get; set; }
        public FingerAction FingerAction { get; set; }
        public string Pattern { get; set; } //todo: replace with enum
        public bool HandChord => HitObjects.Count > 1;
        public int HandChordState { get; set; } = 0;

        public StrainSolverData(StrainSolverHitObject hitOb)
        {
            StartTime = hitOb.HitObject.StartTime;
            EndTime = hitOb.HitObject.EndTime;
            HitObjects.Add(hitOb);
        }
    }
}
