using Quaver.API.Qss.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss
{
    public class QssData
    {
        public float MapLength { get; set; } = 0;
        public float OverallDifficulty { get; set; } = 0;
        public float AverageNoteDensity { get; set; } = 0;
        public List<HitObjectData> HitObjects {get; set; }
        public List<HitObjectData> LeftHandObjects { get; set; }
        public List<HitObjectData> RightHandObjects { get; set; }

        public QssData()
        {

        }
    }
}
