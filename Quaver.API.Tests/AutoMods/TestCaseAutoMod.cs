using System.Collections.Generic;
using System.Linq;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.API.Maps.AutoMod;
using Quaver.API.Maps.AutoMod.Issues.Audio;
using Quaver.API.Maps.AutoMod.Issues.Autoplay;
using Quaver.API.Maps.AutoMod.Issues.Background;
using Quaver.API.Maps.AutoMod.Issues.HitObjects;
using Quaver.API.Maps.AutoMod.Issues.Map;
using Quaver.API.Maps.AutoMod.Issues.Mapset;
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
        public void DetectIncorrectPreviewPoint()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/preview-point-1.qua", false));
            var autoMod2 = new AutoMod(Qua.Parse("./AutoMods/Resources/preview-point-2.qua", false));
            autoMod.Run();
            autoMod2.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssuePreviewPoint);
            Assert.Contains(autoMod2.Issues, x => x is AutoModIssuePreviewPoint);
        }

        [Fact]
        public void DetectNonRomanizedCharacters()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/non-romanized.qua", false));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueNonRomanized issue &&
                                                 issue.Text.Contains("Artist"));
        }

        [Fact]
        public void DetectNoBackgroundFile()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/no-bg-file.qua", false));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueNoBackground);
        }

        [Fact]
        public void DetectLargeBackgroundFile()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/large-bg-file.qua", false));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueBackgroundTooLarge);
        }

        [Fact]
        public void DetectSmallBackgroundResolution()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/small-bg-resolution.qua", false));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueBackgroundResolution);
        }

        [Fact]
        public void DetectNotesAfterAudio()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/notes-after-audio.qua", false));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueObjectAfterAudioEnd);
        }

        [Fact]
        public void DetectTimingPointAfterAudio()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/timing-point-after-end.qua", false));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueTimingPointAfterAudioEnd);
        }

        [Fact]
        public void DetectScrollVelocityAfterAudio()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/sv-after-end.qua", false));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueScrollVelocityAfterEnd);
        }

        [Fact]
        public void DetectAudioBitrateTooHigh()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/high-bitrate.qua", false));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueAudioBitrate);
        }

        [Fact]
        public void DetectWrongAudioFormat()
        {
            var autoMod = new AutoMod(Qua.Parse("./AutoMods/Resources/wrong-audio-format.qua", false));
            autoMod.Run();

            Assert.Contains(autoMod.Issues, x => x is AutoModIssueAudioFormat);
        }

        [Fact]
        public void DetectMapsetSpreadIssues()
        {
            var autoModMapset = new AutoModMapset(new List<Qua>()
            {
                Qua.Parse("./AutoMods/Resources/short-ln.qua", false)
            });

            autoModMapset.Run();

            Assert.Contains(autoModMapset.Issues, x => x is AutoModIssueMapsetSpreadLength);
        }

        [Fact]
        public void DetectMapsetMismatchedMetadata()
        {
            var autoModMapset = new AutoModMapset(new List<Qua>()
            {
                Qua.Parse("./AutoMods/Resources/mismatch-metadata-1.qua", false),
                Qua.Parse("./AutoMods/Resources/mismatch-metadata-2.qua", false)
            });

            autoModMapset.Run();

            Assert.True(autoModMapset.Issues.FindAll(x => x is AutoModIssueMismatchingMetdata).Count == 4);
        }

        [Fact]
        public void DetectMultiModeDiffNameIssues()
        {
            var autoModMapset = new AutoModMapset(new List<Qua>()
            {
                Qua.Parse("./AutoMods/Resources/multi-mode-diff-name-1.qua", false),
                Qua.Parse("./AutoMods/Resources/multi-mode-diff-name-2.qua", false)
            });

            autoModMapset.Run();

            Assert.Contains(autoModMapset.Issues, x => x is AutoModIssueMultiModeDiffName issue &&
                                                       issue.Map.Mode == GameMode.Keys7);
        }
    }
}