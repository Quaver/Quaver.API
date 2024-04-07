using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.Stepmania;

public class StepBpmOrStop
{
    public StepFileBPM Bpm { get; }
    public StepFileStop Stop { get; }

    public StepBpmOrStop(StepFileBPM bpm)
    {
        Bpm = bpm;
    }

    public StepBpmOrStop(StepFileStop stop)
    {
        Stop = stop;
    }

    public bool IsBpm => Bpm != null;
    public bool IsStop => Stop != null;
    
    
    public float Beat => IsBpm ? Bpm.Beat : Stop.Beat;

    private sealed class BeatRelationalComparer : IComparer<StepBpmOrStop>
    {
        public int Compare(StepBpmOrStop x, StepBpmOrStop y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return x.Beat.CompareTo(y.Beat);
        }
    }

    public static IComparer<StepBpmOrStop> BeatComparer { get; } = new BeatRelationalComparer();
}