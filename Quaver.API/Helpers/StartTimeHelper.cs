// SPDX-License-Identifier: MPL-2.0
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Helpers
{
    public static class StartTimeHelper
    {
        public static int IndexAtTime<T>(this IReadOnlyList<T> list, float time)
            where T : IStartTime
        {
            var left = 0;
            var right = list.Count - 1;

            while (left <= right)
                if ((left + ((right - left) / 2)) is var mid && list[mid].StartTime <= time)
                    left = mid + 1;
                else
                    right = mid - 1;

            return right;
        }

        public static int IndexAtTimeBefore<T>(this IReadOnlyList<T> list, float time)
            where T : IStartTime =>
            IndexAtTime(list, Before(time));

        public static T AtTime<T>(this IReadOnlyList<T> list, float time)
            where T : IStartTime
        {
            var i = list.IndexAtTime(time);
            return i is -1 ? default : list[i];
        }

        public static T AtTimeBefore<T>(this IReadOnlyList<T> list, float time)
            where T : IStartTime =>
            AtTime(list, Before(time));

        // Thanks to https://stackoverflow.com/a/10426033 for the implementation.
        public static float After(float time)
        {
            // NaNs and positive infinity map to themselves.
            if (float.IsNaN(time) || float.IsPositiveInfinity(time))
                return time;

            // 0.0 and -0.0 both map to the smallest +ve float.
            if (time is 0)
                return float.Epsilon;

            unsafe
            {
                // Slightly evil bit hack.
                _ = time > 0 ? ++*(int*)&time : --*(int*)&time;
            }

            return time;
        }

        public static float Before(float time) => -After(-time);
    }
}
