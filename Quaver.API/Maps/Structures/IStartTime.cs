// SPDX-License-Identifier: MPL-2.0
using MoonSharp.Interpreter.Interop;

namespace Quaver.API.Maps.Structures
{
    public interface IStartTime
    {
        public float StartTime { get; [MoonSharpVisible(false)] set; }
    }
}
