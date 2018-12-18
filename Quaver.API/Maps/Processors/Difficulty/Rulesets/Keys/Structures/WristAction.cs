using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    /// <summary>
    /// Will Represent Wrist Action
    /// - Up = Easier to play when wrist is up. (Think: Mashing)
    /// - Down = Player forced to have wrist down due to LN
    /// </summary>
    public enum WristAction
    {
        None,
        Up,
        Down
    }
}
