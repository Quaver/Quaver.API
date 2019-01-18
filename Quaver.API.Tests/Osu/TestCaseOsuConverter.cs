/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2019 Swan & The Quaver Team <support@quavergame.com>.
*/

using System.IO;
using Quaver.API.Maps;
using Quaver.API.Maps.Parsers;
using Xunit;

namespace Quaver.API.Tests.Osu
{
    public class TestCaseOsuConverter
    {
        private const string BeatmapFilename = "./Osu/Resources/Camellia - Backbeat Maniac (Evening) [Rewind VIP].osu";

        [Fact]
        public void SuccessfulParse()
        {
            var converter = new OsuBeatmap(BeatmapFilename);
            Assert.True(converter.IsValid);
        }

        [Fact]
        public void ConvertToQuaFile()
        {
            var dir = "./tests/osu";
            Directory.CreateDirectory(dir);

            var converter = new OsuBeatmap(BeatmapFilename);
            var qua = converter.ToQua();
            qua.Save($"{dir}/output.qua");
        }

        [Fact]
        public void CheckObjectCount()
        {
            var converter = new OsuBeatmap(BeatmapFilename);
            var qua = converter.ToQua();
            Assert.Equal(2041 + 270, qua.HitObjects.Count);
        }

        [Fact]
        public void CheckObjectCountWithHitsounds()
        {
            // This map had missing objects due to an error in the parsing logic.
            var converter = new OsuBeatmap("./Osu/Resources/xi - Blue Zenith (Jepetski) [Zen's Black Another].osu");
            var qua = converter.ToQua();
            Assert.Equal(4084 + 49, qua.HitObjects.Count);
        }

        [Fact]
        public void FullConversionCheck()
        {
            var converter = new OsuBeatmap(BeatmapFilename);
            var qua = converter.ToQua();

            var groundTruthQua = Qua.Parse(Path.ChangeExtension(BeatmapFilename, "qua"));
            Assert.True(qua.EqualByValue(groundTruthQua));
        }

        [Fact]
        public void CheckCommonBPM()
        {
            // This map had incorrect common BPM.
            var converter = new OsuBeatmap("./Osu/Resources/Camellia feat. Nanahira - ChoChoKouSokuDeMaeSaiSoku!!! SpeedStarKanade (jakads) [Tsukahara's MAXIMUM].osu");
            var qua = converter.ToQua();

            Assert.Equal(500, qua.GetCommonBpm());
        }

        [Fact]
        public void FailUponBadPath()
        {
            var converter = new OsuBeatmap("bad-path-no-file");
            Assert.False(converter.IsValid);
        }
    }
}
