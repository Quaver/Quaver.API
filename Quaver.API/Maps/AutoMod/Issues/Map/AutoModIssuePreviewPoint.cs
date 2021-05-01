namespace Quaver.API.Maps.AutoMod.Issues.Map
{
    public class AutoModIssuePreviewPoint : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.HitObjects;

        public AutoModIssuePreviewPoint() : base(AutoModIssueLevel.Ranking)
            => Text = $"The map must have a valid preview point.";
    }
}