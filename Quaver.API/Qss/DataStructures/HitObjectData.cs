using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss.DataStructures
{
    public class HitObjectData
    {
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public int KeyLane { get; set; }
        public float StrainValue { get; set; }
        public List<HitObjectData> LinkedChordedHitObjects { get; set; }
    }
}
