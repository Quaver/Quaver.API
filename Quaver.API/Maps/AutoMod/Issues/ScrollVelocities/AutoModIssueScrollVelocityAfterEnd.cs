using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.ScrollVelocities
{
    public class AutoModIssueScrollVelocityAfterEnd : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.ScrollVelocities;

        public SliderVelocityInfo ScrollVelocity { get; }

        public AutoModIssueScrollVelocityAfterEnd(SliderVelocityInfo sv) : base(AutoModIssueLevel.Warning)
        {
            ScrollVelocity = sv;
            Text = $"The scroll velocity at: {ScrollVelocity.StartTime} is placed after the audio ends.";
        }
    }
}