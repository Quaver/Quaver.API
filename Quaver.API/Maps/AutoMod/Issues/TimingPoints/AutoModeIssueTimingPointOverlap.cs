using System.Linq;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.TimingPoints
{
    public class AutoModeIssueTimingPointOverlap : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.TimingPoints;

        public TimingPointInfo[] TimingPoints { get; }

        public AutoModeIssueTimingPointOverlap(TimingPointInfo[] timingPoints) : base(AutoModIssueLevel.Warning)
        {
            TimingPoints = timingPoints;
            Text = $"There are overlapping timing points at: {TimingPoints.First().StartTime} ms.";
        }
    }
}