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
using System.Drawing;
using Quaver.API.Enums;

namespace Quaver.API.Maps.Processors.Difficulty
{
    /// <summary>
    ///     Generates Colors for anything related to QSR and note density,
    ///     This class and its contents are only used for generating colors,
    /// </summary>
    public static class DifficultyColors
    {
        /// <summary>
        ///     Interval at which each difficulty tier starts at
        /// </summary>
        public static Dictionary<ColorTier, float> DifficultyTierInterval { get; } = new Dictionary<ColorTier, float>()
        {
            { ColorTier.Tier1, 0 },
            { ColorTier.Tier2, 10 },
            { ColorTier.Tier3, 18 },
            { ColorTier.Tier4, 26 },
            { ColorTier.Tier5, 34 },
            { ColorTier.Tier6, 42 },
            { ColorTier.Tier7, 50 }
        };

        /// <summary>
        ///     Colors to represent each difficulty tier
        /// </summary>
        public static Dictionary<ColorTier, Color> DifficultyTierColor { get; } = new Dictionary<ColorTier, Color>
        {
            { ColorTier.Tier1, Color.FromArgb(255, 126, 111, 90) },
            { ColorTier.Tier2, Color.FromArgb(255, 184, 184, 184) },
            { ColorTier.Tier3, Color.FromArgb(255, 242, 218, 104) },
            { ColorTier.Tier4, Color.FromArgb(255, 146, 255, 172) },
            { ColorTier.Tier5, Color.FromArgb(255, 112, 227, 225) },
            { ColorTier.Tier6, Color.FromArgb(255, 255, 146, 255) },
            { ColorTier.Tier7, Color.FromArgb(255, 255, 97, 119) }
        };

        /// <summary>
        ///     Interval at which each note density tier starts at
        /// </summary>
        public static Dictionary<ColorTier, float> DensityTierInterval { get; } = new Dictionary<ColorTier, float>()
        {
            { ColorTier.Tier1, 7 },
            { ColorTier.Tier2, 14 },
            { ColorTier.Tier3, 21 },
            { ColorTier.Tier4, 28 },
            { ColorTier.Tier5, 35 },
            { ColorTier.Tier6, 42 }
        };

        /// <summary>
        ///     Colors to represent each note density tier
        /// </summary>
        public static Dictionary<ColorTier, Color> DensityTierColor { get; } = new Dictionary<ColorTier, Color>
        {
            { ColorTier.Tier1, Color.FromArgb(255, 25, 25, 204) },
            { ColorTier.Tier2, Color.FromArgb(255, 51, 51, 255) },
            { ColorTier.Tier3, Color.FromArgb(255, 153, 102, 255) },
            { ColorTier.Tier4, Color.FromArgb(255, 255, 102, 51) },
            { ColorTier.Tier5, Color.FromArgb(255, 255, 25, 13) },
            { ColorTier.Tier6, Color.FromArgb(255, 178, 0, 0) }
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
            if (qsr <= DifficultyTierInterval[ColorTier.Tier1])
                return DifficultyTierColor[ColorTier.Tier1];

            // Find color for qsr
            for (var i = 0; i < DifficultyTierInterval.Count - 1; i++)
            {
                var val = qsr - DifficultyTierInterval[(ColorTier)i];
                var current = (ColorTier)i;
                var next = (ColorTier)(i+1);
                var denominator = DifficultyTierInterval[next] - DifficultyTierInterval[current];

                // return proper color if qsr value is between current and next interval
                if (val >= 0 && val < denominator)
                {
                    return GetGradientColor(DifficultyTierColor[current], DifficultyTierColor[next], val/denominator);
                }
            }

            // qsr exceeds max tier interval for max color
            return DifficultyTierColor[ColorTier.Tier7];
        }

        /// <summary>
        ///     Returns Color for note density graph area. Reads NPS (Notes per second)
        /// </summary>
        ///  /// <param name="nps"></param>
        /// <returns></returns>
        public static Color GetNoteDensityColor(float nps)
        {
            // Tier 0 and negative density?
            if (nps <= DensityTierInterval[ColorTier.Tier1])
                return DensityTierColor[ColorTier.Tier1];

            // Find color for density
            for (var i = 0; i < DensityTierInterval.Count - 1; i++)
            {
                var val = nps - DensityTierInterval[(ColorTier)i];
                var current = (ColorTier)i;
                var next = (ColorTier)(i + 1);
                var denominator = DensityTierInterval[next] - DensityTierInterval[current];

                // return proper color if qsr value is between current and next interval
                if (val >= 0 && val < denominator)
                {
                    return GetGradientColor(DensityTierColor[current], DensityTierColor[next], val / denominator);
                }
            }

            // density exceeds max tier interval for max color
            return DensityTierColor[ColorTier.Tier6];
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
            var red = FloatToByte(((color2.R - color1.R) * value) + color1.R);
            var green = FloatToByte(((color2.G - color1.G) * value) + color1.G);
            var blue = FloatToByte(((color2.B - color1.B) * value) + color1.B);

            return Color.FromArgb(255, red, green, blue);
        }

        /// <summary>
        ///     Returns an int value between 0 and 255
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static byte FloatToByte(float f) => (byte)Math.Min(Math.Max(f, 0), 255);
    }
}
