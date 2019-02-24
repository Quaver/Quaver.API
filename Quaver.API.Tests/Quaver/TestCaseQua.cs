using System.Collections.Immutable;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;
using Xunit;

namespace Quaver.API.Tests.Quaver
{
    public class TestCaseQua
    {
        private const string LN_CONVERSION_INPUT = "./Quaver/Resources/ln-conversion-input.qua";

        [Fact]
        public void MirrorHitObjects4K()
        {
            var qua = new Qua { Mode = GameMode.Keys4 };

            for (var i = 0; i < qua.GetKeyCount(); i++)
                qua.HitObjects.Add(new HitObjectInfo { Lane = i + 1});

            qua.MirrorHitObjects();

            Assert.True(qua.HitObjects[0].Lane == 4 &&
                        qua.HitObjects[1].Lane == 3 &&
                        qua.HitObjects[2].Lane == 2 &&
                        qua.HitObjects[3].Lane == 1);
        }

        [Fact]
        public void MirrorHitObjects7K()
        {
            var qua = new Qua { Mode = GameMode.Keys7 };

            for (var i = 0; i < qua.GetKeyCount(); i++)
                qua.HitObjects.Add(new HitObjectInfo { Lane = i + 1});

            qua.MirrorHitObjects();

            Assert.True(qua.HitObjects[0].Lane == 7 &&
                        qua.HitObjects[1].Lane == 6 &&
                        qua.HitObjects[2].Lane == 5 &&
                        qua.HitObjects[3].Lane == 4 &&
                        qua.HitObjects[4].Lane == 3 &&
                        qua.HitObjects[5].Lane == 2 &&
                        qua.HitObjects[6].Lane == 1);
        }

        [Fact]
        public void NLN()
        {
            var qua = Qua.Parse(LN_CONVERSION_INPUT);
            qua.ApplyMods(ModIdentifier.NoLongNotes);
            var objects = qua.HitObjects.ToImmutableHashSet(HitObjectInfo.ByValueComparer);

            var expected = Qua.Parse("./Quaver/Resources/nln-output.qua");
            var expectedObjects = expected.HitObjects.ToImmutableHashSet(HitObjectInfo.ByValueComparer);

            Assert.True(objects.SetEquals(expectedObjects));
        }

        [Fact]
        public void Inverse()
        {
            var qua = Qua.Parse(LN_CONVERSION_INPUT);
            qua.ApplyMods(ModIdentifier.Inverse);
            var objects = qua.HitObjects.ToImmutableHashSet(HitObjectInfo.ByValueComparer);

            var expected = Qua.Parse("./Quaver/Resources/inverse-output.qua");
            var expectedObjects = expected.HitObjects.ToImmutableHashSet(HitObjectInfo.ByValueComparer);

            Assert.True(objects.SetEquals(expectedObjects));
        }

        [Fact]
        public void FullLN()
        {
            var qua = Qua.Parse(LN_CONVERSION_INPUT);
            qua.ApplyMods(ModIdentifier.FullLN);
            var objects = qua.HitObjects.ToImmutableHashSet(HitObjectInfo.ByValueComparer);

            var expected = Qua.Parse("./Quaver/Resources/fullln-output.qua");
            var expectedObjects = expected.HitObjects.ToImmutableHashSet(HitObjectInfo.ByValueComparer);

            Assert.True(objects.SetEquals(expectedObjects));
        }
    }
}