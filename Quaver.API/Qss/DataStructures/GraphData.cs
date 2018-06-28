using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss.DataStructures
{
    class GraphData
    {
        /// <summary>
        /// Start Time
        /// </summary>
        internal int StartTime { get; set; }

        /// <summary>
        /// Y Value for current graph object. Used for note density and strain value.
        /// </summary>
        internal float Value { get; set; }
    }
}
