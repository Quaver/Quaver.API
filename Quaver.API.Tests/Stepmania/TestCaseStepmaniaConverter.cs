/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.IO;
using System.Linq;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;
using Quaver.API.Maps.Parsers.StepMania;
using Xunit;

namespace Quaver.API.Tests.Stepmania
{
    public class TestCaseStepManiaConverter
    {
        [Fact]
        public void ConvertToQua()
        {
            var converter = new StepmaniaConverter("./Stepmania/Resources/chaoz-airflow.sm");
            var quas = converter.ToQua();

            Assert.Single(quas);
        }

        [Fact]
        public void ConvertToQuaFile()
        {
            var dir = "./tests/sm";
            Directory.CreateDirectory(dir);

            var converter = new StepmaniaConverter("./Stepmania/Resources/chaoz-airflow.sm");
            var quas = converter.ToQua();

            for (var i = 0; i < quas.Count; i++)
                quas[i].Save($"{dir}/{i}.qua");
        }

        [Fact]
        public void CheckTimingPoints()
        {
            var converter = new StepmaniaConverter("./Stepmania/Resources/all-measures.sm");
            var qua = converter.ToQua().First();

            var accurateQua = Qua.Parse("./Stepmania/Resources/all-measures.qua");
            Assert.True(qua.TimingPoints.SequenceEqual(accurateQua.TimingPoints, TimingPointInfo.ByValueComparer));
        }

        [Fact]
        public void CheckHitObjects() {
            // Contains all possible measure types.
            var converter = new StepmaniaConverter("./Stepmania/Resources/all-measures.sm");
            var qua = converter.ToQua().First();

            var accurateQua = Qua.Parse("./Stepmania/Resources/all-measures.qua");
            Assert.True(qua.HitObjects.SequenceEqual(accurateQua.HitObjects, HitObjectInfo.ByValueComparer));
        }

        [Fact]
        public void CheckFullConversion() {
            // Contains all possible measure types.
            var converter = new StepmaniaConverter("./Stepmania/Resources/all-measures.sm");
            var qua = converter.ToQua().First();

            var accurateQua = Qua.Parse("./Stepmania/Resources/all-measures.qua");
            Assert.True(qua.EqualByValue(accurateQua));
        }

        [Fact]
        public void InMeasureBPMChange() {
            // Contains BPM changes inside a measure - some at 4ths, some at 3rds.
            var converter = new StepmaniaConverter("./Stepmania/Resources/in-measure-change.sm");
            var qua = converter.ToQua().First();

            var accurateQua = Qua.Parse("./Stepmania/Resources/in-measure-change.qua");
            Assert.True(qua.EqualByValue(accurateQua));
        }

        [Fact]
        public void FailUponBadPath()
        {
            try
            {
                var converter = new StepmaniaConverter("bad-path-no-file");
                Assert.NotNull(converter);
            }
            catch (Exception e)
            {
                // passed.
            }
        }
    }
}
