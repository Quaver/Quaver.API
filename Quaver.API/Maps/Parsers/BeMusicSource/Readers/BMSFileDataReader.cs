using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Quaver.API.Maps.Parsers.BeMusicSource.Classes;
using Quaver.API.Maps.Parsers.BeMusicSource.Utils;

namespace Quaver.API.Maps.Parsers.BeMusicSource.Readers
{
    public class BMSFileDataReader
    {
        public readonly BMSFileData FileData = new BMSFileData()
        {
            Metadata = new BMSMetadata()
            {
                Title = "No title",
                Artist = "Unknown artist",
                Difficulty = "Unknown",
                SubArtists = new List<string>()
            },
            // bm98 says that if #bpm n isn't specified the default is 130.
            // weird init?
            StartingBPM = 130.0,
            SoundHexPairs = new List<string>(),
            SoundFiles = new List<string>(),
            BPMChangeIndex = new Dictionary<string, double>(),
            StopIndex = new Dictionary<string, double>()
        };

        public readonly SortedDictionary<int, List<BMSLineData>> TrackLines = new SortedDictionary<int, List<BMSLineData>>();

        public readonly string InvalidReason;

        public BMSFileDataReader(string inputPath)
        {
            var ignoreLine = false;
            try
            {
                foreach (var line in File.ReadAllLines(inputPath, Encoding.GetEncoding("shift_jis")))
                {
                    if (!line.StartsWith("#")) continue;
                    var lineLower = line.ToLower();

                    if (lineLower.StartsWith("#end") && ignoreLine)
                    {
                        ignoreLine = false;
                        continue;
                    }

                    if (lineLower.StartsWith("#if") && !ignoreLine && (line.Length < 5 || line[4] != '1'))
                    {
                        ignoreLine = true;
                        continue;
                    }

                    if (ignoreLine) continue;

                    // shut the fuck up
                    if (line.Length < 7 || line[6] != ':')
                    {
                        if (lineLower.StartsWith("#player") && ( line.Length < 9 || line[8] != '1' ))
                        {
                            InvalidReason = "Not 1p mode";
                            return;
                        };
                        if (lineLower.StartsWith("#genre"))
                        {
                            if (line.Length < 8) continue;
                            FileData.Metadata.Tags = line.Substring(7);
                        }
                        else if (lineLower.StartsWith("#subtitle"))
                        {
                            if (line.Length < 11) continue;
                            FileData.Metadata.SubTitle = line.Substring(10);
                        }
                        else if (lineLower.StartsWith("#subartist"))
                        {
                            if (line.Length < 12) continue;
                            FileData.Metadata.SubArtists.Add(line.Substring(11));
                        }
                        else if (lineLower.StartsWith("#title"))
                        {
                            if (line.Length < 8) continue;
                            FileData.Metadata.Title = line.Substring(7);
                        }
                        else if (lineLower.StartsWith("#lnobj"))
                        {
                            if (line.Length < 8 || line.Substring(7).Length != 2) continue;
                            FileData.LNObject = lineLower.Substring(7);
                        }
                        else if (lineLower.StartsWith("#artist"))
                        {
                            if (line.Length < 9) continue;
                            FileData.Metadata.Artist = line.Substring(8);
                        }
                        else if (lineLower.StartsWith("#playlevel"))
                        {
                            if (line.Length < 12) continue;
                            FileData.Metadata.Difficulty = line.Substring(11);
                        }
                        else if (lineLower.StartsWith("#stagefile"))
                        {
                            if (line.Length < 12) continue;
                            FileData.Metadata.StageFile = line.Substring(11);
                        }
                        else if (lineLower.StartsWith("#bpm "))
                        {
                            if (line.Length < 6)
                            {
                                InvalidReason = "Initial bpm invalid";
                                return;
                            }

                            FileData.StartingBPM = double.Parse(line.Substring(5));
                        }
                        else if (lineLower.StartsWith("#bpm"))
                        {
                            if (line.Length < 8) continue;
                            FileData.BPMChangeIndex[lineLower.Substring(4, 2)] = double.Parse(line.Substring(7));
                        }
                        else if (lineLower.StartsWith("#stop"))
                        {
                            if (line.Length < 9) continue;
                            var yahweh = double.Parse(line.Substring(8));
                            // stops cant be negative
                            if (yahweh < 0.0) continue;

                            FileData.StopIndex[lineLower.Substring(5, 2)] = yahweh;
                        }
                        else if (lineLower.StartsWith("#wav"))
                        {
                            if (line.Length < 8) continue;

                            var soundEffect = BMSSoundEffectFixer.SearchForSoundFile(Path.GetDirectoryName(inputPath),
                                line.Substring(7));
                            if (string.IsNullOrEmpty(soundEffect)) continue;

                            FileData.SoundFiles.Add(soundEffect);
                            FileData.SoundHexPairs.Add(lineLower.Substring(4, 2));
                        }

                        continue;
                    }

                    var trackInteger = int.Parse(line.Substring(1, 3));
                    var thisLineData = new BMSLineData()
                    {
                        Channel = line.Substring(4, 2),
                        Message = line.Length > 7 ? lineLower.Substring(7) : ""
                    };

                    if (!TrackLines.ContainsKey(trackInteger))
                    {
                        TrackLines.Add(trackInteger, new List<BMSLineData>());
                    }

                    TrackLines[trackInteger].Add(thisLineData);
                }


                // i feel the exact same as what you're feeling right now don't worry
                var match = Regex.Match(FileData.Metadata.Title, "\\(([^)]*)\\)|-([^-]*)-|\\[([^\\]]*)\\]|'([^']*)'|\"([^\"]*)\"|\\~([^\\~]*)\\~", RegexOptions.RightToLeft);

                if (match.Groups.Count < 1) return;
                for (var i = match.Groups.Count - 1; i > 0; i--)
                {
                    if (match.Groups[i].Length == 0) continue;
                    FileData.Metadata.SubTitle = match.Groups[i].Value;
                    FileData.Metadata.Title = FileData.Metadata.Title
                        .Replace(match.Groups[0].Value, "");
                    break;
                }
            }
            catch (Exception e)
            {
                InvalidReason = e.Message;
            }
        }
    }
}