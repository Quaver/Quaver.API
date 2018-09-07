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
        /// QSR required in order to be considered first tier
        /// </summary>
        public const float TIER_1_RATING = 10;

        /// <summary>
        /// QSR required in order to be considered second tier
        /// </summary>
        public const float TIER_2_RATING = 20;

        /// <summary>
        /// QSR required in order to be considered third tier
        /// </summary>
        public const float TIER_3_RATING = 30;

        /// <summary>
        /// QSR required in order to be considered fourth tier
        /// </summary>
        public const float TIER_4_RATING = 40;

        /// <summary>
        /// QSR tiers limit
        /// </summary>
        public const float MAX_TIER_RATING = 50;

        /// <summary>
        /// Color for Beginner Tier Songs (QSR 0 - 10)
        /// </summary>
        public static Color TierZeroColor { get; } = Color.FromArgb(128, 128, 128, 0);

        /// <summary>
        /// Color for First Tier Songs (QSR 10 - 20)
        /// </summary>
        public static Color TierOneColor { get; } = Color.FromArgb(255, 0, 255, 0);

        /// <summary>
        /// Color for Second Tier Songs (QSR 20 - 30)
        /// </summary>
        public static Color TierTwoColor { get; } = Color.FromArgb(255, 255, 255, 0);

        /// <summary>
        /// Color for Third Tier Songs (QSR 30 - 40)
        /// </summary>
        public static Color TierThreeColor { get; } = Color.FromArgb(255, 255, 0, 0);

        /// <summary>
        /// Color for Fourth Tier Songs (QSR 40+)
        /// </summary>
        public static Color TierFourColor { get; } = Color.FromArgb(255, 255, 0, 255);

        /// <summary>
        /// Max Tier Color (QSR 50+)
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
            // Gradient value for two colors
            float val;

            // Tier 0 and negative rating?
            if (qsr <= 0)
            {
                return TierZeroColor;
            }

            // Tier 0
            else if (qsr < TIER_1_RATING)
            {
                val = qsr / TIER_1_RATING;
                return GetGradientColor(TierZeroColor, TierOneColor, val);
            }

            // Tier 1
            else if (qsr < TIER_2_RATING)
            {
                val = qsr / (TIER_2_RATING -TIER_1_RATING);
                return GetGradientColor(TierOneColor, TierTwoColor, val);
            }

            // Tier 2
            else if (qsr < TIER_3_RATING)
            {
                val = qsr / (TIER_3_RATING - TIER_2_RATING);
                return GetGradientColor(TierTwoColor, TierThreeColor, val);
            }

            // Tier 3
            else if (qsr < TIER_4_RATING)
            {
                val = qsr / (TIER_4_RATING - TIER_3_RATING);
                return GetGradientColor(TierThreeColor, TierFourColor, val);
            }

            // Tier 4
            else if (qsr < MAX_TIER_RATING)
            {
                val = qsr / (MAX_TIER_RATING - TIER_4_RATING);
                return GetGradientColor(TierFourColor, MaxTierColor, val);
            }

            // Max Tier
            return MaxTierColor;
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

        /// <summary>
        /// Returns color from percentage in between two colors
        /// </summary>
        /// <param name="color1">First Color</param>
        /// <param name="color2">Second Color</param>
        /// <param name="value">Percentage Value (Between 0 and 1)</param>
        /// <returns></returns>
        public static Color GetGradientColor(Color color1, Color color2, float value)
        {
            var red = (int)(Math.Abs(color1.R - color2.R) * value) + Math.Min(color1.R, color2.R);
            var green = (int)(Math.Abs(color1.G - color2.G) * value) + Math.Min(color1.G, color2.G);
            var blue = (int)(Math.Abs(color1.B - color2.B) * value) + Math.Min(color1.B, color2.B);

            return Color.FromArgb(255, red, green, blue);
        }
    }
}
