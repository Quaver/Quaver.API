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
        public float MapLength { get; set; }
        public float OverallDifficulty { get; set; }
        public float AverageNoteDensity { get; set; }
        public List<HitObjectData> HitObjects {get; set; }
        public List<HitObjectData> LeftHandObjects { get; set; }
        public List<HitObjectData> RightHandObjects { get; set; }

        public QssData()
        {

        }
    }
}
