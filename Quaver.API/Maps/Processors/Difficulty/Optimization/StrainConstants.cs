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
        public List<DynamicVariable> ConstantVariables { get; set; }

        /// <summary>
        ///     Create a new constant variable for difficulty calculation and optimization.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DynamicVariable NewConstant(string name, float value)
        {
            // Create new constant variable
            var constVar = new DynamicVariable(name, value);
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

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="constVariables"></param>
        public StrainConstants(List<DynamicVariable> constVariables = null)
        {
            if (constVariables == null)
                ConstantVariables = new List<DynamicVariable>();

            else
                ConstantVariables = constVariables;
        }

        /// <summary>
        ///     Returns an array of the Constant Variables
        /// </summary>
        /// <returns></returns>
        public double[] ConstantsToArray()
        {
            var result = new double[ConstantVariables.Count];
            for (var i = 0; i < ConstantVariables.Count; i++)
            {
                result[i] = ConstantVariables[i].Value;
            }

            return result;
        }

        /// <summary>
        ///     Update current constants
        /// </summary>
        /// <param name="updated"></param>
        public void UpdateConstants(double[] updated)
        {
            for (var i = 0; i < updated.Length; i++)
            {
                ConstantVariables[i].Value = (float)updated[i];
            }
        }
    }
}
