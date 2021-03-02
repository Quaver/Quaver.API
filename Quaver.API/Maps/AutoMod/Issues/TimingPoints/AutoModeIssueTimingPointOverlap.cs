using System.Linq;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.TimingPoints
{
    public class AutoModeIssueTimingPointOverlap : AutoModIssue
    {
        public TimingPointInfo[] TimingPoints { get; }

        public AutoModeIssueTimingPointOverlap(TimingPointInfo[] timingPoints) : base(AutoModIssueLevel.Warning)
        {
            TimingPoints = timingPoints;
            Text = $"There are {TimingPoints.Length} overlapping timing points at: {TimingPoints.First().StartTime} ms.";
        }
    }
}