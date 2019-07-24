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
        public void CheckTimingPointCount()
        {
            // Contains all possible measure types, each measure having exactly one note;
            //   as well as timing points to test each transition.
            // Timing points are at: 0s, 2s,     4s, 6s,     8s, ...
            // Hit objects are at:   0s, 2s, 3s; 4s, 6s, 7s; 8s, ...
            var converter = new StepmaniaConverter("./Stepmania/Resources/all-measures.sm");
            var qua = converter.ToQua().First();

            Assert.Equal(21, qua.TimingPoints.Count());
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
        public void CheckHitObjectCount()
        {
            var converter = new StepmaniaConverter("./Stepmania/Resources/all-measures.sm");
            var qua = converter.ToQua().First();

            Assert.Equal(31, qua.HitObjects.Count());
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
