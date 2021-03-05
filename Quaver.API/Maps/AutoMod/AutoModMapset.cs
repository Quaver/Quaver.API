using System.Collections.Generic;
using System.Linq;
using Quaver.API.Maps.AutoMod.Issues;
using Quaver.API.Maps.AutoMod.Issues.Mapset;

namespace Quaver.API.Maps.AutoMod
{
    public class AutoModMapset
    {
        /// <summary>
        /// </summary>
        private List<Qua> Maps { get; }

        /// <summary>
        ///     Contains the AutoMod for each map
        /// </summary>
        public Dictionary<Qua, AutoMod> Mods { get; private set; }

        /// <summary>
        ///     Issues for the mapset itself (not individual maps)
        /// </summary>
        public List<AutoModIssue> Issues { get; } = new List<AutoModIssue>();

        /// <summary>
        /// </summary>
        /// <param name="maps"></param>
        public AutoModMapset(List<Qua> maps) => Maps = maps;

        /// <summary>
        ///     Runs AutoMod on each file in the set.
        /// </summary>
        public void Run()
        {
            Mods = new Dictionary<Qua, AutoMod>();
            Maps.ForEach(x => Mods.Add(x, new AutoMod(x)));

            DetectSpreadRequirementIssues();
        }

        /// <summary>
        ///     Detects rankability issues with the mapset spread.
        ///     If only map is contained inside of the mapset, then it must be >= 2:30 in length
        /// </summary>
        private void DetectSpreadRequirementIssues()
        {
            if (Maps.Count > 1)
                return;

            if (Maps.First().Length < 150000)
                Issues.Add(new AutoModIssueMapsetSpreadLength());
        }
    }
}