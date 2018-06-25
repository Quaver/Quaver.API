using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss.DataStructures
{
    class HitObjectData
    {
        internal float StartTime { get; set; }
        internal float EndTime { get; set; }
        internal int KeyLane { get; set; }
        internal float StrainValue { get; set; }
        internal List<HitObjectData> LinkedChordedHitObjects { get; set; }
    }
}
