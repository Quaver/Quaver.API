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
            var converter = JsonConvert.DeserializeObject<MalodyFile>(File.ReadAllText("./Malody/Resources/1574436578.mc"));
            var qua = converter.ToQua();

            Assert.NotNull(qua);
        }

        [Fact]
        public void ConvertToQuaFile()
        {
            var dir = "./tests/malody";
            Directory.CreateDirectory(dir);

            var converter = JsonConvert.DeserializeObject<MalodyFile>(File.ReadAllText("./Malody/Resources/1574436578.mc"));
            var qua = converter.ToQua();

            qua.Save($"{dir}/map.qua");
        }

        [Fact]
        public void CheckObjectCount()
        {
            var converter = JsonConvert.DeserializeObject<MalodyFile>(File.ReadAllText("./Malody/Resources/1574436578.mc"));
            var qua = converter.ToQua();

            Assert.True(qua.HitObjects.Count >= 1);
        }

        [Fact]
        public void FailUponBadPath()
        {
            try
            {
                var converter = JsonConvert.DeserializeObject<MalodyFile>(File.ReadAllText("bad-path-no-file"));
                Assert.NotNull(converter);
            }
            catch (Exception e)
            {
                // passed.
            }
        }
    }
}
