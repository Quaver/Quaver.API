using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Rulesets.Keys.Structures
{
    /// <summary>
    ///     Determined by how many HitObjects are in a Chord
    /// </summary>
    public enum ChordType
    {
        None,
        Single,
        Jump,
        Hand,
        Quad,
        NChord
    }
}
