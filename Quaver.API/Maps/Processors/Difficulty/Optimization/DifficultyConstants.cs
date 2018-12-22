/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Quaver.API.Maps.Processors.Difficulty.Optimization
{
    /// <summary>
    ///     Constant Variables for any specific Gamemode that the Strain Solver can use to solve.
    /// </summary>
    public class DifficultyConstants
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
        public DynamicVariable NewConstant(string name, double value)
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
        public DifficultyConstants(List<DynamicVariable> constVariables = null)
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
                ConstantVariables[i].Value = updated[i];
            }
        }
    }
}
