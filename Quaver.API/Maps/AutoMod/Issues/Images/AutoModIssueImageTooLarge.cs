namespace Quaver.API.Maps.AutoMod.Issues.Images
{
    public class AutoModIssueImageTooLarge : AutoModIssue
    {
        public override AutoModIssueCategory Category { get; protected set; } = AutoModIssueCategory.Files;

        public string Item { get; }
        public int MaxSize { get; }

        public AutoModIssueImageTooLarge(string item, int maxSize) : base(AutoModIssueLevel.Ranking)
        {
            Item = item;
            MaxSize = maxSize;

            Text = $"The file size of the {Item} is too large. Must be less " +
                   $"than {maxSize / 1000000} MB.";
        }
    }
}