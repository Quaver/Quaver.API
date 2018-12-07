using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    public class WristState
    {
        /// <summary>
        /// 
        /// </summary>
        public bool ForceDown { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FingerState WristPair { get; set; }
    }
}
