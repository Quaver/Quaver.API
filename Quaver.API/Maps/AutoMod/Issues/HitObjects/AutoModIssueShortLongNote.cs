using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.HitObjects
{
    public class AutoModIssueShortLongNote : AutoModIssue
    {
        public HitObjectInfo HitObject { get; }

        public AutoModIssueShortLongNote(HitObjectInfo hitObject) : base(AutoModIssueLevel.Ranking)
        {
            HitObject = hitObject;
            Text = $"The long note in column {HitObject.Lane} at {HitObject.StartTime} is less than {AutoMod.ShortLongNoteThreshold} ms.";
        }
    }
}