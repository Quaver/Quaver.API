using System.Collections.Generic;
using System.Linq;
using Quaver.API.Helpers;
using Quaver.API.Maps.AutoMod.Issues;
using Quaver.API.Maps.AutoMod.Issues.Mapset;
using Quaver.API.Maps.AutoMod.Issues.Metadata;

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
            Issues.Clear();

            Mods = new Dictionary<Qua, AutoMod>();

            Maps.ForEach(x =>
            {
                var autoMod = new AutoMod(x);
                autoMod.Run();
                Mods.Add(x, autoMod);
            });

            DetectSpreadRequirementIssues();
            DetectMismatchingMetadata();
            DetectMultiModeDifficultyNameIssues();
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

        /// <summary>
        ///     Detects if the metadata in all files are the same.
        /// </summary>
        private void DetectMismatchingMetadata()
        {
            if (Maps.Count == 1)
                return;

            if (Maps.Any(x => x.Artist != Maps.First().Artist))
                Issues.Add(new AutoModIssueMismatchingMetdata(MetadataField.Artist));

            if (Maps.Any(x => x.Title != Maps.First().Title))
                Issues.Add(new AutoModIssueMismatchingMetdata(MetadataField.Title));

            if (Maps.Any(x => x.Source != Maps.First().Source))
                Issues.Add(new AutoModIssueMismatchingMetdata(MetadataField.Source));

            if (Maps.Any(x => x.Tags != Maps.First().Tags))
                Issues.Add(new AutoModIssueMismatchingMetdata(MetadataField.Tags));
        }

        /// <summary>
        ///     Detects if a multi-mode mapset's difficulty names aren't prceeeded by
        ///     either "4K" or "7K"
        /// </summary>
        private void DetectMultiModeDifficultyNameIssues()
        {
            if (Maps.Count == 1)
                return;

            // Set only contains 1 unique game mode.
            if (Maps.Select(x => x.Mode).Distinct().Count() == 1)
                return;

            foreach (var map in Maps)
            {
                var modeString = ModeHelper.ToShortHand(map.Mode).ToLower();

                if (map.DifficultyName.ToLower().Contains(modeString))
                    continue;

                Issues.Add(new AutoModIssueMultiModeDiffName(map));
            }
        }
    }
}