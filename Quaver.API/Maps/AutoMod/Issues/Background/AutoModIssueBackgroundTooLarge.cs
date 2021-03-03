namespace Quaver.API.Maps.AutoMod.Issues.Background
{
    public class AutoModIssueBackgroundTooLarge : AutoModIssue
    {
        public AutoModIssueBackgroundTooLarge() : base(AutoModIssueLevel.Ranking)
            => Text = $"The file size of the background is too large. Must be less " +
                      $"than {AutoMod.MaxBackgroundFileSize / 1000000} MB.";
    }
}