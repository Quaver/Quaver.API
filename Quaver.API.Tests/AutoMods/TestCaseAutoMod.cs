using System.Linq;
using Quaver.API.Maps;
using Quaver.API.Maps.AutoMod;
using Quaver.API.Maps.AutoMod.Issues.Autoplay;
using Quaver.API.Maps.AutoMod.Issues.HitObjects;
using Quaver.API.Maps.AutoMod.Issues.Map;
using Quaver.API.Maps.AutoMod.Issues.Metadata;
using Quaver.API.Maps.AutoMod.Issues.ScrollVelocities;
using Quaver.API.Maps.AutoMod.Issues.TimingPoints;
using Xunit;
using Xunit.Abstractions;

namespace Quaver.API.Tests.AutoMods
{
    public class TestCaseAutoMod
    {
        private readonly ITestOutputHelper testOutputHelper;

        public TestCaseAutoMod(ITestOutputHelper testOutputHelper) => this.testOutputHelper = testOutputHelper;

        [Fact]
        public void DetectShortLongNotes()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/short-ln.qua"));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueShortLongNote ln && ln.HitObject.Lane == 1);
        }

        [Fact]
        public void DetectObjectBeforeStart()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/object-before-start.qua"));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueObjectBeforeStart);
        }

        [Fact]
        public void DetectOverlappingStartTimes()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/overlap-start-times.qua"));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueOverlappingObjects issue
                                                 && issue.HitObjects.First().Lane == 1);
        }

        [Fact]
        public void DetectOverlappingLongNoteEnd()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/overlap-ln-end.qua"));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueOverlappingObjects issue
                                                 && issue.HitObjects.First().StartTime == 1090);
        }

        [Fact]
        public void DetectOverlapInsideLongNote()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/overlap-inside-ln.qua"));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueOverlappingObjects issue
                                                 && issue.HitObjects.First().StartTime == 545);
        }

        [Fact]
        public void DetectColumnMissingInAllColumns()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/object-missing-column.qua"));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueObjectInAllColumns issue
                                                 && issue.MissingColumns.Count == 1 && issue.MissingColumns.First() == 3);
        }

        [Fact]
        public void DetectOverlappingTimingPoints()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/overlap-timing-points.qua"));
            autoMod.Run();

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            Assert.Contains(autoMod.Issues, x => x is AutoModeIssueTimingPointOverlap issue &&
                                                 issue.TimingPoints.First().StartTime == 545);
        }

        [Fact]
        public void DetectOverlappingScrollVelocities()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/overlap-sv.qua"));
            autoMod.Run();

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            Assert.Contains(autoMod.Issues, x => x is AutoModIssueScrollVelocityOverlap issue &&
                                                 issue.ScrollVelocities.First().StartTime == 545);
        }

        [Fact]
        public void DetectAutoPlayFailure()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/autoplay-failure.qua"));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueAutoplayFailure);
        }

        [Fact]
        public void DetectExcessiveBreak()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/30-sec-break.qua"));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueExcessiveBreakTime);
        }

        [Fact]
        public void DetectShortMapLength()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/no-45-sec-map.qua"));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueMapLength);
        }

        [Fact]
        public void DetectNonRomanizedCharacters()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/non-romanized.qua", false));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueNonRomanized issue &&
                                                 issue.Text.Contains("Artist"));
        }
    }
}