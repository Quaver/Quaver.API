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
    ///     This class is used to represent a constant variable for difficutly calculation.
    ///     It is also used for optimization. 
    /// </summary>
    public class DynamicVariable
    {
        /// <summary>
        ///     Name of this variable
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Value of this variable
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        ///     Const
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public DynamicVariable(string name, float value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     Returns string "(name) = (value)". Mainly used for debugging and optimizing.
        /// </summary>
        /// <returns></returns>
        public string GetVariableInfo() => $"{Name} = NewConstant(\"{Name}\", {Value}f);";
    }
}
