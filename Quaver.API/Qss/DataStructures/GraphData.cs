using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss.DataStructures
{
    public class GraphData
    {
        /// <summary>
        /// Start Time
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// Y Value for current graph object. Used for note density and strain value.
        /// </summary>
        public float Value { get; set; }
    }
}
