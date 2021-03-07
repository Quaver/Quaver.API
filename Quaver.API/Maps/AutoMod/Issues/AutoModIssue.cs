namespace Quaver.API.Maps.AutoMod.Issues
{
    public class AutoModIssue
    {
        /// <summary>
        /// </summary>
        public AutoModIssueLevel Level { get; }

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