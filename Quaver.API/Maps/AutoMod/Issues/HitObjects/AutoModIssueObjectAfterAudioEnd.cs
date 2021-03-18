using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.HitObjects
{
    public class AutoModIssueObjectAfterAudioEnd : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.HitObjects;

        public HitObjectInfo HitObject { get; }

        public AutoModIssueObjectAfterAudioEnd(HitObjectInfo hitObject) : base(AutoModIssueLevel.Critical)
        {
            HitObject = hitObject;
            Text = $"The object at: {HitObject.StartTime} in column {HitObject.Lane} is placed after the audio ends.";
        }
    }
}