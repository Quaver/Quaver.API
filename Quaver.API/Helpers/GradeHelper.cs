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
        public static Grades GetGradeFromAccuracy(float accuracy, bool isPerfect = false)
        {
            switch (accuracy)
            {
                case 100 when isPerfect:
                    return Grades.XX;
                case 100 when true:
                    return Grades.X;
                default:
                    if (accuracy >= 99)
                        return Grades.SS;
                    else if (accuracy >= 95)
                        return Grades.S;
                    else if (accuracy >= 90)
                        return Grades.A;
                    else if (accuracy >= 80)
                        return Grades.B;
                    else if (accuracy >= 70)
                        return Grades.C;
                    else if (accuracy >= 60)
                        return Grades.D;
                    break;
            }

            return Grades.F;
        }
    }
}
