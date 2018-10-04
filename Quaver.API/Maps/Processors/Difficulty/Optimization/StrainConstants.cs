using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Optimization
{
    /// <summary>
    ///     Constant Variables for any specific Gamemode that the Strain Solver can use to solve.
    /// </summary>
    public class StrainConstants
    {
        /// <summary>
        ///     List of Constant Variables for the current Solver.
        /// </summary>
        public List<ConstantVariable> ConstantVariables = new List<ConstantVariable>();

        /// <summary>
        ///     Create a new constant variable for difficulty calculation and optimization.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ConstantVariable NewConstant(string name, float value)
        {
            // Create new constant variable
            var constVar = new ConstantVariable(name, value);
            ConstantVariables.Add(constVar);

            // return
            return constVar;
        }

        /// <summary>
        ///     Returns a string of Constant Variable info mainly used for debugging and optimization 
        /// </summary>
        /// <returns></returns>
        public string GetInfoFromVariables()
        {
            // Generate output
            var output = "";
            foreach (var constVar in ConstantVariables)
                output += constVar.GetVariableInfo() + "\n";

            // return
            return output;
        }
    }
}
