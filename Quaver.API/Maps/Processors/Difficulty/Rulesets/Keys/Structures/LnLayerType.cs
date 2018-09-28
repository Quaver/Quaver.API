using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    /// <summary>
    ///     Represented by how the current LN object is stacked with other LN objects in a hand.
    /// </summary>
    public enum LnLayerType
    {
        None,
        InsideRelease,
        OutsideRelease,
        InsideTap
    }
}
