using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
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

        [Fact]
        public void IssueQuaver592()
        {
            var qua = Qua.Parse("./Quaver/Resources/issue-quaver-592.qua");
            qua.ApplyMods(ModIdentifier.FullLN); // Should not throw.
        }

        [Fact]
        public void IssueQuaver721()
        {
            var qua = Qua.Parse("./Quaver/Resources/issue-quaver-721.qua");
            qua.ApplyMods(ModIdentifier.FullLN);

            var originalQua = Qua.Parse("./Quaver/Resources/issue-quaver-721.qua");

            // Full LN should preserve the object if it's the only object in a lane.
            Assert.True(qua.EqualByValue(originalQua));
            Assert.True(qua.IsValid());
        }

        [Fact]
        public void LoadFromStream()
        {
            var map = "./Quaver/Resources/fullln-output.qua";

            var buffer = File.ReadAllBytes(map);
            var byteArrayQua = Qua.Parse(buffer);
            var normalQua = Qua.Parse(map);

            var expectedObjects = normalQua.HitObjects.ToImmutableHashSet(HitObjectInfo.ByValueComparer);
            Assert.True(byteArrayQua.HitObjects.ToImmutableHashSet(HitObjectInfo.ByValueComparer).SetEquals(expectedObjects));
        }

        [Fact]
        public void SoundEffects()
        {
            var qua = Qua.Parse("./Quaver/Resources/sound-effects.qua");
            Assert.True(qua.IsValid());
            Assert.Equal(new []
            {
                new CustomAudioSampleInfo()
                {
                    Path = "hello.wav",
                    UnaffectedByRate = false
                },
                new CustomAudioSampleInfo()
                {
                    Path = "world.mp3",
                    UnaffectedByRate = true
                }
            }, qua.CustomAudioSamples, CustomAudioSampleInfo.ByValueComparer);
            Assert.Equal(new []
            {
                new SoundEffectInfo()
                {
                    StartTime = 123,
                    Sample = 2,
                    Volume = 100
                },
                new SoundEffectInfo()
                {
                    StartTime = 200,
                    Sample = 1,
                    Volume = 53
                }
            }, qua.SoundEffects, SoundEffectInfo.ByValueComparer);
        }

        [Fact]
        public void InvalidSampleIndex()
        {
            var qua = Qua.Parse("./Quaver/Resources/sound-effects-invalid-sample-index.qua", false);
            Assert.False(qua.IsValid());
        }

        [Fact]
        public void KeySounds()
        {
            var qua = Qua.Parse("./Quaver/Resources/keysounds.qua");
            Assert.True(qua.IsValid());
            Assert.Equal(new []
            {
                new CustomAudioSampleInfo()
                {
                    Path = "hello.wav",
                    UnaffectedByRate = false
                },
                new CustomAudioSampleInfo()
                {
                    Path = "world.mp3",
                    UnaffectedByRate = true
                }
            }, qua.CustomAudioSamples, CustomAudioSampleInfo.ByValueComparer);
            Assert.Equal(new List<KeySoundInfo>
            {
                new KeySoundInfo
                {
                    Sample = 1,
                    Volume = 100
                },
                new KeySoundInfo
                {
                    Sample = 2,
                    Volume = 50
                }
            }, qua.HitObjects[0].KeySounds, KeySoundInfo.ByValueComparer);
            Assert.Equal(new List<KeySoundInfo>
            {
                new KeySoundInfo
                {
                    Sample = 2,
                    Volume = 100
                }
            }, qua.HitObjects[1].KeySounds, KeySoundInfo.ByValueComparer);
            Assert.Equal(new List<KeySoundInfo>(), qua.HitObjects[2].KeySounds, KeySoundInfo.ByValueComparer);
        }

        [Fact]
        public void InvalidKeySoundIndex()
        {
            var qua = Qua.Parse("./Quaver/Resources/keysounds-invalid-sample-index.qua", false);
            Assert.False(qua.IsValid());
        }

        [Fact]
        public void SVNormalization()
        {
            var tests = new[]
            {
                "regression-1",
                "regression-2",
                "regression-3",
                "regression-4",
                "regression-5",
                "regression-6",
                "regression-7",
                "regression-8",
                "sample",
                "sv-at-first-timing-point",
                "sv-before-first-timing-point",
                "symphony",
                "timing-points-override-svs",
                "cheat",
            };

            foreach (var test in tests)
            {
                // These files were generated with plitki-map-qua (taken as ground truth) from its test suite.
                var pathNormalized = $"./Quaver/Resources/{test}.qua";
                var pathDenormalized = $"./Quaver/Resources/{test}-normalized.qua";

                var quaDenormalized = Qua.Parse(pathNormalized, false);
                var quaNormalized = Qua.Parse(pathDenormalized, false);

                // Check that the normalization gives the correct result.
                var quaDenormalizedNormalized = quaDenormalized.WithNormalizedSVs();
                Assert.True(quaDenormalizedNormalized.EqualByValue(quaNormalized));

                // Denormalization can move the first SV (it doesn't matter where to put the InitialScrollVelocity SV).
                // So check back-and-forth instead of just denormalization.
                var quaNormalizedDenormalizedNormalized = quaNormalized.WithDenormalizedSVs().WithNormalizedSVs();
                Assert.True(quaNormalizedDenormalizedNormalized.EqualByValue(quaNormalized));

                // Check that serializing and parsing the result does not change it.
                var bufferDenormalized = Encoding.UTF8.GetBytes(quaDenormalized.Serialize());
                var quaDenormalized2 = Qua.Parse(bufferDenormalized, false);
                Assert.True(quaDenormalized.EqualByValue(quaDenormalized2));

                var bufferNormalized = Encoding.UTF8.GetBytes(quaNormalized.Serialize());
                var quaNormalized2 = Qua.Parse(bufferNormalized, false);
                Assert.True(quaNormalized.EqualByValue(quaNormalized2));
            }
        }
    }
}