/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using Quaver.API.Enums;

namespace Quaver.API.Helpers
{
    public static class ModHelper
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="mods"></param>
        /// <returns></returns>
        public static string GetModsString(ModIdentifier mods)
        {
            if (mods == 0)
                return "None";

            var modStrings = new List<string>();

            foreach (ModIdentifier mod in Enum.GetValues(typeof(ModIdentifier)))
            {
                if (!mods.HasFlag(mod))
                    continue;

                switch (mod)
                {
                    case ModIdentifier.NoSliderVelocity:
                        modStrings.Add("NSV");
                        break;
                    case ModIdentifier.Speed05X:
                        modStrings.Add("0.5x");
                        break;
                    case ModIdentifier.Speed06X:
                        modStrings.Add("0.6x");
                        break;
                    case ModIdentifier.Speed07X:
                        modStrings.Add("0.7x");
                        break;
                    case ModIdentifier.Speed08X:
                        modStrings.Add("0.8x");
                        break;
                    case ModIdentifier.Speed09X:
                        modStrings.Add("0.9x");
                        break;
                    case ModIdentifier.Speed11X:
                        modStrings.Add("1.1x");
                        break;
                    case ModIdentifier.Speed12X:
                        modStrings.Add("1.2x");
                        break;
                    case ModIdentifier.Speed13X:
                        modStrings.Add("1.3x");
                        break;
                    case ModIdentifier.Speed14X:
                        modStrings.Add("1.4x");
                        break;
                    case ModIdentifier.Speed15X:
                        modStrings.Add("1.5x");
                        break;
                    case ModIdentifier.Speed16X:
                        modStrings.Add("1.6x");
                        break;
                    case ModIdentifier.Speed17X:
                        modStrings.Add("1.7x");
                        break;
                    case ModIdentifier.Speed18X:
                        modStrings.Add("1.8x");
                        break;
                    case ModIdentifier.Speed19X:
                        modStrings.Add("1.9x");
                        break;
                    case ModIdentifier.Speed20X:
                        modStrings.Add("2.0x");
                        break;
                    case ModIdentifier.Strict:
                        modStrings.Add("Strict");
                        break;
                    case ModIdentifier.Chill:
                        modStrings.Add("Chill");
                        break;
                    case ModIdentifier.NoPause:
                        modStrings.Add("No Pause");
                        break;
                    case ModIdentifier.Autoplay:
                        modStrings.Add("Autoplay");
                        break;
                    case ModIdentifier.Paused:
                        modStrings.Add("Paused");
                        break;
                    case ModIdentifier.NoFail:
                        modStrings.Add("No Fail");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Short string for ModIdentifier: {mod} does not exist.");
                }
            }

            return string.Join(", ", modStrings);
        }

        /// <summary>
        ///     Gets the audio rate from selected mods.
        /// </summary>
        /// <param name="mods"></param>
        /// <returns></returns>
        public static float GetRateFromMods(ModIdentifier mods)
        {
            // The rate of the audio.
            var rate = 1.0f;

            // Map mods to rate.
            if (mods.HasFlag(ModIdentifier.None))
                rate = 1.0f;
            else if (mods.HasFlag(ModIdentifier.Speed05X))
                rate = 0.5f;
            else if (mods.HasFlag(ModIdentifier.Speed06X))
                rate = 0.6f;
            else if (mods.HasFlag(ModIdentifier.Speed07X))
                rate = 0.7f;
            else if (mods.HasFlag(ModIdentifier.Speed08X))
                rate = 0.8f;
            else if (mods.HasFlag(ModIdentifier.Speed09X))
                rate = 0.9f;
            else if (mods.HasFlag(ModIdentifier.Speed11X))
                rate = 1.1f;
            else if (mods.HasFlag(ModIdentifier.Speed12X))
                rate = 1.2f;
            else if (mods.HasFlag(ModIdentifier.Speed13X))
                rate = 1.3f;
            else if (mods.HasFlag(ModIdentifier.Speed14X))
                rate = 1.4f;
            else if (mods.HasFlag(ModIdentifier.Speed15X))
                rate = 1.5f;
            else if (mods.HasFlag(ModIdentifier.Speed16X))
                rate = 1.6f;
            else if (mods.HasFlag(ModIdentifier.Speed17X))
                rate = 1.7f;
            else if (mods.HasFlag(ModIdentifier.Speed18X))
                rate = 1.8f;
            else if (mods.HasFlag(ModIdentifier.Speed19X))
                rate = 1.9f;
            else if (mods.HasFlag(ModIdentifier.Speed20X))
                rate = 2.0f;

            return rate;
        }

        /// <summary>
        ///     Gets a speed mod id from a given rate.
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static ModIdentifier GetModsFromRate(float rate)
        {
            switch (rate)
            {
                case 0.5f:
                    return ModIdentifier.Speed05X;
                case 0.6f:
                    return ModIdentifier.Speed06X;
                case 0.7f:
                    return ModIdentifier.Speed07X;
                case 0.8f:
                    return ModIdentifier.Speed08X;
                case 0.9f:
                    return ModIdentifier.Speed09X;
                case 1.1f:
                    return ModIdentifier.Speed11X;
                case 1.2f:
                    return ModIdentifier.Speed12X;
                case 1.3f:
                    return ModIdentifier.Speed13X;
                case 1.4f:
                    return ModIdentifier.Speed14X;
                case 1.5f:
                    return ModIdentifier.Speed15X;
                case 1.6f:
                    return ModIdentifier.Speed16X;
                case 1.7f:
                    return ModIdentifier.Speed17X;
                case 1.8f:
                    return ModIdentifier.Speed18X;
                case 1.9f:
                    return ModIdentifier.Speed19X;
                case 2.0f:
                    return ModIdentifier.Speed20X;
            }

            return ModIdentifier.None;
        }
    }
}
