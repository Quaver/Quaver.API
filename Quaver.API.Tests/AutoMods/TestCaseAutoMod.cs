using System;
using System.Linq;
using Quaver.API.Maps;
using Quaver.API.Maps.AutoMod;
using Quaver.API.Maps.AutoMod.Issues;
using Quaver.API.Maps.AutoMod.Issues.HitObjects;
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
    }
}