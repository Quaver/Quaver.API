using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Enums
{
    /// <summary>
    ///     Used to warn players about calculation inaccuracies due to vibro/rolls in client
    /// </summary>
    [Flags]
    public enum QssWarningFlags
    {
        None = 1 << -1,
        VibroOverload = 1 << 0,
        RollsOverload = 1 << 1
    }
}
