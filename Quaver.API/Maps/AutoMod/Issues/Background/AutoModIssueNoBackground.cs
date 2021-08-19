namespace Quaver.API.Maps.AutoMod.Issues.Background
{
    public class AutoModIssueNoBackground : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.Files;
        
        public AutoModIssueNoBackground() : base(AutoModIssueLevel.Ranking)
            => Text = $"This map does not contain a background image.";
    }
}