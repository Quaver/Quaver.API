using System.Linq;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.HitObjects
{
    public class AutoModIssueOverlappingObjects : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.HitObjects;

        public HitObjectInfo[] HitObjects { get; }

        public AutoModIssueOverlappingObjects(HitObjectInfo[] hitObjects) : base(AutoModIssueLevel.Critical)
        {
            HitObjects = hitObjects;
            Text = $"There are {HitObjects.Length} objects in column {HitObjects.First().Lane} at {HitObjects.First().StartTime} that overlap.";
        }
    }
}