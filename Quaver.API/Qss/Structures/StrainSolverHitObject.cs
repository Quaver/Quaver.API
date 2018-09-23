using Quaver.API.Maps.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quaver.API.Qss.Structures
{
    /// <summary>
    ///     An expanded version of hit object that is used for QSS implementations in API/editor/calculation
    /// </summary>
    public class StrainSolverHitObject : HitObjectInfo
    {
        /// <summary>
        ///     Current Finger State of the hit object
        /// </summary>
        public FingerState FingerState { get; set; }

        /// <summary>
        ///     Strain Multiplier calculated by LN difficulty
        /// </summary>
        public float LnStrainMultiplier { get; set; } = 1;

        /// <summary>
        ///     Current Strain Value for this hit object.
        ///     Note: This is not used for QSS calculation. It is for display/ui purposes.
        /// </summary>
        public float StrainValue { get; set; }
    }
}
