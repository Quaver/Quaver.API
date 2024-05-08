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

                            if (line.Contains(";"))
                                inBpms = false;
                            break;
                        case "STOPS":
                            inStops = true;
                            Stops = string.IsNullOrEmpty(value) ? new List<StepFileStop>() : StepFileStop.Parse(value);

                            if (line.Contains(";"))
                                inStops = false;
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
                else if (currentChart != null && currentChart.GrooveRadarValues != null &&
                         !string.IsNullOrEmpty(trimmedLine))
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
            return Charts.Select(ConvertChart).ToList();
        }

        private Qua ConvertChart(StepFileChart chart)
        {
            var qua = new Qua
            {
                Title = Title,
                Artist = Artist,
                Creator = Credit,
                BannerFile = Banner,
                BackgroundFile = Background,
                AudioFile = Music,
                SongPreviewTime = (int)(SampleStart * 1000),
                Mode = GameMode.Keys4,
                DifficultyName = chart.Difficulty,
                BPMDoesNotAffectScrollVelocity = true,
                InitialScrollVelocity = 1,
            };

            // Combine BPM changes and stops together and order by its beat time
            var bpmAndStops = new Queue<StepBpmOrStop>(Bpms.Select(b => new StepBpmOrStop(b))
                .Concat(
                    Stops.Select(s => new StepBpmOrStop(s)))
                .OrderBy(s => s.Beat));

            // The starting beat time of the current measure
            var measureBeats = 0;

            var startTime = -Offset * 1000;

            // The starting time of the measure on which the most recent BPM change was added
            var lastBpmChangeMeasureTime = startTime;

            // The number of measures passed from the last BPM change
            var measureCountSinceLastChange = 0;

            var millisecondsPerMeasure = 0f;
            var millisecondsPerBeat = 0f;

            foreach (var measure in chart.Measures)
            {
                // Calculate the time the measure starts
                var measureTime = lastBpmChangeMeasureTime + measureCountSinceLastChange * millisecondsPerMeasure;

                var beatTimePerRow = 4.0f / measure.Notes.Count;
                var millisecondsPerRow = millisecondsPerMeasure / measure.Notes.Count;

                for (var rowIndex = 0; rowIndex < measure.Notes.Count; rowIndex++)
                {
                    var row = measure.Notes[rowIndex];
                    var totalBeats = measureBeats + rowIndex * beatTimePerRow;
                    var currentTime = measureTime + rowIndex * millisecondsPerRow;

                    AddRow(row, qua, currentTime);

                    // Process every bpm and stops between the current row and the next (inclusive-exclusive)
                    while (bpmAndStops.Count > 0 && totalBeats + beatTimePerRow > bpmAndStops.Peek().Beat)
                    {
                        var bpmOrStop = bpmAndStops.Dequeue();
                        if (bpmOrStop.IsBpm)
                        {
                            var bpm = bpmOrStop.Bpm;
                            // Fraction of row before the timing point is placed
                            var insertTime = currentTime + millisecondsPerBeat * (bpm.Beat - totalBeats);
                            var newTimingPointInfo = new TimingPointInfo
                            {
                                StartTime = insertTime,
                                Signature = TimeSignature.Quadruple,
                                Bpm = bpm.BPM
                            };
                            qua.TimingPoints.Add(newTimingPointInfo);

                            // Update the conversion related variables
                            millisecondsPerBeat = newTimingPointInfo.MillisecondsPerBeat;
                            millisecondsPerMeasure = newTimingPointInfo.MillisecondsPerBeat * 4;
                            millisecondsPerRow = millisecondsPerMeasure / measure.Notes.Count;

                            // The measure time is the "virtual" measure time
                            // The "virtual" measure has the same BPM as the recently added one and
                            // will have the same beat number at the time when the new timing point is reached.
                            // It is as if the BPM has always been the new BPM added
                            var beatsPassed = bpm.Beat - measureBeats;
                            lastBpmChangeMeasureTime = insertTime - beatsPassed * millisecondsPerBeat;
                            measureCountSinceLastChange = 0;
                            measureTime = lastBpmChangeMeasureTime;
                            currentTime = measureTime + rowIndex * millisecondsPerRow;
                        }
                        else
                        {
                            var stop = bpmOrStop.Stop;
                            var stopMilliseconds = stop.Seconds * 1000;
                            qua.SliderVelocities.Add(new SliderVelocityInfo
                            {
                                StartTime = currentTime,
                                Multiplier = 0
                            });
                            // Shift the entire time forward
                            lastBpmChangeMeasureTime += stopMilliseconds;
                            measureTime += stopMilliseconds;
                            currentTime += stopMilliseconds;
                            qua.SliderVelocities.Add(new SliderVelocityInfo
                            {
                                StartTime = currentTime,
                                Multiplier = 1
                            });
                        }
                    }
                }

                // A measure has 4 beats
                measureBeats += 4;
                measureCountSinceLastChange++;
            }

            return qua;
        }

        /// <summary>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="qua"></param>
        /// <param name="currentTime"></param>
        private static void AddRow(List<StepFileChartNoteType> row, Qua qua, float currentTime)
        {
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
                            StartTime = (int)Math.Truncate(currentTime),
                            Lane = i + 1
                        });
                        break;
                    // For hold heads, create a new object with an int.MinValue end time,
                    // so that it can be found later when the end pops up
                    case StepFileChartNoteType.Head:
                        qua.HitObjects.Add(new HitObjectInfo
                        {
                            StartTime = (int)Math.Truncate(currentTime),
                            EndTime = int.MinValue,
                            Lane = i + 1
                        });
                        break;
                    // Find the last object in this lane that has an int.MinValue end time
                    case StepFileChartNoteType.Tail:
                        var longNote = qua.HitObjects.FindLast(x =>
                            x.Lane == i + 1 && x.EndTime == int.MinValue);

                        if (longNote != null)
                            longNote.EndTime = (int)Math.Round(currentTime, MidpointRounding.AwayFromZero);
                        break;
                }
            }
        }
    }
}