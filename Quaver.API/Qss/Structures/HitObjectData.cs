using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss.Structures
{
    public class HitObjectData
    {
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public int Lane { get; set; }

        public float LnStrainMultiplier { get; set; } = 1;
        public float ActionStrainMultiplier { get; set; } = 1;
        public float PatternStrainMultiplier { get; set; } = 1;
        public float ManipulationStrainMultiplier { get; set; } = 1;
        public float StrainValue { get; set; } = 0;
        public Hand Hand { get; set; }
        public FingerState FingerState { get; set; }
        public FingerAction FingerAction { get; set; }
        public string Pattern { get; set; } //todo: replace with enum
        public List<HitObjectData> LinkedChordedHitObjects { get; set; } = new List<HitObjectData>();
        public bool HandChord { get; set; }
        public int HandChordStateIndex { get; set; } = 0;
    }
}
