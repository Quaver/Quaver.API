using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Quaver.API.Maps;

namespace Quaver.API.StepMania
{
    public class StepManiaFile
    {
        /// <summary>
        ///     The title of the track
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     The source equivalent in osu?
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        ///     The artist of the track
        /// </summary>
        public string Artist { get; set; }

        /// <summary>
        ///     The creator of the map
        /// </summary>
        public string Credit { get; set; }

        /// <summary>
        ///     The audio file
        /// </summary>
        public string Music { get; set; }

        /// <summary>
        ///     The background file
        /// </summary>
        public string Background { get; set; }

        /// <summary>
        ///     The offset that the song starts at
        /// </summary>
        public float Offset { get; set; }

        /// <summary>
        ///     The time in the song where the song's preview is played.
        /// </summary>
        public float SampleStart { get; set; }

        /// <summary>
        ///     The BPMs of the map
        /// </summary>
        public List<Bpm> Bpms { get; set; }

        /// <summary>
        ///     The list of charts in the map.
        /// </summary>
        public List<Chart> Charts { get; set; }

        /// <summary>
        ///     Parses a StepManiaFile
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static StepManiaFile Parse(string path)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var sm = new StepManiaFile { Bpms = new List<Bpm>(), Charts = new List<Chart>() };

            var file = File.ReadAllLines(path);

            var inNotes = false; // Keeps track of if we are currently in the #NOTES section
            var inNoteData = false; // Keeps track of it we are parsing the actual note data.
            var currentColons = 0; // Keeps track of the current amount of colons in the notes section

            foreach (var line in file)
            {
                if (line.Contains("#"))
                {
                    var key = line.Substring(0, line.IndexOf(':')).Trim().ToUpper();
                    var value = line.Split(':').Last().Trim();

                    if (key != "#NOTES")
                        value = value.Replace(";", "");

                    switch (key)
                    {
                        case "#TITLE":
                            sm.Title = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                            continue;
                        case "#SUBTITLE":
                            sm.Subtitle = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                            continue;
                        case "#ARTIST":
                            sm.Artist = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                            continue;
                        case "#CREDIT":
                            sm.Credit = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                            continue;
                        case "#MUSIC":
                            sm.Music = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                            continue;
                        case "#BACKGROUND":
                            sm.Background = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
                            continue;
                        case "#OFFSET":
                            sm.Offset = float.Parse(value);
                            continue;
                        case "#SAMPLESTART":
                            sm.SampleStart = float.Parse(value);
                            continue;
                        case "#BPMS":
                            var bpms = value.Split(',').ToList();

                            foreach (var bpm in bpms)
                            {
                                // An individual bpm is split by "offset=bpm"
                                var bpmSplit = bpm.Split('=').ToList();
                                sm.Bpms.Add(new Bpm { Beats = (int)float.Parse(bpmSplit[0], CultureInfo.InvariantCulture), BeatsPerMinute = float.Parse(bpmSplit[1], CultureInfo.InvariantCulture) });
                            }             
                            continue;
                        case "#NOTES":
                            inNotes = true;
                            sm.Charts.Add(new Chart { Measures = new List<NoteMeasure>() });
                            currentColons = 0; // Reset colon counter.
                            inNoteData = false;
                            continue;
                    }
                }

                if (inNotes)
                {
                    // Skip comments
                    if (line.Contains("//"))
                        continue;

                    if (line.Contains(":"))
                        currentColons++;

                    switch (currentColons)
                    {
                        // Type of map
                        case 1:
                            // Only parse dance-single maps. Setting inNotes to false will 
                            // cause the parser to skip this step in the parsing phase until it reaches
                            // another #NOTES section - Remove the current chart as well
                            if (!line.ToLower().Contains("dance-single"))
                            {
                                sm.Charts.Remove(sm.Charts.Last());
                                inNotes = false;
                                continue;
                            }

                            sm.Charts.Last().ChartType = line.Trim().Trim(':');
                            break;
                        // Creator? Don't need to parse this.
                        case 2:
                            break;
                        // Difficulty
                        case 3:
                            if (line.ToLower().Contains("beginner") || line.ToLower().Contains("easy") ||
                                line.ToLower().Contains("medium") || line.ToLower().Contains("hard") ||
                                line.ToLower().Contains("challenge"))
                            {
                                sm.Charts.Last().Difficulty = line.Trim().Trim(':');
                            }
                            break;
                        // Numerical meter, doesn't need parsing.
                        case 4:
                            break;
                        // Groove Radar Values - doesn't need parsing.
                        case 5:
                            // However, after this line, we will be parsing note data, 
                            inNoteData = true;
                            currentColons = -1; // Set the current colons back to -1 to avoid skipping.
                            sm.Charts.Last().Measures.Add(new NoteMeasure { NoteRows = new List<NoteRow>() }); // Add first measure of notes
                            continue;
                    }

                    // Parse Note Data
                    if (inNoteData)
                    {
                        // If there are 4 characters in this line, that must mean we're at a row of objects
                        if (line.Trim().Length == 4)
                        {
                            var row = new NoteRow
                            {
                                Lane1 = NoteType.None,
                                Lane2 = NoteType.None,
                                Lane3 = NoteType.None,
                                Lane4 = NoteType.None
                            };

                            // Turn the row of objects into an array so we can try to parse each row.
                            var rowArray = line.Trim().ToCharArray();

                            for (var i = 0; i < rowArray.Length; i++)
                            {
                                switch (i)
                                {
                                    // Lane 1
                                    case 0:
                                        row.Lane1 = Enum.TryParse(rowArray[i].ToString(), out NoteType typeLane1) ? typeLane1 : NoteType.None;
                                        break;
                                    // Lane 2
                                    case 1:
                                        row.Lane2 = Enum.TryParse(rowArray[i].ToString(), out NoteType typeLane2) ? typeLane2 : NoteType.None;
                                        break;
                                    // Lane 3
                                    case 2:
                                        row.Lane3 = Enum.TryParse(rowArray[i].ToString(), out NoteType typeLane3) ? typeLane3 : NoteType.None;
                                        break;
                                    // Lane 4
                                    case 3:
                                        row.Lane4 = Enum.TryParse(rowArray[i].ToString(), out NoteType typeLane4) ? typeLane4 : NoteType.None;
                                        break;
                                }
                            }

                            // Add the row to the list of measures
                            sm.Charts.Last().Measures.Last().NoteRows.Add(row);
                            continue;
                        }

                        // If the line is a ',', that means it marks the end of a new measure.
                        if (line.Trim().Contains(","))
                            sm.Charts.Last().Measures.Add(new NoteMeasure { NoteRows = new List<NoteRow>() });
                    }
                }
            }

            return sm;
        }

        /// <summary>
        ///     Gets the BPM Index from a given beat in the map
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        public static int GetBpmIndexFromBeat(StepManiaFile sm, int beat)
        {
            var currentBpm = 0;

            for (var i = 0; i < sm.Bpms.Count; i++)
            {
                if (sm.Bpms[i].Beats <= beat)
                    currentBpm = i;
            }

            return currentBpm;
        }

        /// <summary>
        ///     Converts an individual lane to a HitObject
        /// </summary>
        /// <param name="time"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static void ConvertLaneToHitObject(List<HitObjectInfo> currentObjects, float time, int lane, NoteType type)
        {
            var hitObject = new HitObjectInfo() { Lane = lane };

            switch (type)
            {
                case NoteType.None:
                    break;
                case NoteType.Normal:
                    hitObject.StartTime = (int)time;
                    break;
                case NoteType.HoldHead:
                    hitObject.StartTime = (int)time;
                    hitObject.EndTime = -2147483648;
                    break;
                case NoteType.HoldTail:
                    // For tails we need to find the last Object in this lane that has a HoldHead
                    // and update it with the end time
                    for (var i = currentObjects.Count; i > 0; i--)
                    {
                        if (currentObjects[i - 1]?.Lane != lane)
                            continue;

                        if (currentObjects[i - 1].EndTime == -2147483648)
                            currentObjects[i - 1].EndTime = (int)time;
                    }                      
                    break;
                default:
                    break;
            }

            // Add the new HitObject
            if (hitObject.StartTime != 0)
                currentObjects.Add(hitObject);
        }
    }

    /// <summary>
    ///     The #BPMS section of the metadata header
    /// </summary>
    public struct Bpm
    {
        /// <summary>
        ///     The amount of beats in the map the bpm begins at
        /// </summary>
        public int Beats { get; set; }

        /// <summary>
        ///     The actual BPM
        /// </summary>
        public float BeatsPerMinute { get; set; }
    }

    /// <summary>
    ///     The chart date itself, also known as the #NOTES section of the file
    /// </summary>
    public class Chart
    {
        /// <summary>
        ///     The type of chart
        ///     dance-single, etc.
        /// </summary>
        public string ChartType { get; set; }

        /// <summary>
        ///     The description/author of the chart
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     The difficulty name of the chart
        /// </summary>
        public string Difficulty { get; set; }

        /// <summary>
        ///     The list measures containing note data
        /// </summary>
        public List<NoteMeasure> Measures { get; set; }
    }

    // An individual measure for the map (as separated by ',' in the .sm file)
    public struct NoteMeasure
    {
        /// <summary>
        ///     The list of notes in this measure
        /// </summary>
        public List<NoteRow> NoteRows { get; set; }
    }

    /// <summary>
    ///     An individual row of notes
    /// </summary>
    public struct NoteRow
    {
        public NoteType Lane1 { get; set; }
        public NoteType Lane2 { get; set; }
        public NoteType Lane3 { get; set; }
        public NoteType Lane4 { get; set; }
    }

    /// <summary>
    ///     The type of note
    /// </summary>
    public enum NoteType
    {
        None, // 0
        Normal, // 1
        HoldHead, // 2
        HoldTail, // 3
    }
}
