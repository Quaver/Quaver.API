using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Quaver.API.Qss
{
    internal class Colors
    {
        /// <summary>
        /// Returns Color for general Strain Rating difficulty. Reads QSR (Quaver Strain Rating)
        /// This can be used to return Color to represent difficulty of a map or play.
        /// </summary>
        /// <param name="qsr"></param>
        /// <returns></returns>
        public static Color GetStrainRatingColor(float qsr)
        {
            var color = Color.FromArgb(255, 255, 255, 1);
            return color;
        }

        /// <summary>
        /// Returns Color for note density graph area. Reads NPS (Notes per second)
        /// </summary>
        ///  /// <param name="nps"></param>
        /// <returns></returns>
        public static Color GetNoteDensityColor(float nps)
        {
            var color = Color.FromArgb(255, 255, 255, 1);
            return color;
        }

        /// <summary>
        /// wip. Returns color of 2 combined skills. (Arbitrary example: 20% jack + 60% stream = yellowish-orange color)
        /// </summary>
        /// <returns></returns>
        public static Color GetSkillColor()
        {
            var color = Color.FromArgb(255, 255, 255, 1);
            return color;
        }
    }
}
