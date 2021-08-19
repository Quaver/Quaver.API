using System.Linq;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.ScrollVelocities
{
    public class AutoModIssueScrollVelocityOverlap : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.ScrollVelocities;

        public SliderVelocityInfo[] ScrollVelocities { get; }

        public AutoModIssueScrollVelocityOverlap(SliderVelocityInfo[] scrollVelocities) : base(AutoModIssueLevel.Warning)
        {
            ScrollVelocities = scrollVelocities;

            Text = $"There are scroll velocities overlapping at: " +
                   $"{ScrollVelocities.First().StartTime}";
        }
    }
}