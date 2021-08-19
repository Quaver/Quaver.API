namespace Quaver.API.Maps.AutoMod.Issues.Mapset
{
    public class AutoModIssueMapsetSpreadLength : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.Mapset;

        public AutoModIssueMapsetSpreadLength() : base(AutoModIssueLevel.Ranking)
            => Text = $"A single-set mapset must be at least 2:30 in length.";
    }
}