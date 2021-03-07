using System.Linq;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.ScrollVelocities
{
    public class AutoModIssueScrollVelocityOverlap : AutoModIssue
    {
        public SliderVelocityInfo[] ScrollVelocities { get; }

        public AutoModIssueScrollVelocityOverlap(SliderVelocityInfo[] scrollVelocities) : base(AutoModIssueLevel.Warning)
        {
            ScrollVelocities = scrollVelocities;

            Text = $"There are {ScrollVelocities.Length} scroll velocities overlapping at: " +
                   $"{ScrollVelocities.First().StartTime}";
        }
    }
}