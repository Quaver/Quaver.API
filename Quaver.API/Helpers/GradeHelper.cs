/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quaver.API.Enums;

namespace Quaver.API.Helpers
{
    public static class GradeHelper
    {
        /// <summary>
        ///     Gets the grade from an accuracy value
        /// </summary>
        /// <param name="accuracy"></param>
        /// <param name="isPerfect"></param>
        /// <returns></returns>
        public static Grade GetGradeFromAccuracy(float accuracy, bool isPerfect = false)
        {
            switch (accuracy)
            {
                case 100 when isPerfect:
                    return Grade.XX;
                case 100 when true:
                    return Grade.X;
                default:
                    if (accuracy >= 99)
                        return Grade.SS;
                    else if (accuracy >= 95)
                        return Grade.S;
                    else if (accuracy >= 90)
                        return Grade.A;
                    else if (accuracy >= 80)
                        return Grade.B;
                    else if (accuracy >= 70)
                        return Grade.C;
                    else if (accuracy >= 60)
                        return Grade.D;
                    break;
            }

            return Grade.F;
        }
    }
}
