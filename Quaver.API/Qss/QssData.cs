using Quaver.API.Qss.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss
{
    class QssData
    {
        internal float OverallDifficulty { get; set; }
        internal float AverageNoteDensity { get; set; }
        internal List<HitObjectData> HitObjects {get; set;}
        internal List<GraphData> NoteDensityData { get; set; }
        internal List<GraphData> StrainValueData { get; set; }
    }
}
