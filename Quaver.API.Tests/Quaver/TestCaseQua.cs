using System.Collections.Generic;
using Quaver.API.Enums;
using Quaver.API.Maps;
using Quaver.API.Maps.Structures;
using Xunit;

namespace Quaver.API.Tests.Quaver
{
    public class TestCaseQua
    {
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
    }
}