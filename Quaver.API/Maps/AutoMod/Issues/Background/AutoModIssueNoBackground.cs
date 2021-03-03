namespace Quaver.API.Maps.AutoMod.Issues.Background
{
    public class AutoModIssueNoBackground : AutoModIssue
    {
        public AutoModIssueNoBackground() : base(AutoModIssueLevel.Ranking)
            => Text = $"This map does not contain a background image.";
    }
}