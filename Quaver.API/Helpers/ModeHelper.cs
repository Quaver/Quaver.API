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
    public static class ModeHelper
    {
        public static int MaxKeyCount => AllModes.Length;

        /// <summary>
        ///     Converts game mode to short hand version.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="hasScratch"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToShortHand(GameMode mode, bool hasScratch = false)
        {
            string res;
            switch (mode)
            {
                case GameMode.Keys4: res = "4K"; break;
                case GameMode.Keys7: res = "7K"; break;

                case GameMode.Keys1: res = "1K"; break;
                case GameMode.Keys2: res = "2K"; break;
                case GameMode.Keys3: res = "3K"; break;
                case GameMode.Keys5: res = "5K"; break;
                case GameMode.Keys6: res = "6K"; break;
                case GameMode.Keys8: res = "8K"; break;
                case GameMode.Keys9: res = "9K"; break;
                case GameMode.Keys10: res = "10K"; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            return res + (hasScratch ? "+1" : "");
        }

        /// <summary>
        ///     Converts the game mode into the long hand version
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToLongHand(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Keys4: return "4 Keys";
                case GameMode.Keys7: return "7 Keys";

                case GameMode.Keys1: return "1 Keys";
                case GameMode.Keys2: return "2 Keys";
                case GameMode.Keys3: return "3 Keys";
                case GameMode.Keys5: return "5 Keys";
                case GameMode.Keys6: return "6 Keys";
                case GameMode.Keys8: return "8 Keys";
                case GameMode.Keys9: return "9 Keys";
                case GameMode.Keys10: return "10 Keys";
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }

        public static int ToKeyCount(GameMode mode, bool hasScratch = false)
        {
            int res;
            switch (mode)
            {

                case GameMode.Keys4: res = 4; break;
                case GameMode.Keys7: res = 7; break;

                case GameMode.Keys1: res = 1; break;
                case GameMode.Keys2: res = 2; break;
                case GameMode.Keys3: res = 3; break;
                case GameMode.Keys5: res = 5; break;
                case GameMode.Keys6: res = 6; break;
                case GameMode.Keys8: res = 8; break;
                case GameMode.Keys9: res = 9; break;
                case GameMode.Keys10: res = 10; break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            return res + (hasScratch ? 1 : 0);
        }

        public static GameMode FromKeyCount(int keyCount)
        {
            switch (keyCount)
            {
                case 4: return GameMode.Keys4;
                case 7: return GameMode.Keys7;

                case 1: return GameMode.Keys1;
                case 2: return GameMode.Keys2;
                case 3: return GameMode.Keys3;
                case 5: return GameMode.Keys5;
                case 6: return GameMode.Keys6;
                case 8: return GameMode.Keys8;
                case 9: return GameMode.Keys9;
                case 10: return GameMode.Keys10;
                default: throw new ArgumentOutOfRangeException(nameof(keyCount), keyCount, null);
            }
        }

        public static bool IsKeyMode(GameMode mode)
        {
            // we only have keys gamemode for now...
            return true;
        }

        public static bool IsRanked(GameMode mode){
            switch (mode)
            {
                case GameMode.Keys4:
                case GameMode.Keys7:
                    return true;
                default:
                    return false;
            }
        }

        public static readonly GameMode[] AllModes = (GameMode[])Enum.GetValues(typeof(GameMode));
    }
}
