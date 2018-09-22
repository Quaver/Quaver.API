using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Quaver.API.Qss
{
    /// <summary>
    /// Generates Colors for anything related to QSR and note density,
    /// This class and its contents are only used for generating colors,
    /// </summary>
    internal class Colors
    {
        /// <summary>
        /// QSR required in order to be considered first tier
        /// </summary>
        private const float TIER_1_RATING = 10;

        /// <summary>
        /// QSR required in order to be considered second tier
        /// </summary>
        private const float TIER_2_RATING = 20;

        /// <summary>
        /// QSR required in order to be considered third tier
        /// </summary>
        private const float TIER_3_RATING = 30;

        /// <summary>
        /// QSR required in order to be considered fourth tier
        /// </summary>
        private const float TIER_4_RATING = 40;

        /// <summary>
        /// QSR tiers limit
        /// </summary>
        private const float MAX_TIER_RATING = 50;

        /// <summary>
        /// NPS interval used for color
        /// </summary>
        private const float NPS_RATING_1 = 7;

        /// <summary>
        /// NPS interval used for color
        /// </summary>
        private const float NPS_RATING_2 = 14;

        /// <summary>
        /// NPS interval used for color
        /// </summary>
        private const float NPS_RATING_3 = 21;

        /// <summary>
        /// NPS interval used for color
        /// </summary>
        private const float NPS_RATING_4 = 28;

        /// <summary>
        /// NPS interval used for color
        /// </summary>
        private const float NPS_RATING_5 = 35;

        /// <summary>
        /// NPS interval used for color
        /// </summary>
        private const float NPS_RATING_MAX = 42;

        /// <summary>
        /// Used for skill color delta
        /// </summary>
        private const float SKILL_COLOR_DELTA = 0.6f;

        /// <summary>
        /// Skill Color for Tech Jacks
        /// </summary>
        private static Color TechJackSkillColor { get; } = Color.FromArgb(255, 85, 169, 255);

        /// <summary>
        /// Skill Color for Long Jacks
        /// </summary>
        private static Color LongJackSkillColor { get; } = Color.FromArgb(255, 127, 84, 255);

        /// <summary>
        /// Skill Color for Anchors
        /// </summary>
        private static Color AnchorSkillColor { get; } = Color.FromArgb(255, 255, 80, 86);

        /// <summary>
        /// Skill Color for Rolls
        /// </summary>
        private static Color RollSkillColor { get; } = Color.FromArgb(255, 255, 203, 80);

        /// <summary>
        /// Skill Color for Release
        /// </summary>
        private static Color ReleaseSkillColor { get; } = Color.FromArgb(255, 68, 255, 80);

        /// <summary>
        /// Color for Beginner Tier Songs (QSR 0 - 10)
        /// </summary>
        private static Color TierZeroColor { get; } = Color.FromArgb(255, 128, 128, 128);

        /// <summary>
        /// Color for First Tier Songs (QSR 10 - 20)
        /// </summary>
        private static Color TierOneColor { get; } = Color.FromArgb(255, 0, 255, 0);

        /// <summary>
        /// Color for Second Tier Songs (QSR 20 - 30)
        /// </summary>
        private static Color TierTwoColor { get; } = Color.FromArgb(255, 255, 255, 0);

        /// <summary>
        /// Color for Third Tier Songs (QSR 30 - 40)
        /// </summary>
        private static Color TierThreeColor { get; } = Color.FromArgb(255, 255, 0, 0);

        /// <summary>
        /// Color for Fourth Tier Songs (QSR 40+)
        /// </summary>
        private static Color TierFourColor { get; } = Color.FromArgb(255, 255, 0, 255);

        /// <summary>
        /// Max Tier Color (QSR 50+)
        /// </summary>
        private static Color MaxTierColor { get; } = Color.FromArgb(255, 255, 200, 255);

        /// <summary>
        /// NPS Graph Color for anything below 7nps
        /// </summary>
        private static Color NpsColor0 { get; } = Color.FromArgb(255, 25, 25, 204);

        /// <summary>
        /// NPS Graph Color for anything between 7-14nps
        /// </summary>
        private static Color NpsColor1 { get; } = Color.FromArgb(255, 51, 51, 255);

        /// <summary>
        /// NPS Graph Color for anything between 14-21nps
        /// </summary>
        private static Color NpsColor2 { get; } = Color.FromArgb(255, 153, 102, 255);

        /// <summary>
        /// NPS Graph Color for anything between 21-28nps
        /// </summary>
        private static Color NpsColor3 { get; } = Color.FromArgb(255, 255, 51, 178);

        /// <summary>
        /// NPS Graph Color for anything between 28-35nps
        /// </summary>
        private static Color NpsColor4 { get; } = Color.FromArgb(255, 255, 102, 51);

        /// <summary>
        /// NPS Graph Color for anything between 35-42nps
        /// </summary>
        private static Color NpsColor5 { get; } = Color.FromArgb(255, 255, 25, 13);

        /// <summary>
        /// NPS Graph Color for anything over 42nps
        /// </summary>
        private static Color NpsColorMax { get; } = Color.FromArgb(255, 178, 0, 0);

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
            // Gradient value for two colors
            float val;

            // Nps Interval 0 (0 - 7nps)
            if (nps < NPS_RATING_1)
            {
                val = nps / NPS_RATING_1;
                return GetGradientColor(NpsColor0, NpsColor1, val);
            }

            // Nps Interval 1 (7 - 14nps)
            else if (nps < NPS_RATING_2)
            {
                val = nps / (NPS_RATING_2 - NPS_RATING_1);
                return GetGradientColor(NpsColor1, NpsColor2, val);
            }

            // Nps Interval 2 (14 - 21nps)
            else if (nps < NPS_RATING_3)
            {
                val = nps / (NPS_RATING_3 - NPS_RATING_2);
                return GetGradientColor(NpsColor2, NpsColor3, val);
            }

            // Nps Interval 3 (21 - 28nps)
            else if (nps < NPS_RATING_4)
            {
                val = nps / (NPS_RATING_4 - NPS_RATING_3);
                return GetGradientColor(NpsColor3, NpsColor4, val);
            }

            // Nps Interval 4 (28 - 35nps)
            else if (nps < NPS_RATING_5)
            {
                val = nps / (NPS_RATING_5 - NPS_RATING_4);
                return GetGradientColor(NpsColor4, NpsColor5, val);
            }

            // Nps Interval 5 (35 - 42nps)
            else if (nps < NPS_RATING_MAX)
            {
                val = nps / (NPS_RATING_MAX - NPS_RATING_5);
                return GetGradientColor(NpsColor5, NpsColorMax, val);
            }

            // Max nps Interval (42+ nps)
            return NpsColorMax;
        }

        /// <summary>
        /// WIP. Returns color of combined skill values.
        /// </summary>
        /// <returns></returns>
        public static Color GetSkillColor(float tjack, float ljack, float roll, float anchor, float release)
        {
            // todo: this code is probably not gonna work, i'll need to test it later
            var red = (int)(((TechJackSkillColor.R / 255f) * Math.Max(Math.Pow(tjack / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((LongJackSkillColor.R / 255f) * Math.Max(Math.Pow(ljack / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((RollSkillColor.R / 255f) * Math.Max(Math.Pow(roll / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((AnchorSkillColor.R / 255f) * Math.Max(Math.Pow(anchor / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((ReleaseSkillColor.R / 255f) * Math.Max(Math.Pow(release / TIER_4_RATING, SKILL_COLOR_DELTA), 1)) / 5f);

            var green = (int)(((TechJackSkillColor.G / 255f) * Math.Max(Math.Pow(tjack / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((LongJackSkillColor.G / 255f) * Math.Max(Math.Pow(ljack / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((RollSkillColor.G / 255f) * Math.Max(Math.Pow(roll / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((AnchorSkillColor.G / 255f) * Math.Max(Math.Pow(anchor / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((ReleaseSkillColor.G / 255f) * Math.Max(Math.Pow(release / TIER_4_RATING, SKILL_COLOR_DELTA), 1)) / 5f);

            var blue = (int)(((TechJackSkillColor.B / 255f) * Math.Max(Math.Pow(tjack / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((LongJackSkillColor.B / 255f) * Math.Max(Math.Pow(ljack / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((RollSkillColor.B / 255f) * Math.Max(Math.Pow(roll / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((AnchorSkillColor.B / 255f) * Math.Max(Math.Pow(anchor / TIER_4_RATING, SKILL_COLOR_DELTA), 1))
                + ((ReleaseSkillColor.B / 255f) * Math.Max(Math.Pow(release / TIER_4_RATING, SKILL_COLOR_DELTA), 1)) / 5f);

            return Color.FromArgb(255, red, green, blue);
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
