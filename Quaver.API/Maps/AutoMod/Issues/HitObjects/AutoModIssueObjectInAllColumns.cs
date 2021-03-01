using System.Collections.Generic;

namespace Quaver.API.Maps.AutoMod.Issues.HitObjects
{
    public class AutoModIssueObjectInAllColumns : AutoModIssue
    {
        public List<int> MissingColumns { get; }

        public AutoModIssueObjectInAllColumns(List<int> missingColumns) : base(AutoModIssueLevel.Ranking)
        {
            MissingColumns = missingColumns;
            Text = $"The map is missing objects in columns: {string.Join(", ", MissingColumns)}";
        }
    }
}