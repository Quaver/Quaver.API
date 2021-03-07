namespace Quaver.API.Maps.AutoMod.Issues.Autoplay
{
    public class AutoModIssueAutoplayFailure : AutoModIssue
    {
        public AutoModIssueAutoplayFailure() : base(AutoModIssueLevel.Critical)
            => Text = $"Autoplay is unable to achieve a 100% score.";
    }
}