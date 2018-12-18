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
        public float WristDifficulty { get; set; } = 1;

        /// <summary>
        /// 
        /// </summary>
        public float NextStateDelta => NextState.Time - Time;

        /// <summary>
        /// 
        /// </summary>
        public WristAction WristAction { get; set; } = WristAction.None;

        /// <summary>
        /// 
        /// </summary>
        public float Time { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FingerState WristPair { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public WristState NextState { get; }

        /// <summary>
        /// 
        /// </summary>
        public int RepetitionCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public WristState(WristState next) => NextState = next;
    }
}
