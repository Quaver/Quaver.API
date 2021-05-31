// this is for yahweh my best friend
// and also fodi because he is my biggest supporter for this (i think)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Quaver.API.Enums;
using Quaver.API.Maps.Parsers.BeMusicSource.Classes;
using Quaver.API.Maps.Parsers.BeMusicSource.Readers;
using Quaver.API.Maps.Parsers.BeMusicSource.Timing;
using Quaver.API.Maps.Parsers.BeMusicSource.Utils;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.Parsers.BeMusicSource
{
    public class BMSFile
    {
        // MAN
        private readonly Regex normalNoteRegex = new Regex("[1][1-z]");

        private readonly Regex p2Regex = new Regex("[2][1-z]");

        private readonly Regex p2LnRegex = new Regex("[6][1-z]");

        private readonly Regex longNoteRegex = new Regex("[5][1-z]");

        private readonly List<BMSSoundEffect> soundEffects = new List<BMSSoundEffect>();

        private readonly Dictionary<int, List<BMSHitObject>> hitObjects = new Dictionary<int, List<BMSHitObject>>();

        private readonly SortedDictionary<double, double> timingPoints = new SortedDictionary<double, double>();

        public readonly string InvalidReason = "";

        private readonly BMSFileData fileData;

        public BMSFile(string inputPath)
        {
            try
            {
                var startTrackAt = 0.0;

                var longNoteTracker = new Dictionary<int, double>();
                var longNoteSoundEffectTracker = new Dictionary<int, int>();

                var reader = new BMSFileDataReader(inputPath);

                if (reader.InvalidReason != null)
                {
                    InvalidReason = reader.InvalidReason;
                    return;
                }

                if (reader.TrackLines.Count == 0)
                {
                    InvalidReason = "no tracks to process??? lol";
                    return;
                }

                fileData = reader.FileData;
                var startTrackWithBpm = fileData.StartingBPM;
                timingPoints[0.0] = startTrackWithBpm;

                foreach (var track in reader.TrackLines)
                {
                    var trackReader = new BMSTrackDataReader(track.Value, fileData);

                    foreach (var line in from line in track.Value
                        where line.Message.Length % 2 == 0
                        where !p2LnRegex.IsMatch(line.Channel) && !p2Regex.IsMatch(line.Channel)
                        where normalNoteRegex.IsMatch(line.Channel) || longNoteRegex.IsMatch(line.Channel) ||
                              line.Channel == "01"
                        select line)
                    {
                        for (var i = 0; i < line.Message.Length / 2; i++)
                        {
                            var target = BMSStringUtil.GetHexAtIndex(i, line.Message);
                            if (target == "00") continue;

                            var time = BMSPositionTiming.GetStartTime(trackReader.TrackData, i, line.Message,
                                startTrackWithBpm);
                            var sfx = GetHitObjectKeySound(fileData.SoundHexPairs, target);
                            if (normalNoteRegex.IsMatch(line.Channel) || longNoteRegex.IsMatch(line.Channel))
                            {
                                var laneInt = BMSStringUtil.GetLaneNumber(line.Channel.Substring(1));
                                if (laneInt == 6) laneInt = 8;
                                else if (laneInt >= 8) laneInt -= 2;
                                if (laneInt > 8) continue;

                                var hitObject = new BMSHitObject()
                                {
                                    StartTime = startTrackAt + time,
                                    IsLongNote = false,
                                    KeySoundIndex = -1
                                };

                                if (target == fileData.LNObject)
                                {
                                    if (hitObjects[laneInt].Count == 0) continue;
                                    var back = hitObjects[laneInt].Count - 1;

                                    if (hitObject.StartTime - hitObjects[laneInt][back].StartTime < 2.0) continue;

                                    hitObjects[laneInt][back].IsLongNote = true;
                                    hitObjects[laneInt][back].EndTime = hitObject.StartTime;
                                    continue;
                                }

                                if (sfx != -1)
                                {
                                    hitObject.KeySoundIndex = sfx;
                                }

                                if (longNoteRegex.IsMatch(line.Channel))
                                {
                                    if (!longNoteTracker.ContainsKey(laneInt)) longNoteTracker.Add(laneInt, 0.0);
                                    if (!longNoteSoundEffectTracker.ContainsKey(laneInt)) longNoteSoundEffectTracker.Add(laneInt, 0);
                                    if (longNoteTracker[laneInt] != 0.0)
                                    {
                                        hitObject.EndTime = hitObject.StartTime;
                                        hitObject.StartTime = longNoteTracker[laneInt];
                                        if (hitObject.EndTime <= hitObject.StartTime) continue;
                                        hitObject.IsLongNote = true;
                                        if (longNoteSoundEffectTracker[laneInt] != 0)
                                        {
                                            hitObject.KeySoundIndex = longNoteSoundEffectTracker[laneInt];
                                        }

                                        longNoteTracker[laneInt] = 0.0;
                                        longNoteSoundEffectTracker[laneInt] = 0;
                                    }
                                    else
                                    {
                                        longNoteTracker[laneInt] = hitObject.StartTime;
                                        longNoteSoundEffectTracker[laneInt] = hitObject.KeySoundIndex;
                                        continue;
                                    }
                                }

                                if (!hitObjects.ContainsKey(laneInt))
                                {
                                    hitObjects[laneInt] = new List<BMSHitObject>();
                                }
                                hitObjects[laneInt].Add(hitObject);
                                continue;
                            }

                            if (line.Channel == "01")
                            {
                                var soundEffect = new BMSSoundEffect()
                                {
                                    StartTime = time + startTrackAt
                                };
                                // fuck sake
                                if (sfx != -1)
                                {
                                    soundEffect.Sample = sfx;
                                }
                                else continue;

                                soundEffects.Add(soundEffect);
                            }
                        }
                    }

                    var fullTrackLength = BMSLengthTiming.GetTotalTrackLength(startTrackWithBpm, trackReader.TrackData);
                    var tp =
                        BMSPositionTiming.CalculateTimingPoints(startTrackAt, startTrackWithBpm, trackReader.TrackData, track.Key);

                    foreach (var p in tp)
                    {
                        timingPoints[p.Key] = p.Value;
                    }

                    if (trackReader.TrackData.BPMChanges.Count > 0)
                    {
                        startTrackWithBpm = trackReader.TrackData.BPMChanges[trackReader.TrackData.BPMChanges.Count - 1]
                            .BPM;
                    }

                    startTrackAt += fullTrackLength;
                }
            }
            catch (Exception e)
            {
                InvalidReason = e.Message;
            }
        }

        private static int GetHitObjectKeySound(IReadOnlyList<string> hexList, string target)
        {
            for (var i = 0; i < hexList.Count; i++)
            {
                if (target == hexList[i])
                {
                    return i + 1;
                }
            }

            return -1;
        }

        public Qua ToQua()
        {
            var quaFile = new Qua
            {
                BackgroundFile = fileData.Metadata.StageFile,
                AudioFile = "virtual",
                SongPreviewTime = -1,
                MapId = -1,
                MapSetId = -1,
                Mode = GameMode.Keys7,
                HasScratchKey = true,
                Title = fileData.Metadata.Title,
                Artist = fileData.Metadata.Artist,
                Source = "BMS",
                Tags = fileData.Metadata.Tags,
                Creator = BMSStringUtil.AppendSubArtistsToArtist( fileData.Metadata.Artist, fileData.Metadata.SubArtists ),
                DifficultyName = BMSStringUtil.GetDifficultyName( fileData.Metadata.Difficulty, fileData.Metadata.SubTitle ),
                Description = "This is a BMS map converted to Quaver",
            };

            foreach (var cas in fileData.SoundFiles)
            {
                quaFile.CustomAudioSamples.Add(new CustomAudioSampleInfo()
                {
                    Path = cas,
                    UnaffectedByRate = false
                });
            }

            foreach (var sfx in soundEffects)
            {
                quaFile.SoundEffects.Add(new SoundEffectInfo()
                {
                    Sample = sfx.Sample,
                    StartTime = (float) sfx.StartTime,
                    Volume = 100
                });
            }

            foreach (var tp in timingPoints)
            {
                quaFile.TimingPoints.Add(new TimingPointInfo()
                {
                    StartTime = (float) tp.Key,
                    Bpm = (float) tp.Value
                });
            }

            foreach (var lane in hitObjects)
            {
                foreach (var obj in lane.Value)
                {
                    quaFile.HitObjects.Add(new HitObjectInfo()
                    {
                        StartTime = (int) obj.StartTime,
                        Lane = lane.Key,
                        EndTime = obj.IsLongNote ? (int) obj.EndTime : 0,
                        KeySounds = obj.KeySoundIndex != -1
                            ? new List<KeySoundInfo>()
                            {
                                new KeySoundInfo()
                                {
                                    Sample = obj.KeySoundIndex,
                                    Volume = 100
                                }
                            }
                            : new List<KeySoundInfo>()
                    });
                }
            }

            return quaFile;
        }
    }
}