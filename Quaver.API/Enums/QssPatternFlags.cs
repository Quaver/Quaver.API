using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Enums
{
    /// <summary>
    ///     Used to display prominent patterns of a map in the client
    /// </summary>
    [Flags]
    public enum QssPatternFlags
    {
        Unknown = 1 << -1,
        MiniJack = 1 << 0,
        ChordJack = 1 << 1,
        KoreaJack = 1 << 2,
        LongJack = 1 << 3,
        QuadJack = 1 << 4,
        LightStream = 1 << 5,
        JumpStream = 1 << 6,
        HandStream = 1 << 7,
        QuadStream = 1 << 8,
        InverseLN = 1 << 9,
        ReleaseLN = 1 << 10,
        Polyrhythm = 1 << 11,
        JumpTrill = 1 << 12,
        SplitTrill = 1 << 13
    }
}
