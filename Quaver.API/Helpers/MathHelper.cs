using System;

namespace Quaver.API.Helpers
{
    public static class MathHelper
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            return val.CompareTo(max) > 0 ? max : val;
        }

        /// <summary>
        ///     Mirrors the first <see cref="bits"/> bits of <see cref="x"/>.
        ///     https://stackoverflow.com/a/4245986/23723435
        /// </summary>
        /// <param name="x"></param>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static uint Reverse(uint x, int bits)
        {
            x = ((x & 0x55555555) << 1) | ((x & 0xAAAAAAAA) >> 1); // Swap _<>_
            x = ((x & 0x33333333) << 2) | ((x & 0xCCCCCCCC) >> 2); // Swap __<>__
            x = ((x & 0x0F0F0F0F) << 4) | ((x & 0xF0F0F0F0) >> 4); // Swap ____<>____
            x = ((x & 0x00FF00FF) << 8) | ((x & 0xFF00FF00) >> 8); // Swap ...
            x = ((x & 0x0000FFFF) << 16) | ((x & 0xFFFF0000) >> 16); // Swap ...
            return x >> (32 - bits);
        }
    }
}