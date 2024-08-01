/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2019 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Helpers
{
    public static class StartTimeHelper
    {
        // If the framework supports it, we want to use Random.Shared since it is much faster.
        static readonly Random _rng = Shared()?.GetMethod?.Invoke(null, null) as Random ?? new Random();

        public static void InsertSorted<T>(this List<T> list, T element)
            where T : IStartTime
        {
            var i = list.BinarySearch(element);
            list.Insert(i >= 0 ? i : ~i, element);
        }

        public static void InsertSorted<T>(this List<T> list, IEnumerable<T> elements)
            where T : IStartTime
        {
            // Thanks to @WilliamQiufeng for going through the trouble of benchmarking
            // to find the optimal capacity and count for our use case.
            const int MaximumCapacity = 128;

            const int MinimumCount = 128;

            // ReSharper disable PossibleMultipleEnumeration
            switch (TryCount(elements))
            {
                case 0: break;
                case 1:
                    InsertSorted(list, elements.First());
                    break;
                case { } count when count <= MinimumCount && list.Capacity >= MaximumCapacity:
                    var capacity = list.Capacity;

                    if (capacity - list.Count < count)
                        list.Capacity = Math.Max(capacity * 2, capacity + count);

                    InsertSortedList(list, elements, count);
                    break;
                default: // If the list ends up becoming large, it is no longer worth it to find the insertion.
                    list.AddRange(elements);
                    list.HybridSort();
                    break;
            }
        }

        /// <summary>
        ///     Sorts the list.
        /// </summary>
        /// <remarks><para>
        ///     This method is intended to be used on collections that have a decent chance of being nearly sorted.
        ///     It uses Insertion Sort, but falls back on Quick Sort if that algorithm takes too long.
        ///     The sorting is stable, meaning that the order for equal elements are preserved.
        /// </para></remarks>
        /// <typeparam name="T">The type of list to sort.</typeparam>
        /// <param name="list">The list to sort.</param>
        public static void HybridSort<T>(this List<T> list)
            where T : IStartTime
        {
            var maxBacktracking = list.Count * (int)Math.Log(list.Count, 2);

            for (var i = 1; i < list.Count; i++)
            {
                var j = i;

                while (j > 0 && list[j - 1].StartTime > list[j].StartTime)
                {
                    (list[j], list[j - 1]) = (list[j - 1], list[j]);
                    j--;

                    if (--maxBacktracking > 0)
                        continue;

                    // Insertion Sort is deemed to take too long, let's fall back to Quick Sort.
                    QuickSort(list, 0, list.Count - 1);
                    return;
                }
            }
        }

        // Ideally would be IReadOnlyList<T> to indicate no mutation,
        // but unfortunately IList<T> doesn't implement IReadOnlyList<T>.
        [Pure]
        public static int IndexAtTime<T>(this List<T> list, float time)
            where T : IStartTime
        {
            var left = 0;
            var right = list.Count - 1;

            while (left <= right)
                if (left + ((right - left) / 2) is var mid && list[mid].StartTime <= time)
                    left = mid + 1;
                else
                    right = mid - 1;

            return right;
        }

        [Pure]
        public static int IndexAtTimeBefore<T>(this List<T> list, float time)
            where T : IStartTime =>
            IndexAtTime(list, Before(time));

        [Pure]
        public static T AtTime<T>(this List<T> list, float time)
            where T : IStartTime
        {
            var i = list.IndexAtTime(time);
            return i is -1 ? default : list[i];
        }

        [Pure]
        public static T AtTimeBefore<T>(this List<T> list, float time)
            where T : IStartTime =>
            AtTime(list, Before(time));

        // Thanks to https://stackoverflow.com/a/10426033 for the implementation.
        [Pure]
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

        [Pure]
        public static float Before(float time) => -After(-time);

        // For interfaces, indexers are generally more performant, hence the suppression below.
        private static void InsertSortedList<T>(List<T> list, IEnumerable<T> elements, int count)
            where T : IStartTime
        {
            switch (elements)
            {
                case IList<T> e:
                    for (var i = 0; i < count; i++)
                        InsertSorted(list, e[i]);

                    break;
                case IReadOnlyList<T> e:
                    for (var i = 0; i < count; i++)
                        InsertSorted(list, e[i]);

                    break;
                default:
                    foreach (var e in elements)
                        InsertSorted(list, e);

                    break;
            }
        }

        // ReSharper disable once CognitiveComplexity SuggestBaseTypeForParameter
        private static void QuickSort<T>(List<T> list, int leftIndex, int rightIndex)
            where T : IStartTime
        {
            while (true)
            {
                var i = leftIndex;
                var j = rightIndex;
                var pivot = list[_rng.Next(leftIndex, rightIndex + 1)];

                while (i <= j)
                {
                    while (list[i].StartTime < pivot.StartTime)
                        i++;

                    while (list[j].StartTime > pivot.StartTime)
                        j--;

                    if (i > j)
                        continue;

                    (list[i], list[j]) = (list[j], list[i]);
                    i++;
                    j--;
                }

                if (leftIndex < j)
                    QuickSort(list, leftIndex, j);

                if (i >= rightIndex)
                    break;

                leftIndex = i;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        private static int? TryCount<T>(IEnumerable<T> enumerable) =>
            enumerable switch
            {
                string c => c.Length,
                ICollection c => c.Count,
                ICollection<T> c => c.Count,
                IReadOnlyCollection<T> c => c.Count,
                _ => null,
            };

        private static PropertyInfo Shared() =>
            typeof(Random).GetProperty(
                nameof(Shared),
                BindingFlags.Public | BindingFlags.Static,
                Type.DefaultBinder,
                typeof(Random),
                Type.EmptyTypes,
                Array.Empty<ParameterModifier>()
            );
    }
}
