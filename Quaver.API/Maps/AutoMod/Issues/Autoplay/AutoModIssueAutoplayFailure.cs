namespace Quaver.API.Maps.AutoMod.Issues.Autoplay
{
    public class AutoModIssueAutoplayFailure : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.HitObjects;
        public AutoModIssueAutoplayFailure() : base(AutoModIssueLevel.Critical)
            => Text = $"Autoplay is unable to achieve a 100% score.";
    }
}