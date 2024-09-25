using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.HitObjects
{
    public class AutoModIssueObjectInvalidTimingGroup : AutoModIssue
    {
        /// <summary>
        /// </summary>
        public HitObjectInfo HitObjectInfo { get; }

        public AutoModIssueObjectInvalidTimingGroup(HitObjectInfo hitObjectInfo) : base(AutoModIssueLevel.Critical)
        {
            HitObjectInfo = hitObjectInfo;
            Text = $"The note in column {hitObjectInfo.Lane} at {hitObjectInfo.StartTime} has " +
                   $"invalid timing group id '{hitObjectInfo.TimingGroup}'.";
        }

        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.HitObjects;
    }
}