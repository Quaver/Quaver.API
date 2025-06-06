using Newtonsoft.Json;
using Quaver.API.Enums;
using Quaver.API.Helpers;
using Quaver.API.Maps.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Quaver.API.Maps.Parsers.Malody
{
    public class MalodyFile
    {
        [JsonProperty("meta")]
        public MalodyFileMeta Meta { get; set; }

        [JsonProperty("note")]
        public List<MalodyHitObject> Hitobjects { get; set; }

        [JsonProperty("time")]
        public List<MalodyTimingPoint> TimingPoints { get; set; }

        [JsonProperty("effect")]
        public List<MalodyEffectPoint> EffectPoints { get; set; }

        /// <summary>
        ///     Parses a malody file from a file path
        /// </summary>
        /// <returns></returns>
        public static MalodyFile Parse(string path) => JsonConvert.DeserializeObject<MalodyFile>(File.ReadAllText(path));

        /// <summary>
        /// Converts a Malody object to Qua
        /// </summary>
        /// <returns>Qua object</returns>
        public Qua ToQua()
        {
            var audioFile = Hitobjects.FirstOrDefault(x => x.Type == 1).Sound;
            var audioOffset = -Hitobjects.FirstOrDefault(x => x.Type == 1).Offset;

            var qua = new Qua()
            {
                AudioFile = audioFile,
                SongPreviewTime = Meta.PreviewTime,
                BackgroundFile = Meta.Background,
                MapId = -1,
                MapSetId = -1,
                Title = string.IsNullOrEmpty(Meta.Song.Title) ? Meta.Song.TitleOriginal : Meta.Song.Title,
                Artist = string.IsNullOrEmpty(Meta.Song.Artist) ? Meta.Song.ArtistOriginal : Meta.Song.Artist,
                Source = "Malody",
                Tags = "",
                Creator = Meta.Creator,
                DifficultyName = Meta.Version,
                Description = $"This is a Quaver converted version of {Meta.Creator}'s map."
            };

            if (Meta.Mode != 0)
                throw new ArgumentException("Only the 'Key' Malody game mode can be converted to Qua");

            qua.Mode = ModeHelper.FromKeyCount(Meta.Keymode.Keymode);

            foreach (var tp in TimingPoints)
            {
                qua.TimingPoints.Add(new TimingPointInfo()
                {
                    StartTime = GetMilliSeconds(GetBeat(tp.Beat), audioOffset),
                    Bpm = tp.Bpm,
                    Signature = TimeSignature.Quadruple
                });
            }

            if (EffectPoints != null)
            {
                foreach (var sv in EffectPoints)
                {
                    if (sv.Scroll == null) continue;
                    qua.SliderVelocities.Add(new SliderVelocityInfo
                    {
                        StartTime = GetMilliSeconds(GetBeat(sv.Beat), audioOffset),
                        Multiplier = sv.Scroll.Value
                    });
                }
            }

            foreach (var ho in Hitobjects)
            {
                KeySoundInfo keySound;

                if (ho.Type == 1) // The song itself, doesn't have a note representation
                    continue;

                if (string.IsNullOrEmpty(ho.Sound))
                    keySound = null;
                else
                {
                    var cas = new CustomAudioSampleInfo
                    {
                        Path = ho.Sound,
                        UnaffectedByRate = false
                    };

                    if (!qua.CustomAudioSamples.Contains(cas))
                        qua.CustomAudioSamples.Add(cas);

                    keySound = new KeySoundInfo
                    {
                        Sample = qua.CustomAudioSamples.IndexOf(cas),
                        Volume = ho.Volume
                    };
                }

                qua.HitObjects.Add(new HitObjectInfo
                {
                    StartTime = GetMilliSeconds(GetBeat(ho.Beat), audioOffset),
                    EndTime = ho.BeatEnd == null ? 0 : GetMilliSeconds(GetBeat(ho.BeatEnd), audioOffset),
                    Lane = ho.Column + 1,
                    KeySounds = keySound == null ? new List<KeySoundInfo>() : new List<KeySoundInfo> { keySound }
                });
            }

            qua.Sort();

            if (!qua.IsValid())
                throw new ArgumentException("The .qua file is invalid. It does not have HitObjects, TimingPoints, its Mode is invalid or some hit objects are invalid.");

            return qua;
        }

        /// <summary>
        /// Gets the time of a beat including an offset
        /// </summary>
        /// <param name="beat"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private int GetMilliSeconds(float beat, int offset)
        {
            var ms = offset;

            var i = 1;

            for (; i < TimingPoints.Count; i++)
            {
                if (GetBeat(TimingPoints[i].Beat) >= beat)
                    break;

                offset = GetOffset(TimingPoints[i - 1].Bpm, GetBeat(TimingPoints[i].Beat) - GetBeat(TimingPoints[i - 1].Beat), offset);
            }

            ms = GetOffset(TimingPoints[i - 1].Bpm, beat - GetBeat(TimingPoints[i - 1].Beat), offset);

            return ms;
        }

        /// <summary>
        /// Gets the duration of a given amount of beats based on a given bpm and adds it to an offset.
        /// </summary>
        /// <param name="bpm"></param>
        /// <param name="beats"></param>
        /// <param name="prevOffset"></param>
        /// <returns></returns>
        private int GetOffset(float bpm, float beats, int prevOffset) => (int)((1000 * (60 / bpm) * beats) + prevOffset);

        /// <summary>
        /// Converts a beat (Form: [measure, nth beat, divisor]) to a single float number
        /// </summary>
        /// <param name="beat"></param>
        /// <returns></returns>
        private float GetBeat(List<int> beat) => beat[0] + (beat[1] / (float)beat[2]);
    }
}
