using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Enums
{
    /// <summary>
    ///     Used to warn players about calculation inaccuracies due to vibro/rolls in client
    /// </summary>
    public enum QssWarningFlags
    {
        VibroOverload,
        RollsOverload
    }
}
