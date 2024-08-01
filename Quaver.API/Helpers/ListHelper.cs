/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Quaver.API.Helpers
{
    public static class ListHelper
    {
        private static class Cache<T>
        {
            public static Converter<List<T>, T[]> Converter { get; } = typeof(List<T>)
               .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
               .FirstOrDefault(x => x.FieldType == typeof(T[])) is { } method
                ? CreateGetter(method)
                : x => x.ToArray();

            private static Converter<List<T>, T[]> CreateGetter(FieldInfo field)
            {
                var name = $"{field.DeclaringType?.FullName}.get_{field.Name}";
                var getter = new DynamicMethod(name, typeof(T[]), new[] { typeof(List<T>) }, true);
                var il = getter.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldfld, field);
                il.Emit(OpCodes.Ret);
                return (Converter<List<T>, T[]>)getter.CreateDelegate(typeof(Converter<List<T>, T[]>));
            }
        }

        /// <summary>
        ///     Gets the underlying <see cref="Array"/> of the <see cref="List{T}"/>.
        /// </summary>
        /// <remarks><para>
        ///     Be careful when using this method as the <see cref="List{T}"/> cannot safeguard
        ///     against underlying mutations or out-of-bounds reading within its capacity.
        /// </para></remarks>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetUnderlyingArray<T>(List<T> list) => Cache<T>.Converter(list);
    }
}
