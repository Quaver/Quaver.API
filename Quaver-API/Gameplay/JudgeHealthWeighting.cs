using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Gameplay
{
    /// <summary>
    ///     The weighting in health each judge gives
    /// </summary>
    public static class JudgeHealthWeighting
    {
        public static readonly double Marv = 0.5;
        public static readonly double Perf = 0.4;
        public static readonly double Great = 0.1;
        public static readonly double Good = -2.0;
        public static readonly double Okay = -2.5;
        public static readonly double Miss = -3.0;
    }
}
