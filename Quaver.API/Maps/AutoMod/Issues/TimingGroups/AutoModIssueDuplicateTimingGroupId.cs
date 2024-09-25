using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.AutoMod.Issues.TimingGroups
{
    public class AutoModIssueDuplicateTimingGroupId : AutoModIssue
    {
        /// <summary>
        /// </summary>
        public string Id { get; }

        public AutoModIssueDuplicateTimingGroupId(string id) : base(
            AutoModIssueLevel.Critical)
        {
            Id = id;
            Text = $"Duplicate timing group ID '{id}'.";
        }

        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.TimingGroups;
    }
}