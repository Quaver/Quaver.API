using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

namespace Quaver.API.Maps.Structures
{
    [MoonSharpUserData]
    [Serializable]
    public class BookmarkInfo : IComparable<BookmarkInfo>, IStartTime
    {
        public int StartTime
        {
            get;
            [MoonSharpVisible(false)] set;
        }

        public string Note
        {
            get;
            [MoonSharpVisible(false)] set;
        }

        float IStartTime.StartTime
        {
            get => StartTime;
            set => StartTime = (int)value;
        }

        /// <inheritdoc />
        public int CompareTo(BookmarkInfo other) => StartTime.CompareTo(other.StartTime);

        private sealed class TimeNoteEqualityComparer : IEqualityComparer<BookmarkInfo>
        {
            public bool Equals(BookmarkInfo x, BookmarkInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.StartTime == y.StartTime && x.Note == y.Note;
            }

            public int GetHashCode(BookmarkInfo obj)
            {
                return HashCode.Combine(obj.StartTime, obj.Note);
            }
        }

        public static IEqualityComparer<BookmarkInfo> ByValueComparer { get; } = new TimeNoteEqualityComparer();
    }
}
