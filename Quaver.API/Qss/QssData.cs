using Quaver.API.Qss.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss
{
    public class QssData
    {
        public float MapLength { get; set; }
        public float OverallDifficulty { get; set; }
        public float AverageNoteDensity { get; set; }
        public List<HitObjectData> HitObjects {get; set; }
        public List<GraphData> NoteDensityData { get; set; }
        public List<GraphData> StrainValueData { get; set; }

        public QssData()
        {

        }
    }
}
