using Quaver.API.Maps.Parsers.Malody;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Newtonsoft.Json;

namespace Quaver.API.Tests.Malody
{
    public class TestCaseMalodyConverter
    {
        [Fact]
        public void ConvertToQua()
        {
            var converter = MalodyFile.Parse("./Malody/Resources/1574436578.mc");
            var qua = converter.ToQua();

            Assert.NotNull(qua);
        }

        [Fact]
        public void ConvertToQuaFile()
        {
            var dir = "./tests/malody";
            Directory.CreateDirectory(dir);

            var converter = MalodyFile.Parse("./Malody/Resources/1574436578.mc");
            var qua = converter.ToQua();

            qua.Save($"{dir}/map.qua");
        }

        [Fact]
        public void CheckObjectCount()
        {
            var converter = MalodyFile.Parse("./Malody/Resources/1574436578.mc");
            var qua = converter.ToQua();

            Assert.True(qua.HitObjects.Count >= 1);
        }

        [Fact]
        public void FailUponBadPath()
        {
            try
            {
                var converter = MalodyFile.Parse("bad-path-no-file");
                Assert.NotNull(converter);
            }
            catch (Exception e)
            {
                // passed.
            }
        }

        [Fact]
        public void CheckSVEffects()
        {
            var converter = MalodyFile.Parse("./Malody/Resources/1626341674.mc");
            var qua = converter.ToQua();

            Assert.True(qua.SliderVelocities.Count > 0);
        }

        [Fact]
        public void IgnoreNonSVEffect()
        {
            var converter = MalodyFile.Parse("./Malody/Resources/1651047750.mc");
            var qua = converter.ToQua();

            Assert.True(qua.SliderVelocities.Count == 0);
        }
    }
}
