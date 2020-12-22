using System.IO;
using Quaver.API.Maps;
using Quaver.API.Maps.Parsers.Bms;
using Xunit;

namespace Quaver.API.Tests.Bms
{
    public class TestCaseBmsConverter
    {
        [Fact]
        public void ParseBasicBms()
        {
            // A regular BMS file with no gimmicks
            var conv = new BmsFile("./Bms/Resources/LeaF - Calamity Fortune - _S.bms");
            Assert.True(conv.IsValid);
        }

        [Fact]
        public void ParseBmsWithLongNotes()
        {
            // A BMS file that contains long notes (LNs)
            var conv = new BmsFile("./Bms/Resources/Hammer Switch - Lots of Spices - 03_LOS_Another.bms");
            Assert.True(conv.IsValid);
        }

        [Fact]
        public void ParseInvalidBmsWithPerColumnSv()
        {
            var conv = new BmsFile("./Bms/Resources/Hammer Switch - Lots of Spices - 77_LOS_Abusive.bml");
            Assert.False(conv.IsValid);
        }

        [Fact]
        public void ParseInvalidBmsWithFakes()
        {
            // "Fakes" are notes that should not be pressed (also known as "landmines")
            // Typically, these BMS maps will also have per-column SV. Quaver cannot parse
            // them, so this should not be a valid .qua file.
            var conv = new BmsFile("./Bms/Resources/Hammer Switch - Lots of Spices - 778_LOSmineds.bml");
            Assert.False(conv.IsValid);
        }

        [Fact]
        public void CheckSoundEffectCount()
        {
            var conv = new BmsFile("./Bms/Resources/Kuon - Genealogy of Lives - _gol_7h.bme");
            var groundTruthQua = Qua.Parse("./Bms/Resources/Kuon - Genealogy of Lives - _gol_7h.qua");
            Assert.Equal(groundTruthQua.SoundEffects.Count, conv.SoundEffects.Count);
        }

        [Fact]
        public void CheckHitObjectCount()
        {
            var conv = new BmsFile("./Bms/Resources/Kuon - Genealogy of Lives - _gol_7h.bme");
            var groundTruthQua = Qua.Parse("./Bms/Resources/Kuon - Genealogy of Lives - _gol_7h.qua");
            Assert.Equal(groundTruthQua.HitObjects.Count, conv.HitObjects.Count);
        }

        [Fact]
        public void CheckTimingPointCount()
        {
            var conv = new BmsFile("./Bms/Resources/Kuon - Genealogy of Lives - _gol_7h.bme");
            var groundTruthQua = Qua.Parse("./Bms/Resources/Kuon - Genealogy of Lives - _gol_7h.qua");
            Assert.Equal(groundTruthQua.TimingPoints.Count, conv.TimingPoints.Count);
        }

        [Fact]
        public void ConvertBmsToQuaFile()
        {
            // Tests a BMS map with no tempo changes or STOP commands
            const string dir = "./tests/bms/normal";
            Directory.CreateDirectory(dir);

            var converter = new BmsFile("./Bms/Resources/LeaF - Calamity Fortune - _S.bms");
            Assert.True(converter.IsValid);

            var qua = converter.ToQua();

            qua.Save($"{dir}/map.qua");
        }

        [Fact]
        public void ConvertLongNoteBmsToQuaFile()
        {
            // Tests a BMS map with LNs
            const string dir = "./tests/bms/ln";
            Directory.CreateDirectory(dir);

            var converter = new BmsFile("./Bms/Resources/Hammer Switch - Lots of Spices - 03_LOS_Another.bms");
            Assert.True(converter.IsValid);

            var qua = converter.ToQua();

            qua.Save($"{dir}/map.qua");
        }

        [Fact]
        public void ConvertDenseBmsToQuaFile()
        {
            // Tests a BMS map with many tempo changes and STOP commands
            const string dir = "./tests/bms/dense";
            Directory.CreateDirectory(dir);

            var converter = new BmsFile("./Bms/Resources/LeaF - Aleph0 - _7INSANE.bms");
            Assert.True(converter.IsValid);

            var qua = converter.ToQua();

            qua.Save($"{dir}/map.qua");
        }

        [Fact]
        public void ConvertStopBmsToQuaFile()
        {
            // Tests a BMS map with simple STOP commands
            const string dir = "./tests/bms/stop";
            Directory.CreateDirectory(dir);

            var converter = new BmsFile("./Bms/Resources/Is-m - reflective moment - _44_11.bme");
            Assert.True(converter.IsValid);

            var qua = converter.ToQua();

            qua.Save($"{dir}/map.qua");
        }

        [Fact]
        public void ConvertTempoBmsToQuaFile()
        {
            // Tests a BMS map with simple tempo changes
            const string dir = "./tests/bms/tempo";
            Directory.CreateDirectory(dir);

            var converter = new BmsFile("./Bms/Resources/Kuon - Genealogy of Lives - _gol_7h.bme");
            var qua = converter.ToQua();

            qua.Save($"{dir}/map.qua");
        }

        [Fact]
        public void FailOnBadPath()
        {
            var conv = new BmsFile("we-do-a-little-trolling");
            Assert.False(conv.IsValid);
        }

    }
}