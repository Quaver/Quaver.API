using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    internal class WristState
    {
        /// <summary>
        /// 
        /// </summary>
        public bool ForceDown { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public FingerState WristPair { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="wristDown"></param>
        public WristState(FingerState pair, bool wristDown)
        {
            WristPair = pair;
            ForceDown = wristDown;
        }
    }
}
