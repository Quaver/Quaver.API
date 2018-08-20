using System;
using Quaver.API.Enums;

namespace Quaver.API.Helpers
{
    public static class ModeHelper
    {
        /// <summary>
        ///     Converts game mode to short hand version.
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string ToShortHand(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Keys4:
                    return "4K";
                case GameMode.Keys7:
                    return "7K";
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}