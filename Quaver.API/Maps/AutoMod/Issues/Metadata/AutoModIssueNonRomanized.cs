namespace Quaver.API.Maps.AutoMod.Issues.Metadata
{
    public class AutoModIssueNonRomanized : AutoModIssue
    {
        public AutoModIssueNonRomanized(string field) : base(AutoModIssueLevel.Warning)
            => Text = $"The '{field}' metadata field contains non-romanized characters.";
    }
}