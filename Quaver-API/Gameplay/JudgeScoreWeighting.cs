using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Gameplay
{
    public static class JudgeScoreWeighting
    {
        public static readonly int Marv = 100;
        public static readonly int Perf = 50;
        public static readonly int Great = 25;
        public static readonly int Good = 10;
        public static readonly int Okay = 5;
        public static readonly int Miss = 0;
    }
}
