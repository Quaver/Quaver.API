using Quaver.API.Qss.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss
{
    internal class QssData
    {
        internal float OverallDifficulty { get; set; }
        internal float AverageNoteDensity { get; set; }
        internal List<FingerState> FingerStates { get; set; }
    }
}
