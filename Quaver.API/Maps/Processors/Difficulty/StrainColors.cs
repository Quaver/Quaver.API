using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Quaver.API.Enums;

namespace Quaver.API.Maps.Processors.Difficulty
{
    /// <summary>
    ///     Generates Colors for anything related to QSR and note density,
    ///     This class and its contents are only used for generating colors,
    /// </summary>
    public static class StrainColors
    {
        /// <summary>
        ///     Interval at which each difficulty tier starts at
        /// </summary>
        public static Dictionary<DifficultyTier, float> DifficultyTierInterval { get; } = new Dictionary<DifficultyTier, float>()
        {
            { DifficultyTier.Tier1, 0 },
            { DifficultyTier.Tier2, 10 },
            { DifficultyTier.Tier3, 18 },
            { DifficultyTier.Tier4, 26 },
            { DifficultyTier.Tier5, 34 },
            { DifficultyTier.Tier6, 42 },
            { DifficultyTier.TierMax, 50 }
        };

        /// <summary>
        ///     Colors to represent each difficulty tier
        /// </summary>
        public static Dictionary<DifficultyTier, Color> DifficultyTierColor { get; } = new Dictionary<DifficultyTier, Color>
        {
            { DifficultyTier. Tier1, Color.FromArgb(255, 126, 111, 90) },
            { DifficultyTier.Tier2, Color.FromArgb(255, 184, 184, 184) },
            { DifficultyTier.Tier3, Color.FromArgb(255, 242, 218, 104) },
            { DifficultyTier.Tier4, Color.FromArgb(255, 146, 255, 172) },
            { DifficultyTier.Tier5, Color.FromArgb(255, 112, 227, 225) },
            { DifficultyTier.Tier6, Color.FromArgb(255, 255, 146, 255) },
            { DifficultyTier.TierMax, Color.FromArgb(255, 255, 97, 119) }
        };

        /// <summary>
        ///     Interval at which each note density tier starts at
        /// </summary>
        public static Dictionary<NoteDensityTier, float> DensityTierInterval { get; } = new Dictionary<NoteDensityTier, float>()
        {
            { NoteDensityTier.Tier1, 7 },
            { NoteDensityTier.Tier2, 14 },
            { NoteDensityTier.Tier3, 21 },
            { NoteDensityTier.Tier4, 28 },
            { NoteDensityTier.Tier5, 35 },
            { NoteDensityTier.TierMax, 42 }
        };

        /// <summary>
        ///     Colors to represent each note density tier
        /// </summary>
        public static Dictionary<NoteDensityTier, Color> DensityTierColor { get; } = new Dictionary<NoteDensityTier, Color>
        {
            { NoteDensityTier.Tier1, Color.FromArgb(255, 25, 25, 204) },
            { NoteDensityTier.Tier2, Color.FromArgb(255, 51, 51, 255) },
            { NoteDensityTier.Tier3, Color.FromArgb(255, 153, 102, 255) },
            { NoteDensityTier.Tier4, Color.FromArgb(255, 255, 102, 51) },
            { NoteDensityTier.Tier5, Color.FromArgb(255, 255, 25, 13) },
            { NoteDensityTier.TierMax, Color.FromArgb(255, 178, 0, 0) }
        };

        /// <summary>
        ///     Skill Color for Tech Jacks
        /// </summary>
        private static Color TechJackSkillColor { get; } = Color.FromArgb(255, 85, 169, 255);

        /// <summary>
        ///     Skill Color for Long Jacks
        /// </summary>
        private static Color LongJackSkillColor { get; } = Color.FromArgb(255, 127, 84, 255);

        /// <summary>
        ///     Skill Color for Anchors
        /// </summary>
        private static Color AnchorSkillColor { get; } = Color.FromArgb(255, 255, 80, 86);

        /// <summary>
        ///     Skill Color for Rolls
        /// </summary>
        private static Color RollSkillColor { get; } = Color.FromArgb(255, 255, 203, 80);

        /// <summary>
        ///     Skill Color for Release
        /// </summary>
        private static Color ReleaseSkillColor { get; } = Color.FromArgb(255, 68, 255, 80);

        /// <summary>
        ///     Used for skill color delta
        /// </summary>
        private const float SKILL_COLOR_DELTA = 0.6f;

        /// <summary>
        ///     Returns Color for general Strain Rating difficulty. Reads QSR (Quaver Strain Rating)
        ///     This can be used to return Color to represent difficulty of a map or play.
        /// </summary>
        /// <param name="qsr"></param>
        /// <returns></returns>
        public static Color GetStrainRatingColor(float qsr)
        {
            // Tier 0 and negative rating?
            if (qsr <= DifficultyTierInterval[DifficultyTier.Tier1])
                return DifficultyTierColor[DifficultyTier.Tier1];

            // Find color for qsr
            for (var i = 0; i < DifficultyTierInterval.Count - 1; i++)
            {
                var val = qsr - DifficultyTierInterval[(DifficultyTier)i];
                var current = (DifficultyTier)i;
                var next = (DifficultyTier)(i+1);
                var denominator = DifficultyTierInterval[next] - DifficultyTierInterval[current];

                // return proper color if qsr value is between current and next interval
                if (val >= 0 && val < denominator)
                {
                    return GetGradientColor(DifficultyTierColor[current], DifficultyTierColor[next], val/denominator);
                }
            }

            // qsr exceeds max tier interval for max color
            return DifficultyTierColor[DifficultyTier.TierMax];
        }

        /// <summary>
        ///     Returns Color for note density graph area. Reads NPS (Notes per second)
        /// </summary>
        ///  /// <param name="nps"></param>
        /// <returns></returns>
        public static Color GetNoteDensityColor(float nps)
        {
            // Tier 0 and negative density?
            if (nps <= DensityTierInterval[NoteDensityTier.Tier1])
                return DensityTierColor[NoteDensityTier.Tier1];

            // Find color for density
            for (var i = 0; i < DensityTierInterval.Count - 1; i++)
            {
                var val = nps - DensityTierInterval[(NoteDensityTier)i];
                var current = (NoteDensityTier)i;
                var next = (NoteDensityTier)(i + 1);
                var denominator = DensityTierInterval[next] - DensityTierInterval[current];

                // return proper color if qsr value is between current and next interval
                if (val >= 0 && val < denominator)
                {
                    return GetGradientColor(DensityTierColor[current], DensityTierColor[next], val / denominator);
                }
            }

            // density exceeds max tier interval for max color
            return DensityTierColor[NoteDensityTier.TierMax];
        }

        /// <summary>
        ///     WIP. Returns color of combined skill values.
        /// </summary>
        /// <returns></returns>
        public static Color GetSkillColor(float tjack, float ljack, float roll, float anchor, float release)
        {
            var red = 255;
            var green = 255;
            var blue = 255;

            /*
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

            */
            return Color.FromArgb(255, red, green, blue);
        }

        /// <summary>
        ///     Returns color from percentage in between two colors
        /// </summary>
        /// <param name="color1">First Color</param>
        /// <param name="color2">Second Color</param>
        /// <param name="value">Percentage Value (Between 0 and 1)</param>
        /// <returns></returns>
        public static Color GetGradientColor(Color color1, Color color2, float value)
        {
            var red = FloatToByteInt(((color2.R - color1.R) * value) + color1.R);
            var green = FloatToByteInt(((color2.G - color1.G) * value) + color1.G);
            var blue = FloatToByteInt(((color2.B - color1.B) * value) + color1.B);

            return Color.FromArgb(255, red, green, blue);
        }

        /// <summary>
        ///     Returns an int value between 0 and 255
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static int FloatToByteInt(float f) => (int)Math.Min(Math.Max(f, 0), 255);
    }
}
