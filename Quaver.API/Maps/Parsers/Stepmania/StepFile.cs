using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quaver.API.Enums;
using Quaver.API.Maps.Structures;

namespace Quaver.API.Maps.Parsers.Stepmania
{
    public class StepFile
    {
        /// <summary>
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// </summary>
        public string Subtitle { get; private set; }

        /// <summary>
        /// </summary>
        public string Artist { get; private set; }

        /// <summary>
        /// </summary>
        public string TitleTranslit { get; private set; }

        /// <summary>
        /// </summary>
        public string SubtitleTranslit { get; private set; }

        /// <summary>
        /// </summary>
        public string ArtistTranslit { get; private set; }

        /// <summary>
        /// </summary>
        public string Credit { get; private set; }

        /// <summary>
        /// </summary>
        public string Banner { get; private set; }

        /// <summary>
        /// </summary>
        public string Background { get; private set; }

        /// <summary>
        /// </summary>
        public string LyricsPath { get; private set; }

        /// <summary>
        /// </summary>
        public string CdTitle { get; private set; }

        /// <summary>
        /// </summary>
        public string Music { get; private set; }

        /// <summary>
        /// </summary>
        public float MusicLength { get; private set; }

        /// <summary>
        /// </summary>
        public float Offset { get; private set; }

        /// <summary>
        /// </summary>
        public float SampleStart { get; private set; }

        /// <summary>
        /// </summary>
        public float SampleLength { get; private set; }

        /// <summary>
        /// </summary>
        public bool Selectable { get; private set; }

        /// <summary>
        /// </summary>
        public List<StepFileBPM> Bpms { get; private set; }

        /// <summary>
        /// </summary>
        public List<StepFileStop> Stops { get; private set; }

        /// <summary>
        /// </summary>
        public List<StepFileChart> Charts { get; } = new List<StepFileChart>();

        /// <summary>
        ///     Parses a Stepmania file from a file path
        /// </summary>
        /// <param name="path"></param>
        public StepFile(string path) => Parse(File.ReadAllLines(path));

        /// <summary>
        ///     Handles the actual parsing of the .sm file
        /// </summary>
        /// <param name="lines"></param>
        private void Parse(string[] lines)
        {
            // The currently parsed chart
            StepFileChart currentChart = null;

            var inBpms = false;
            var inStops = false;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("#") && trimmedLine.Contains(":"))
                {
                    var split = trimmedLine.Split(':');

                    var key = split[0].Replace("#", "");
                    var value = split[1].Replace(";", "").Replace(":", "");

                    switch (key)
                    {
                        case "TITLE":
                            Title = value;
                            break;
                        case "SUBTITLE":
                            Subtitle = value;
                            break;
                        case "ARTIST":
                            Artist = value;
                            break;
                        case "TITLETRANSLIT":
                            TitleTranslit = value;
                            break;
                        case "SUBTITLETRANSLIT":
                            SubtitleTranslit = value;
                            break;
                        case "ARTISTTRANSLIT":
                            ArtistTranslit = value;
                            break;
                        case "CREDIT":
                            Credit = value;
                            break;
                        case "BANNER":
                            Banner = value;
                            break;
                        case "BACKGROUND":
                            Background = value;
                            break;
                        case "LYRICSPATH":
                            LyricsPath = value;
                            break;
                        case "CDTITLE":
                            CdTitle = value;
                            break;
                        case "MUSIC":
                            Music = value;
                            break;
                        case "MUSICLENGTH":
                            MusicLength = float.Parse(value);
                            break;
                        case "OFFSET":
                            Offset = float.Parse(value);
                            break;
                        case "SAMPLESTART":
                            SampleStart = float.Parse(value);
                            break;
                        case "SAMPLELENGTH":
                            SampleLength = float.Parse(value);
                            break;
                        case "SELECTABLE":
                            break;
                        case "BPMS":
                            inBpms = true;
                            Bpms = StepFileBPM.Parse(value);
                            break;
                        case "STOPS":
                            inStops = true;
                            Stops = string.IsNullOrEmpty(value) ? new List<StepFileStop>() : StepFileStop.Parse(value);
                            break;
                        case "NOTES":
                            var chart = new StepFileChart();
                            currentChart = chart;
                            Charts.Add(chart);
                            break;
                    }
                }
                else if (inBpms)
                {
                    Bpms.AddRange(StepFileBPM.Parse(line));

                    if (line.Contains(";"))
                        inBpms = false;
                }
                else if (inStops)
                {
                    Stops.AddRange(StepFileStop.Parse(line));

                    if (line.Contains(";"))
                        inStops = false;
                }
                // Ignore comments
                else if (trimmedLine.StartsWith("//"))
                    continue;
                // To parse the chart metadata (type, difficulty, etc)
                else if (currentChart != null && currentChart.GrooveRadarValues == null)
                {
                    var value = trimmedLine.Replace(":", "");

                    if (currentChart.Type == null)
                        currentChart.Type = value;
                    else if (currentChart.Description == null)
                        currentChart.Description = value;
                    else if (currentChart.Difficulty == null)
                        currentChart.Difficulty = value;
                    else if (currentChart.NumericalMeter == null)
                        currentChart.NumericalMeter = value;
                    else if (currentChart.GrooveRadarValues == null)
                        currentChart.GrooveRadarValues = value;
                }
                // Parsing the actual notes (ex: 0001, 0100...)
                else if (currentChart != null && currentChart.GrooveRadarValues != null && !string.IsNullOrEmpty(trimmedLine))
                {
                    // Denotes a new measure
                    if (trimmedLine.StartsWith(","))
                    {
                        currentChart.Measures.Add(new StepFileChartMeasure(new List<List<StepFileChartNoteType>>()));
                        continue;
                    }

                    currentChart.Measures.Last().Notes.Add(StepFileChartMeasure.ParseLine(trimmedLine));
                }
            }
        }

        /// <summary>
        ///     Converts the file to Qua format
        /// </summary>
        /// <returns></returns>
        public List<Qua> ToQuas()
        {
            var quas = new List<Qua>();

            foreach (var chart in Charts)
            {
                var currentTime = -Offset * 1000;

                var qua = new Qua
                {
                    Title = Title,
                    Artist = Artist,
                    Creator = Credit,
                    BannerFile = Banner,
                    BackgroundFile = Background,
                    AudioFile = Music,
                    SongPreviewTime = (int) (SampleStart * 1000),
                    Mode = GameMode.Keys4,
                    DifficultyName = chart.Difficulty,
                };

                var totalBeats = 0f;
                var bpmCache = new List<StepFileBPM>(Bpms);
                var stopCache = new List<StepFileStop>(Stops);

                foreach (var measure in chart.Measures)
                {
                    var beatTimePerRow = 4.0f / measure.Notes.Count;

                    foreach (var row in measure.Notes)
                    {
                        // Add bpms at the current time if we've reached that beat
                        if (bpmCache.Count != 0 && totalBeats >= bpmCache.First().Beat)
                        {
                            qua.TimingPoints.Add(new TimingPointInfo
                            {
                                StartTime = currentTime,
                                Signature = TimeSignature.Quadruple,
                                Bpm = bpmCache.First().BPM
                            });

                            bpmCache.Remove(bpmCache.First());
                        }

                        for (var i = 0; i < row.Count; i++)
                        {
                            switch (row[i])
                            {
                                case StepFileChartNoteType.None:
                                    break;
                                // For normal objects, create a normal object
                                case StepFileChartNoteType.Normal:
                                    qua.HitObjects.Add(new HitObjectInfo
                                    {
                                        StartTime = (int) Math.Round(currentTime, MidpointRounding.AwayFromZero),
                                        Lane = i + 1
                                    });
                                    break;
                                // For hold heads, create a new object with an int.MinValue end time,
                                // so that it can be found later when the end pops up
                                case StepFileChartNoteType.Head:
                                    qua.HitObjects.Add(new HitObjectInfo
                                    {
                                        StartTime = (int) Math.Round(currentTime, MidpointRounding.AwayFromZero),
                                        EndTime = int.MinValue,
                                        Lane = i + 1
                                    });
                                    break;
                                // Find the last object in this lane that has an int.MinValue end time
                                case StepFileChartNoteType.Tail:
                                    var longNote = qua.HitObjects.FindLast(x => x.Lane == i + 1 && x.EndTime == int.MinValue);

                                    if (longNote != null)
                                        longNote.EndTime = (int) Math.Round(currentTime, MidpointRounding.AwayFromZero);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        currentTime += qua.GetTimingPointAt(currentTime).MillisecondsPerBeat * 4 / measure.Notes.Count;
                        totalBeats += beatTimePerRow;

                        if (stopCache.Count != 0 && totalBeats > stopCache.First().Beat)
                        {
                            currentTime += stopCache.First().Seconds * 1000;

                            qua.SliderVelocities.Add(new SliderVelocityInfo()
                            {
                                StartTime = currentTime - stopCache.First().Seconds * 1000,
                                Multiplier = 1
                            });

                            stopCache.Remove(stopCache.First());
                        }
                    }
                }

                quas.Add(qua);
            }

            return quas;
        }
    }
}