// SPDX-License-Identifier: MPL-2.0
using System;

namespace Quaver.API.Helpers
{
    /// <summary>
    ///     Represents a sequence of items separated by a separator.
    /// </summary>
    /// <typeparam name="T">The type of items.</typeparam>
    public readonly ref struct Drain<T>
        where T : IEquatable<T>
    {
        readonly ReadOnlySpan<T> _remaining;

        readonly T _separator;

        /// <summary>
        ///     Creates a new <see cref="Drain{T}"/> instance.
        /// </summary>
        /// <param name="remaining">The remaining items.</param>
        /// <param name="separator">The separator.</param>
        public Drain(ReadOnlySpan<T> remaining, T separator)
        {
            _remaining = remaining;
            _separator = separator;
        }

        /// <summary>
        ///     Collects the next <see cref="ReadOnlySpan{T}"/>,
        ///     while also initializing the next <see cref="Drain{T}"/>.
        /// </summary>
        /// <param name="next">The next <see cref="ReadOnlySpan{T}"/>.</param>
        /// <param name="rest">The rest as <see cref="Drain{T}"/>.</param>
        public void Deconstruct(out ReadOnlySpan<T> next, out Drain<T> rest)
        {
            var i = _remaining.IndexOf(_separator);

            if (i is -1)
            {
                next = _remaining;
                rest = default;
                return;
            }

            next = _remaining[..i];
            rest = new Drain<T>(_remaining[(i + 1)..], _separator);
        }
    }
}
