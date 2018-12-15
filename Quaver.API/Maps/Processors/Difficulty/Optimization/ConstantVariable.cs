using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Optimization
{
    /// <summary>
    ///     This class is used to represent a constant variable for difficutly calculation.
    ///     It is also used for optimization. 
    /// </summary>
    public class ConstantVariable
    {
        /// <summary>
        ///     Name of this constant
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Value of this constant
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        ///     Const
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public ConstantVariable(string name, double value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     Returns string "(name) = (value)". Mainly used for debugging and optimizing.
        /// </summary>
        /// <returns></returns>
        public string GetVariableInfo() => Name + " " + Value;
    }
}
