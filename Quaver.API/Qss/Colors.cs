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
        /// Color for Beginner Tier (Skill Lvl 0 - 10)
        /// </summary>
        public static Color TierZeroColor { get; } = Color.FromArgb(128, 128, 128, 0);

        /// <summary>
        /// Color for First Tier Players (Skill Lvl 10 - 20)
        /// </summary>
        public static Color TierOneColor { get; } = Color.FromArgb(255, 0, 255, 0);

        /// <summary>
        /// Color for Second Tier Players (Skill Lvl 20 - 30)
        /// </summary>
        public static Color TierTwoColor { get; } = Color.FromArgb(255, 255, 255, 0);

        /// <summary>
        /// Color for Third Tier Players (Skill Lvl 30 - 40)
        /// </summary>
        public static Color TierThreeColor { get; } = Color.FromArgb(255, 255, 0, 0);

        /// <summary>
        /// Color for Fourth Tier Players (Skill Lvl 40+)
        /// </summary>
        public static Color TierFourColor { get; } = Color.FromArgb(255, 255, 0, 255);

        /// <summary>
        /// Max Tier Color (Lvl 50+)
        /// </summary>
        public static Color MaxTierColor { get; } = Color.FromArgb(255, 255, 200, 255);

        /// <summary>
        /// Returns Color for general Strain Rating difficulty. Reads QSR (Quaver Strain Rating)
        /// This can be used to return Color to represent difficulty of a map or play.
        /// </summary>
        /// <param name="qsr"></param>
        /// <returns></returns>
        public static Color GetStrainRatingColor(float qsr)
        {
            var color = Color.FromArgb(255, 255, 255, 255);
            return color;
        }

        /// <summary>
        /// Returns Color for note density graph area. Reads NPS (Notes per second)
        /// </summary>
        ///  /// <param name="nps"></param>
        /// <returns></returns>
        public static Color GetNoteDensityColor(float nps)
        {
            var color = Color.FromArgb(255, 255, 255, 255);
            return color;
        }

        /// <summary>
        /// wip. Returns color of 2 combined skills. (Arbitrary example: 20% jack + 60% stream = yellowish-orange color)
        /// </summary>
        /// <returns></returns>
        public static Color GetSkillColor()
        {
            var color = Color.FromArgb(255, 255, 255, 255);
            return color;
        }
    }
}
