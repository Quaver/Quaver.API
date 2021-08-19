namespace Quaver.API.Maps.AutoMod.Issues.Background
{
    public class AutoModIssueBackgroundResolution : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.Files;

        public AutoModIssueBackgroundResolution() : base(AutoModIssueLevel.Ranking)
            => Text = $"The background resolution must be at least 1280x720.";
    }
}