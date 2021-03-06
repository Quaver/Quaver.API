/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 * Copyright (c) 2017-2018 Swan & The Quaver Team <support@quavergame.com>.
*/

using System;
using System.IO;
using System.Linq;
using Quaver.API.Maps.Parsers.Stepmania;
using Xunit;

namespace Quaver.API.Tests.Stepmania
{
    public class TestCaseStepManiaConverter
    {
        [Fact]
        public void ConvertToQua()
        {
            var converter = new StepFile("./Stepmania/Resources/chaoz-airflow.sm");
            var quas = converter.ToQuas();

            Assert.Single(quas);
        }

        [Fact]
        public void ConvertToQuaFile()
        {
            var dir = "./tests/sm";
            Directory.CreateDirectory(dir);

            var converter = new StepFile("./Stepmania/Resources/chaoz-airflow.sm");
            var quas = converter.ToQuas();


            for (var i = 0; i < quas.Count; i++)
                quas[i].Save($"{dir}/{i}.qua");
        }

        [Fact]
        public void CheckObjectCount()
        {
            var converter = new StepFile("./Stepmania/Resources/chaoz-airflow.sm");
            var qua = converter.ToQuas().First();

            Assert.True(qua.HitObjects.Count >= 1);
        }

        [Fact]
        public void FailUponBadPath()
        {
            try
            {
                var converter = new StepFile("bad-path-no-file");
                Assert.NotNull(converter);
            }
            catch (Exception e)
            {
                // passed.
            }
        }
    }
}
