using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss.DataStructures
{
    internal class FingerState
    {
        /// <summary>
        /// Hand Index
        /// </summary>
        private int HandIndex { get; set; }

        /// <summary>
        /// When your fingers are supposed to be at this state
        /// </summary>
        private float StartTime { get; set; }

        /// <summary>
        /// State of fingers
        /// </summary>
        private int State { get; set; }

        /// <summary>
        /// Strain Value (Difficulty)
        /// </summary>
        private float Strain { get; set; }
    }
}
