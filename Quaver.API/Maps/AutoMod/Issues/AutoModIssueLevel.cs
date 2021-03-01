namespace Quaver.API.Maps.AutoMod.Issues
{
    public enum AutoModIssueLevel
    {
        /// <summary>
        ///     Small warnings that could be considered negligible by the author.
        /// </summary>
        Warning,

        /// <summary>
        ///     Critical errors that have an effect on playability.
        /// </summary>
        Critical,

        /// <summary>
        ///     Errors in the map that aren't acceptable by the ranking criteria.
        /// </summary>
        Ranking
    }
}