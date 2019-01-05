/**
 * StepMania Parser provided by: zardoru
 * https://gist.github.com/zardoru/5298155#file-fixed_converter-L498
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Quaver.API.Enums;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.Parsers.StepMania
{
    public class StepmaniaConverter
    {
        private Regex CommandFmt { get; } = new Regex("#(\\w+):(.+)?;");

        private string Artist { get; set; } = "";

        private string SongTitle { get; set; } = "";

        private string ChartAuthor { get; set; } = "";

        private string SongFile { get; set; } = "music.mp3";

        private string BackgroundFile { get; set; } = "";

        private List<FPair> BpmChanges { get; } = new List<FPair>();

        private List<FPair> Stops { get; } = new List<FPair>();

        private List<Difficulty> Difficulties { get; set; }

        private float Offset { get; set; }

        private float PreviewPoint { get; set; }

        public StepmaniaConverter(string path) => ReadHeader(path, true);

        /// <summary>
        ///     Converts a stepmania style to their associated key count.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        private static int GetKeyCountFromStyle(string style)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (style)
            {
                case "kb7-single":
                    return 7;
                case "dance-single":
                    return 4;
                case "dance-solo":
                    return 6;
                case "dance-double":
                    return 8;
                case "pump-single":
                    return 5;
                case "pump-double":
                    return 10;
            }

            return 4;
        }

        /// <summary>
        /// </summary>
        /// <param name="In"></param>
        /// <returns></returns>
        private static string ReadCommand(System.IO.StreamReader In)
        {
            var retval = "";

            while (In.Peek() != ';' && !In.EndOfStream)
            {
                while (In.Peek() == '\n' && !In.EndOfStream)
                    In.Read();

                if (In.Peek() != ';' && !In.EndOfStream)
                    retval += (char)In.Read();
            }

            while (In.Peek() == ';')
                In.Read();

            retval += ";";
            return retval;
        }

        /// <summary>
        ///     Parses an x=y list
        ///
        ///     if BPM is false, work with the stops list.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bpm"></param>
        private void ParseLists(string value, bool bpm)
        {
            var outList = bpm ? BpmChanges : Stops;

            var pairs = value.Split(',');

            foreach (var element in pairs)
            {
                var values = element.Split('=');

                if (values.GetLength(0) <= 1)
                    continue;

                float.TryParse(values[0], out var nBeat);
                float.TryParse(values[1], out var nValue);

                var newElement = new FPair
                {
                    Beat = nBeat,
                    Value = nValue
                };

                outList.Add(newElement);
            }
        }

        /// <summary>
        ///     Finds the BPM at a particular point in time.
        /// </summary>
        /// <param name="beat"></param>
        /// <returns></returns>
        private float GetBpmForBeat(float beat)
        {
            float best = 0;

            foreach (var value in BpmChanges)
            {
                if (value.Beat <= beat)
                    best = value.Value;
            }

            return best;
        }

        /// <summary>
        ///     Finds a stop at a particular point in time.
        /// </summary>
        /// <param name="beat"></param>
        /// <returns></returns>
        private float GetStopForBeat(float beat)
        {
            foreach (var value in Stops)
            {
                if (Math.Abs(value.Beat - beat) < 0.00001)
                    return value.Value * 1000;
            }

            return 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="beat"></param>
        /// <param name="nextbeat"></param>
        /// <returns></returns>
        private float SumStopInterval(float beat, float nextbeat)
        {
            float time = 0;

            foreach (var value in Stops)
            {
                if (value.Beat > beat && value.Beat < nextbeat)
                    time += value.Value;
            }

            return time;
        }

        /// <summary>
        /// </summary>
        /// <param name="lastBpm"></param>
        /// <param name="beat"></param>
        /// <param name="nextBeat"></param>
        /// <param name="altBpm"></param>
        /// <returns></returns>
        private float SumBpmInterval(float lastBpm, float beat, float nextBeat, out float altBpm)
        {
            float timeSum = 0;
            var bpmList = new List<FPair>();

            foreach (var pair in BpmChanges)
            {
                if (pair.Beat - beat > 0.0001 && nextBeat - pair.Beat > 0.0001)
                    bpmList.Add(pair);
            }

            if (bpmList.Count > 0)
            {
                var spb = 60 / lastBpm;

                // Calculate value for time between first bpm change in the interval and the current beat
                timeSum += spb * (bpmList[0].Beat - beat) * 1000;
                try
                {
                    timeSum += bpmList.Select((t, index) => 60000.0f / t.Value * ( bpmList[index + 1].Beat - t.Beat )).Sum();
                }
                catch (SystemException)
                {
                    // ignored
                }

                // Calculate value for last bpm change in the list and our next beat.
                timeSum += 60000.0f / bpmList[bpmList.Count - 1].Value * (nextBeat - bpmList[bpmList.Count - 1].Beat);
                altBpm = bpmList[bpmList.Count - 1].Value;
            }
            else
            {
                altBpm = lastBpm;
            }

            return timeSum;
        }

       /// <summary>
       ///     Parses a #notes tag
       /// </summary>
       /// <param name="value"></param>
       /// <returns></returns>
       /// <exception cref="Exception"></exception>
        private Difficulty ParseNotes(string value)
        {
            var currentTime = -Offset * 1000;
            float beat = 0;
            float curBpm = 0;

            var diff = new Difficulty();

            var theNotes = new List<Note>();

            diff.Notes = theNotes;

            // eliminate measure comments
            value = Regex.Replace(value, "\\/\\/ measure \\d+\r", "");

            // eliminate whitespace
            value = Regex.Replace(value, "\\s", "");

            // eliminate carriage returns
            value = value.Replace("\r", " ");

            var data = value.Split(':');

            if (data.Length < 6)
                throw new Exception("invalid data while parsing .sm file");

            // get key count
            diff.KeyCount = GetKeyCountFromStyle(data[0]);
            diff.Style = data[0];

            // make up space for holds
            var pendingTracks = new float[diff.KeyCount];

            diff.TimingSections = new List<BpmPair>();
            diff.ChartName = data[2];

            // now we work with the notes.
            var notes = data[5];
            var measures = notes.Split(',');

            foreach (var measure in measures)
            {
                var rows = measure.Length / diff.KeyCount;

                // measure len / rows. assume mlen = 4
                var fractionperrow  = 4.0f / rows;
                uint currentTrack = 0, mCount = 0;

                while (rows > 0)
                {
                    // this is only for timing sections
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (curBpm != GetBpmForBeat(beat))
                    {
                        // add a bpm change
                        var newPair = new BpmPair
                        {
                            Beatspace = 60000 / GetBpmForBeat(beat),
                            Time = currentTime
                        };

                        diff.TimingSections.Add(newPair);
                    }

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (GetStopForBeat(beat) != 0)
                    {
                        // add a stop.
                        var newpair = new BpmPair
                        {
                            Beatspace = 60000.0f / 0.000000001f,
                            Time = currentTime
                        };

                        // a small enough value
                        diff.TimingSections.Add(newpair);

                        newpair = new BpmPair
                        {
                            Beatspace = 60000 / GetBpmForBeat(beat),
                            Time = currentTime + GetStopForBeat(beat)
                        };

                        diff.TimingSections.Add(newpair);

                    }

                     // the next is for notes and everything else
                    curBpm = GetBpmForBeat(beat);

                    var mspb = 60000 / curBpm;

                    switch (measure[(int)mCount])
                    {
                        case '1':
                            var newNote = new Note
                            {
                                NoteType = ENoteType.Tap,
                                BeatStart = beat,
                                TrackStart = currentTime,
                                BeatEnd = 0,
                                Track = currentTrack
                            };
                            theNotes.Add(newNote);
                            break;
                        case '2': // hold start
                        case '4': // roll start
                            pendingTracks[currentTrack] = currentTime;
                            break;
                        case '3':
                            newNote = new Note
                            {
                                NoteType = ENoteType.Tap,
                                BeatEnd = beat,
                                TrackStart = pendingTracks[currentTrack],
                                TrackEnd = currentTime,
                                Track = currentTrack
                            };
                            theNotes.Add(newNote);
                            break;
                    }

                    mCount++;
                    currentTrack++;

                    if (currentTrack != diff.KeyCount)
                        continue;

                    currentTrack = 0;
                    rows--;

                    currentTime += fractionperrow * mspb + GetStopForBeat(beat);
                    var prevBeat = beat;
                    beat += fractionperrow;
                    currentTime += SumStopInterval(prevBeat, beat);
                    currentTime += SumBpmInterval(curBpm, prevBeat, beat, out var altBpm);
                    curBpm = altBpm;
                }
            }

            return diff;
        }

        /// <summary>
        ///     Reads the chart's headers.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="readNotes"></param>
        private void ReadHeader(string filename, bool readNotes = false)
        {
            var stream = new System.IO.StreamReader(filename);
            string line;

            Difficulties = new List<Difficulty>();

            while (CommandFmt.IsMatch(line = ReadCommand(stream)))
            {
                var matches = CommandFmt.Matches(line);
                foreach (Match m in matches)
                {
                    var key = m.Groups[1].Value;
                    var value = m.Groups[2].Value;

                    // Information relevant only to the metadata
                    switch (key)
                    {
                        case "TITLE":
                            SongTitle = value;
                            break;
                        case "OFFSET":
                            Offset = float.Parse(value);
                            break;
                        case "ARTIST":
                            Artist = value;
                            break;
                        case "CREDIT":
                            ChartAuthor = value;
                            break;
                        case "SAMPLESTART":
                            PreviewPoint = float.Parse(value);
                            break;
                        case "MUSIC":
                            SongFile = value;
                            break;
                        case "BACKGROUND":
                            BackgroundFile = value;
                            break;
                    }

                    if (!readNotes)
                        continue;

                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (key)
                    {
                        case "BPMS":
                            ParseLists(value, true);
                            break;
                        case "STOPS":
                            ParseLists(value, false);
                            break;
                        case "NOTES":
                            Difficulties.Add(ParseNotes(value));
                            break;
                    }
                }
            }
        }

        /// <summary>
        ///     Converts a StepMania file to a Qua object.
        /// </summary>
        /// <returns></returns>
        public List<Qua> ToQua()
        {
            var quas = new List<Qua>();

            foreach (var diff in Difficulties)
            {
                var qua = new Qua()
                {
                    AudioFile = SongFile,
                    SongPreviewTime = (int) ( PreviewPoint * 1000 ),
                    Title = SongTitle,
                    Artist = Artist,
                    Creator = ChartAuthor,
                    DifficultyName = diff.ChartName,
                    MapId = -1,
                    MapSetId = -1,
                    BackgroundFile = BackgroundFile,
                };

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (diff.KeyCount)
                {
                    case 4:
                        qua.Mode = GameMode.Keys4;
                        break;
                    case 7:
                        qua.Mode = GameMode.Keys7;
                        break;
                }

                foreach (var point in diff.TimingSections)
                {
                    qua.TimingPoints.Add(new TimingPointInfo
                    {
                        StartTime = point.Time,
                        Bpm = 60000 / point.Beatspace
                    });
                }

                foreach (var note in diff.Notes)
                {
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (note.TrackEnd == 0)
                    {
                        qua.HitObjects.Add(new HitObjectInfo
                        {
                            StartTime = (int) note.TrackStart,
                            Lane = (int) note.Track + 1
                        });
                    }
                    else
                    {
                        qua.HitObjects.Add(new HitObjectInfo
                        {
                            StartTime = (int) note.TrackStart,
                            Lane = (int) note.Track + 1,
                            EndTime = (int) note.TrackEnd,
                        });
                    }
                }

                quas.Add(qua);
            }

            return quas;
        }
    }
}