namespace Quaver.API.Maps.AutoMod.Issues.Audio
{
    public class AutoModIssueAudioFormat : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.Files;

        public AutoModIssueAudioFormat() : base(AutoModIssueLevel.Ranking)
            => Text = $"The audio file format must be mp3.";
    }
}