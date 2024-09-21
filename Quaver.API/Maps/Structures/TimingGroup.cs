using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MoonSharp.Interpreter;

namespace Quaver.API.Maps.Structures
{
    /// <summary>
    ///     This is a base class that should not be serialized directly.
    ///     A timing group has custom properties that can be applied to an entire group of <see cref="HitObjectInfo"/>s.
    ///     You can add a <see cref="HitObjectInfo"/> to the group by setting <see cref="HitObjectInfo.TimingGroup"/>
    ///     to the <see cref="Id"/> of the desired <see cref="TimingGroup"/>.
    /// </summary>
    [MoonSharpUserData]
    public abstract class TimingGroup
    {
        private const string IdRegexPattern = "^[a-zA-Z0-9_]*$";

        /// <summary>
        ///     ID of the timing group. It should consist of alphanumerical characters or underscore
        /// </summary>
        /// <exception cref="FormatException"></exception>
        public string Id
        {
            get => _id;
            [MoonSharpHidden]
            set
            {
                if (!IdRegex.IsMatch(value))
                    throw new FormatException($"Invalid timing group ID: {value}");
                _id = value;
            }
        }

        private static readonly Regex IdRegex = new Regex(IdRegexPattern);
        private string _id;

        /// <summary>
        /// </summary>
        private sealed class IdEqualityComparer : IEqualityComparer<TimingGroup>
        {
            public bool Equals(TimingGroup x, TimingGroup y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (x is null)
                {
                    return false;
                }

                if (y is null)
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return x.Id == y.Id;
            }

            public int GetHashCode(TimingGroup obj)
            {
                return (obj.Id != null ? obj.Id.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// </summary>
        public static IEqualityComparer<TimingGroup> IdComparer { get; } = new IdEqualityComparer();
    }
}