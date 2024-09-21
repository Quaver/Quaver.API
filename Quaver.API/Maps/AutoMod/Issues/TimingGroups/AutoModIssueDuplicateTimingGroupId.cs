using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.TimingGroups
{
    public class AutoModIssueDuplicateTimingGroupId : AutoModIssue
    {
        /// <summary>
        /// </summary>
        public TimingGroup FirstGroup { get; }

        /// <summary>
        /// </summary>
        public TimingGroup DuplicateGroup { get; }

        public AutoModIssueDuplicateTimingGroupId(TimingGroup firstGroup, TimingGroup duplicateGroup) : base(
            AutoModIssueLevel.Critical)
        {
            FirstGroup = firstGroup;
            DuplicateGroup = duplicateGroup;
            Text = $"Duplicate timing group ID '{firstGroup.Id}' " +
                   $"('{firstGroup.GetType().Name}' and '{duplicateGroup.GetType().Name}').";
        }

        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.TimingGroups;
    }
}