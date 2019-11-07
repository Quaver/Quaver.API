/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using Quaver.API.Enums;

namespace Quaver.API.Helpers
{
    public static class GradeHelper
    {
        /// <summary>
        ///     Gets the grade from an accuracy value
        /// </summary>
        /// <param name="accuracy"></param>
        /// <returns></returns>
        public static Grade GetGradeFromAccuracy(float accuracy)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (accuracy == 100)
                return Grade.X;
            if (accuracy >= 99)
                return Grade.SS;
            if (accuracy >= 95)
                return Grade.S;
            if (accuracy >= 90)
                return Grade.A;
            if (accuracy >= 80)
                return Grade.B;
            if (accuracy >= 70)
                return Grade.C;

            return Grade.D;
        }

        /// <summary>
        ///     <see cref="Grade"/> isn't ordered by the level of importance/lowest-highest grade.
        ///     This returns an integer that represents the level of importance
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int GetGradeImportanceIndex(Grade g)
        {
            switch (g)
            {
                case Grade.None:
                    return 0;
                case Grade.F:
                    return 1;
                case Grade.D:
                    return 2;
                case Grade.C:
                    return 3;
                case Grade.B:
                    return 4;
                case Grade.A:
                    return 5;
                case Grade.S:
                    return 6;
                case Grade.SS:
                    return 7;
                case Grade.X:
                    return 8;
                case Grade.XX:
                    return 9;
                default:
                    throw new ArgumentOutOfRangeException(nameof(g), g, null);
            }
        }

        /// <summary>
        ///     Returns the grade from the integer that represents its level of importance.
        ///     <see cref="GetGradeImportanceIndex"/>
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static Grade FromImportanceIndex(int i)
        {
            switch (i)
            {
                case 0:
                    return Grade.None;
                case 1:
                    return Grade.F;
                case 2:
                    return Grade.D;
                case 3:
                    return Grade.C;
                case 4:
                    return Grade.B;
                case 5:
                    return Grade.A;
                case 6:
                    return Grade.S;
                case 7:
                    return Grade.SS;
                case 8:
                    return Grade.X;
                case 9:
                    return Grade.XX;
                default:
                    throw new ArgumentOutOfRangeException(nameof(i), i, null);
            }
        }
    }
}
