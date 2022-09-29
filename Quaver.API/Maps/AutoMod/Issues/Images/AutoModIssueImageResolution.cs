namespace Quaver.API.Maps.AutoMod.Issues.Images
{
    public class AutoModIssueImageResolution : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.Files;

        public string Item { get; }
        public int MinWidth { get; }
        public int MinHeight { get; }
        public int MaxWidth { get; }
        public int MaxHeight { get; }

        public AutoModIssueImageResolution(string item, int minWidth, int minHeight, int maxWidth, int maxHeight) : base(AutoModIssueLevel.Ranking)
        {
            Item = item;
            MinWidth = minWidth;
            MinHeight = minHeight;
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;

            Text = $"The {Item} resolution must be at least {MinWidth}x{MinHeight} and at most {MaxWidth}x{MaxHeight}.";
        }
    }
}