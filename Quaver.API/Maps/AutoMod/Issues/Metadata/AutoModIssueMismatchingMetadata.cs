namespace Quaver.API.Maps.AutoMod.Issues.Metadata
{
    public class AutoModIssueMismatchingMetadata : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.Metadata;

        public MetadataField Field { get; }

        public AutoModIssueMismatchingMetadata(MetadataField field) : base(AutoModIssueLevel.Ranking)
        {
            Field = field;
            Text = $"The '{Field}' metadata does not match for each difficulty.";
        }
    }

    public enum MetadataField
    {
        Artist,
        Title,
        Source,
        Tags
    }
}