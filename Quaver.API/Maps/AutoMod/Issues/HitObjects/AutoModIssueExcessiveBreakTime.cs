using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.HitObjects
{
    public class AutoModIssueExcessiveBreakTime : AutoModIssue
    {
        public HitObjectInfo HitObject { get; }

        public AutoModIssueExcessiveBreakTime(HitObjectInfo hitObject) : base(AutoModIssueLevel.Ranking)
        {
            HitObject = hitObject;
            Text = $"The map contains 30 seconds or more of break time at: {HitObject.StartTime} ms.";
        }
    }
}