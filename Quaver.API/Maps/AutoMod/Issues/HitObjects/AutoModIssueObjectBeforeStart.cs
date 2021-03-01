using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.HitObjects
{
    public class AutoModIssueObjectBeforeStart : AutoModIssue
    {
        private HitObjectInfo HitObject { get; }

        public AutoModIssueObjectBeforeStart(HitObjectInfo hitObject) : base(AutoModIssueLevel.Critical)
        {
            HitObject = hitObject;
            Text = $"The object in column {HitObject.Lane} at {HitObject.StartTime} is placed before the start of the track.";
        }
    }
}