namespace Quaver.API.Maps.AutoMod.Issues.Audio
{
    public class AutoModIssueAudioBitrate : AutoModIssue
    {
        public AutoModIssueAudioBitrate() : base(AutoModIssueLevel.Ranking)
            => Text = $"The mp3's bitrate must be 192kbps or lower.";
    }
}