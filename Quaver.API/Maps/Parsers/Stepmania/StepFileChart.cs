using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.Stepmania
{
    public class StepFileChart
    {
        /// <summary>
        ///     dance-single, etc
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        public string Difficulty { get; set; }

        /// <summary>
        /// </summary>
        public string NumericalMeter { get; set; }

        /// <summary>
        /// </summary>
        public string GrooveRadarValues { get; set; }

        /// <summary>
        /// </summary>
        public List<StepFileChartMeasure> Measures { get; } = new List<StepFileChartMeasure>
        {
            // Here we intiialize an empty measure because the file format does not start with a comma
            new StepFileChartMeasure(new List<List<StepFileChartNoteType>>())
        };
    }
}