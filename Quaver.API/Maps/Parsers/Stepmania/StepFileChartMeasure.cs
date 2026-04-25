using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.Stepmania
{
    public class StepFileChartMeasure
    {
        /// <summary>
        /// </summary>
        public List<List<StepFileChartNoteType>> Notes { get; }

        /// <summary>
        /// </summary>
        /// <param name="notes"></param>
        public StepFileChartMeasure(List<List<StepFileChartNoteType>> notes) => Notes = notes;

        /// <summary>
        ///     Parses a single line of notes
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static List<StepFileChartNoteType> ParseLine(string line)
        {
            var notes = new List<StepFileChartNoteType>();

            foreach (var character in line)
            {
                var value = character switch
                {
                    '0' => StepFileChartNoteType.None,
                    '1' => StepFileChartNoteType.Normal,
                    '2' => StepFileChartNoteType.Head,
                    '3' => StepFileChartNoteType.Tail,
                    'M' => StepFileChartNoteType.Mine,
                    _ => StepFileChartNoteType.None
                };
                notes.Add(value);
            }

            return notes;
        }
    }
}