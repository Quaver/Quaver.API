using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Quaver.API.Enums;
using Quaver.API.Maps.Parsers.Bms.Utilities;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.Parsers.Bms
{
    public class BmsFile
    {
        private readonly Regex mineRegex = new Regex("[d-e][1-9]");
        private readonly Regex normalNoteRegex = new Regex("[1-2][1-z]");
        private readonly Regex longNoteRegex = new Regex("[5-6][1-z]");

        private readonly BmsFileMetadata metadata = new BmsFileMetadata()
        {
            Title = "No title",
            PlayLevel = "?",
            Artist = "Unknown Artist",
            Tags = "BMS"
        };

        public readonly List<BmsSoundEffect> SoundEffects = new List<BmsSoundEffect>();

        public readonly List<BmsHitObject> HitObjects = new List<BmsHitObject>();

        public readonly List<BmsTimingPoint> TimingPoints = new List<BmsTimingPoint>();

        private readonly bool hasScratchKey;

        private readonly List<string> keySoundStringArray = new List<string>();

        public readonly bool IsValid = true;

        public BmsFile(string filePath)
        {

            var trackData = new Dictionary<int, List<BmsLocalTrackData>>();

            // BM98 original specification defaults to 130.0 if not overwritten/#BPM header is omitted.
            var startThisTrackWithBpm = 130.0;

            // Track the current time as iterations occur, and use it as a base for the current track + whatever offset
            var startTrackWithTime = 0.0;

            // Track long notes. (Exclusive for BMS maps that don't use #LNOBJ)
            // The key represents the lane, and the value is the last known start time of the LN. This way, if we encounter
            // another object in the same lane on the same channel, that would be considered an LN tail and this would reset to null
            var longNoteTracker = new Dictionary<int, double>();

            // Track long note sound effects associated with the long notes in longNoteTracker. When an LN tail is detected in a
            // certain lane the corresponding lane's sound effect will be removed
            var longNoteSoundEffectTracker = new Dictionary<int, BmsSound>();

            // The hex values corresponding to this class's array of string values.
            var hitSoundHexList = new List<string>();

            // Maps a key hex value to a value representing desired BPM change.
            var bpmChanges = new Dictionary<string, double>();

            // Maps a key hex value to a STOP command. Note that STOP commands don't represent time in themselves, but rather 1/192
            // of a whole note in 4/4 meter. (Example: A STOP command with value 48 when the BPM is 60.0 stops for 1 second)
            var stopCommands = new Dictionary<string, double>();

            // The #LNOBJ specified. If this isn't set, we can assume that there is, well, no LN object.
            // The value in a message represents a LN termination, so the point before it becomes an LN.
            var lnObject = "";

            try
            {
                foreach (var line in File.ReadAllLines(filePath))
                {
                    if (!line.StartsWith("#"))
                        continue;
                    var lineLower = line.ToLower();

                    // If this block is true, then the line is not an instruction, but a header.
                    // Ex: #BPM01 190 or #TITLE Song Name
                    if (line.Length < 7 || line[6] != ':')
                    {
                        var lineWithoutHash = lineLower.Substring(1);
                        // Player mode (1 = 1P, 2 = 2P, 3 = DP)
                        if (lineWithoutHash.StartsWith("player"))
                        {
                            if (line.Length < 9)
                            {
                                IsValid = false;
                                return;
                            }

                            switch (line[8])
                            {
                                case '1':
                                case '2':
                                    // Meant for 1st or 2nd player.
                                    // It's okay if it's either one since the regex sees P1 and P2
                                    // channels as indifferent.
                                    break;
                                case '3':
                                    // Double Play (14K?) not supported.
                                    IsValid = false;
                                    return;
                                default:
                                    // A player header was defined, but isn't a valid value
                                    IsValid = false;
                                    return;
                            }
                        }
                        else if (lineWithoutHash.StartsWith("genre"))
                        {
                            if (line.Length < 8) continue;
                            metadata.Tags = line.Substring(7);
                        }
                        // Who made the chart if it wasn't the artist
                        else if (lineWithoutHash.StartsWith("maker"))
                        {
                            if (line.Length < 8) continue;
                            metadata.Creator = line.Substring(7);
                        }
                        else if (lineWithoutHash.StartsWith("title"))
                        {
                            if (line.Length < 8) continue;
                            metadata.Title = line.Substring(7);
                        }
                        // LN object used to denote an LN ending
                        else if (lineWithoutHash.StartsWith("lnobj"))
                        {
                            if (line.Length < 8)
                            {
                                IsValid = false;
                                return;
                            }

                            lnObject = lineLower.Substring(7);
                            // LNOBJ must be 2 characters.
                            if (lnObject.Length != 2)
                            {
                                IsValid = false;
                                return;
                            }
                        }
                        else if (lineWithoutHash.StartsWith("artist"))
                        {
                            if (line.Length < 9) continue;
                            metadata.Artist = line.Substring(8);
                        }
                        // Difficulty level as a number
                        else if (lineWithoutHash.StartsWith("playlevel"))
                        {
                            if (line.Length < 12) continue;
                            metadata.PlayLevel = line.Substring(11);
                        }
                        // The starting BPM. Not to be confused with #BPMXX
                        else if (lineWithoutHash.StartsWith("bpm "))
                        {
                            if (line.Length < 6)
                            {
                                IsValid = false;
                                return;
                            }

                            startThisTrackWithBpm = double.Parse(line.Substring(5));
                            TimingPoints.Add(new BmsTimingPoint
                            {
                                Bpm = startThisTrackWithBpm,
                                StartTime = 0.0
                            });
                        }
                        // Tempo change (#BPMXX)
                        else if (lineWithoutHash.StartsWith("bpm"))
                        {
                            if (line.Length < 8)
                            {
                                bpmChanges.Add(lineLower.Substring(4, 2), 0f);
                                continue;
                            }

                            bpmChanges.Add(lineLower.Substring(4, 2), double.Parse(line.Substring(7)));
                        }
                        // STOP command (#STOPXX)
                        else if (lineWithoutHash.StartsWith("stop"))
                        {
                            if (line.Length < 9)
                            {
                                continue;
                            }

                            var p = double.Parse(line.Substring(8));
                            if (p < 0.0)
                            {
                                continue;
                            }

                            stopCommands.Add(lineLower.Substring(5, 2), p);
                        }
                        // WAV command (#WAVXX)
                        else if (lineWithoutHash.StartsWith("wav"))
                        {
                            if (line.Length < 8)
                            {
                                continue;
                            }

                            keySoundStringArray.Add(line.Substring(7));
                            hitSoundHexList.Add(lineLower.Substring(4, 2));
                        }

                        continue;
                    }

                    // The line is a valid instruction.
                    // Ex: #00111:01000001
                    var trackInteger = int.Parse(line.Substring(1, 3));
                    // Hexatridecimal (base-36).
                    var channel = lineLower.Substring(4, 2);

                    // Cannot parse maps with mines.
                    // Also, maps with mines typically use per-column SV, which cannot be used here.
                    if (mineRegex.IsMatch(channel))
                    {
                        IsValid = false;
                        return;
                    }

                    if (!trackData.ContainsKey(trackInteger))
                    {
                        trackData.Add(trackInteger, new List<BmsLocalTrackData>());
                    }

                    trackData[trackInteger].Add(new BmsLocalTrackData
                        {Channel = channel, Message = line.Length > 7 ? lineLower.Substring(7) : null});
                }

                // If there is no #MAKER header, the map creator must be the artist.
                if (metadata.Creator == null)
                {
                    metadata.Creator = metadata.Artist;
                }

                // Sort out the tracks by ascending order
                var list = trackData.Keys.ToList();
                list.Sort((x, y) => x.CompareTo(y));

                // Iterate over all tracks
                foreach (var track in list)
                {
                    // measureScale is a multiplier for 4/4
                    var measureScale = 1.0;

                    // Keep track of tempo changes & stop commands that occur in this track.
                    var localTempoChanges = new List<BmsLocalTempoChange>();
                    var localStopCommands = new List<BmsLocalStopCommand>();

                    // Read over all the lines in a track and take them apart.
                    // It's important to understand that objects' start time (in ms) are NOT finalized in this block.
                    foreach (var line in trackData[track])
                    {
                        switch (line.Channel)
                        {
                            case "02":
                                // Measure scale change
                                measureScale = double.Parse(line.Message);
                                continue;
                            case "08":
                            case "03":
                                // Tempo change (channels 8 and 3)
                                if (line.Message == null)
                                {
                                    // Lone tempo change (no value)
                                    localTempoChanges.Add(new BmsLocalTempoChange{Position = 0.0, Bpm = 0.0, IsNegative = false});
                                    continue;
                                }

                                for (var i = 0; i < line.Message.Length / 2; i++)
                                {
                                    var value = GetHexValueOfIndex(i, line.Message);
                                    if (value == "00")
                                    {
                                        continue;
                                    }

                                    // Channel 3 is a tempo change based on 00 to FF (1-255),
                                    // while channel 8 is calling a tempo change already known from #BPMXX.
                                    // Some BMS files use both channels for some reason.
                                    if (line.Channel == "08" && !bpmChanges.ContainsKey(value))
                                    {
                                        continue;
                                    }
                                    var bpm = line.Channel == "03" ? int.Parse(value, System.Globalization.NumberStyles.HexNumber) : bpmChanges[value];
                                    localTempoChanges.Add(new BmsLocalTempoChange
                                    {
                                        Position = PositionUtility.GetPositionInTrack(i, line.Message.Length),
                                        Bpm = Math.Abs(bpm),
                                        IsNegative = bpm < 0.0
                                    });
                                }

                                break;
                            case "09":
                                // STOP command
                                if (line.Message == null)
                                {
                                    continue;
                                }
                                for (var i = 0; i < line.Message.Length / 2; i++)
                                {
                                    var value = GetHexValueOfIndex(i, line.Message);
                                    if (value == "00")
                                    {
                                        continue;
                                    }

                                    if (!stopCommands.ContainsKey(value))
                                    {
                                        continue;
                                    }
                                    localStopCommands.Add(new BmsLocalStopCommand
                                    {
                                        Position = PositionUtility.GetPositionInTrack(i, line.Message.Length),
                                        Duration = stopCommands[value]
                                    });
                                }

                                break;
                        }
                    }

                    // Sort everything into ascending order. (It's necessary so that we know what range everything is in,
                    // and to tidy things up.)
                    if (localTempoChanges.Count > 0)
                    {
                        localTempoChanges.Sort((x, y) => x.Position.CompareTo(y.Position));
                    }

                    if (localStopCommands.Count > 0)
                    {
                        localStopCommands.Sort((x, y) => x.Position.CompareTo(y.Position));
                    }

                    // Actually give the objects starting times.
                    // All messages from this point forth need to be hexadecimal.
                    foreach (var line in trackData[track].Where(line => line.Message.Length % 2 == 0))
                    {
                        if (!(normalNoteRegex.IsMatch(line.Channel) ||
                              longNoteRegex.IsMatch(line.Channel) || line.Channel == "01" ))
                        {
                            continue;
                        }

                        for (var i = 0; i < line.Message.Length / 2; i++)
                        {
                            // This pair might be out of range; take extra precaution
                            if (i * 2 + 2 > line.Message.Length)
                            {
                                break;
                            }
                            var value = GetHexValueOfIndex(i, line.Message);

                            if (value == "00")
                            {
                                continue;
                            }

                            var objectStartTimeMs = PositionUtility.GetPositionOfStartTime(startThisTrackWithBpm,
                                localTempoChanges, localStopCommands, i, line.Message, measureScale);
                            var sfx = GetHitObjectKeySound(hitSoundHexList, value);

                            if (normalNoteRegex.IsMatch(line.Channel) ||
                                longNoteRegex.IsMatch(line.Channel))
                            {
                                // 1 - 36
                                var laneInt = getLaneNumber(line.Channel.Substring(1));
                                var hitObject = new BmsHitObject
                                {
                                    Lane = laneInt,
                                    StartTime = (int)( startTrackWithTime + objectStartTimeMs )
                                };
                                // Lane 6 is basically (x)6
                                if ( laneInt == 6 )
                                {
                                    hasScratchKey = true;
                                    hitObject.Lane = 8;
                                }
                                else if (laneInt >= 8)
                                {
                                    // Compensation for notes that exist at or beyond channel 18
                                    hitObject.Lane -= 2;
                                }

                                // This map has a note greater than the 8th column.
                                // For compatibility reasons, it's reasonable to assume that it cannot be parsed.
                                if (hitObject.Lane > 8)
                                {
                                    IsValid = false;
                                    return;
                                }

                                // LN (normal type; uses #LNOBJ)
                                if (!string.IsNullOrEmpty(lnObject) && value == lnObject)
                                {
                                    if (HitObjects.Count == 0)
                                    {
                                        continue;
                                    }

                                    var back = HitObjects.Count - 1;
                                    if (HitObjects[back].KeySound == null)
                                    {
                                        // This means the previous object was an LNOBJ, so we ignore THIS one.
                                        continue;
                                    }

                                    if (HitObjects[back].StartTime >= hitObject.StartTime)
                                    {
                                        // LN has a negative duration
                                        continue;
                                    }

                                    HitObjects[back].IsLongNote = true;
                                    HitObjects[back].EndTime = hitObject.StartTime;
                                    continue;
                                }
                                hitObject.KeySound = sfx;

                                // LN (special type; uses channels 51-59)
                                if (longNoteRegex.IsMatch(line.Channel))
                                {
                                    if (!longNoteTracker.ContainsKey(hitObject.Lane))
                                    {
                                        longNoteTracker.Add(hitObject.Lane, 0.0);
                                    }
                                    if (longNoteTracker[hitObject.Lane] != 0.0)
                                    {
                                        hitObject.EndTime = hitObject.StartTime;
                                        hitObject.StartTime = longNoteTracker[hitObject.Lane];
                                        hitObject.IsLongNote = true;
                                        if (longNoteSoundEffectTracker.ContainsKey(hitObject.Lane) && longNoteSoundEffectTracker[hitObject.Lane] != null)
                                        {
                                            hitObject.KeySound = longNoteSoundEffectTracker[hitObject.Lane];
                                        }

                                        longNoteTracker[hitObject.Lane] = 0.0;
                                        longNoteSoundEffectTracker[hitObject.Lane] = null;
                                    }
                                    else
                                    {
                                        longNoteTracker[hitObject.Lane] = hitObject.StartTime;
                                        longNoteSoundEffectTracker[hitObject.Lane] = hitObject.KeySound;
                                        continue;
                                    }
                                    if (hitObject.EndTime <= hitObject.StartTime)
                                    {
                                        continue;
                                    }
                                }
                                HitObjects.Add(hitObject);
                                continue;
                            }
                            if (line.Channel == "01")
                            {
                                var soundEffect = new BmsSoundEffect
                                {
                                    StartTime = (int)( startTrackWithTime + objectStartTimeMs )
                                };

                                if (sfx != null)
                                {
                                    soundEffect.Sample = sfx.Sample;
                                    soundEffect.Volume = sfx.Volume;
                                }
                                else
                                {
                                    continue;
                                }
                                SoundEffects.Add(soundEffect);
                            }
                        }
                    }

                    var fullLengthOfTrack = DurationUtility.GetTotalTrackDuration(startThisTrackWithBpm, localTempoChanges,
                        localStopCommands, measureScale);
                    var timingPoints = PositionUtility.GetPositionOfTimingPoints(startTrackWithTime, startThisTrackWithBpm,
                        localTempoChanges, localStopCommands, measureScale);
                    foreach (var t in timingPoints)
                    {
                        TimingPoints.Add(t);
                    }

                    if (localTempoChanges.Count > 0)
                    {
                        startThisTrackWithBpm = localTempoChanges.Last().Bpm;
                    }

                    startTrackWithTime += fullLengthOfTrack;

                    TimingPoints.Add(new BmsTimingPoint()
                    {
                        StartTime = startTrackWithTime,
                        Bpm = startThisTrackWithBpm
                    });
                }
            }
            catch
            {
                // If there was a problem in parsing the map headers, the code should return.
                IsValid = false;
            }
        }

        private static BmsSound GetHitObjectKeySound(IReadOnlyList<string> hitSoundHexArray, string target)
        {
            for (var i = 0; i < hitSoundHexArray.Count; i++)
            {
                if (target == hitSoundHexArray[i])
                {
                    return new BmsSound()
                    {
                        Sample = i + 1,
                        Volume = 100
                    };
                }
            }

            return null;
        }

        private static string GetDifficultyName(string initialNum) => "Lv. " + initialNum;
        private static string GetHexValueOfIndex(int index, string message) => message.Substring(index * 2, 2);

        public Qua ToQua()
        {
            var quaFile = new Qua
            {
                AudioFile = "virtual",
                SongPreviewTime = -1,
                MapId = -1,
                MapSetId = -1,
                Mode = GameMode.Keys7,
                HasScratchKey = hasScratchKey,
                Title = metadata.Title,
                Artist = metadata.Artist,
                Source = "BMS",
                Tags = metadata.Tags,
                Creator = metadata.Creator,
                DifficultyName = GetDifficultyName(metadata.PlayLevel),
                Description = $"This is a BMS map converted to Quaver (original creator: {metadata.Creator})",
            };

            foreach (var cas in keySoundStringArray)
            {
                quaFile.CustomAudioSamples.Add(new CustomAudioSampleInfo()
                {
                    Path = cas,
                    UnaffectedByRate = false
                });
            }

            foreach (var sfx in SoundEffects)
            {
                quaFile.SoundEffects.Add(new SoundEffectInfo()
                {
                    Sample = sfx.Sample,
                    StartTime = (float) sfx.StartTime,
                    Volume = sfx.Volume
                });
            }

            foreach (var tp in TimingPoints)
            {
                quaFile.TimingPoints.Add(new TimingPointInfo()
                {
                    StartTime = (float) tp.StartTime,
                    Bpm = (float) tp.Bpm
                });
            }

            foreach (var obj in HitObjects)
            {
                quaFile.HitObjects.Add(new HitObjectInfo()
                {
                    StartTime = (int) obj.StartTime,
                    Lane = obj.Lane,
                    EndTime = obj.IsLongNote ? (int) obj.EndTime : 0,
                    KeySounds = obj.KeySound != null ? new List<KeySoundInfo>()
                    {
                        new KeySoundInfo()
                        {
                            Sample = obj.KeySound.Sample,
                            Volume = obj.KeySound.Volume
                        }
                    } : new List<KeySoundInfo>()
                });
            }

            return quaFile;
        }

        private static int getLaneNumber(string input) => "0123456789abcdefghijklmnopqrstuvwxyz".IndexOf(input, StringComparison.InvariantCulture);
    }
}