using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.TimingPoints
{
    public class AutoModIssueTimingPointAfterAudioEnd : AutoModIssue
    {
        public TimingPointInfo TimingPoint { get; }

        public AutoModIssueTimingPointAfterAudioEnd(TimingPointInfo timingPoint) : base(AutoModIssueLevel.Warning)
        {
            TimingPoint = timingPoint;
            Text = $"The timing point at: {TimingPoint.StartTime} is placed after the audio ends.";
        }
    }
}