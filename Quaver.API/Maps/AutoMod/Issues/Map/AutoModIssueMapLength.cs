namespace Quaver.API.Maps.AutoMod.Issues.Map
{
    public class AutoModIssueMapLength : AutoModIssue
    {
        public AutoModIssueMapLength() : base(AutoModIssueLevel.Ranking)
            => Text = $"The map must be at least 45 seconds long.";
    }
}