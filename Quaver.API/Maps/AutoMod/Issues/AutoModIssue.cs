namespace Quaver.API.Maps.AutoMod.Issues
{
    public abstract class AutoModIssue
    {
        /// <summary>
        /// </summary>
        public AutoModIssueLevel Level { get; }

        public abstract AutoModIssueCategory Category { get; protected set; }

        /// <summary>
        ///     The text that will be displayed for this detection
        /// </summary>
        public string Text { get; protected set; }

        /// <summary>
        /// </summary>
        /// <param name="level"></param>
        public AutoModIssue(AutoModIssueLevel level) => Level = level;
    }
}