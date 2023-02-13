namespace Quaver.API.Maps.AutoMod.Issues.Metadata
{
    public class AutoModIssueNonCommaSeparatedTags : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.Metadata;

        public AutoModIssueNonCommaSeparatedTags() : base(AutoModIssueLevel.Ranking)
            => Text = $"The tags metadata field must be comma separated.";
    }
}