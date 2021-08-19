using Quaver.API.Helpers;

namespace Quaver.API.Maps.AutoMod.Issues.Metadata
{
    public class AutoModIssueMultiModeDiffName : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.Metadata;

        public Qua Map { get; }

        public AutoModIssueMultiModeDiffName(Qua map) : base(AutoModIssueLevel.Ranking)
        {
            Map = map;

            Text = $"The difficulty '{Map.DifficultyName}' should contain '{ModeHelper.ToShortHand(Map.Mode)}'";
        }
    }
}