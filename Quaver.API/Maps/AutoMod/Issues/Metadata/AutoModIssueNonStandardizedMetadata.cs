namespace Quaver.API.Maps.AutoMod.Issues.Metadata
{
    public class AutoModIssueNonStandardizedMetadata : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.Metadata;

        public string Field { get; }
        public string OldTerm { get; }
        public string NewTerm { get; }

        public AutoModIssueNonStandardizedMetadata(string field, string oldTerm, string newTerm) : base(AutoModIssueLevel.Ranking)
        {
            Field = field;
            OldTerm = oldTerm;
            NewTerm = newTerm;
            Text = $"The '{Field}' metadata contains '{OldTerm}', which should be standardized to '{NewTerm}' instead.";
        }
    }
}