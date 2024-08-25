using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.Stepmania
{
    public class StepFileStop : IStepWithBeat
    {
        /// <summary>
        /// </summary>
        public float Beat { get; }

        /// <summary>
        /// </summary>
        public float Seconds { get; }

        /// <summary>
        /// </summary>
        /// <param name="beat"></param>
        /// <param name="seconds"></param>
        public StepFileStop(float beat, float seconds)
        {
            Beat = beat;
            Seconds = seconds;
        }

        /// <summary>
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static List<StepFileStop> Parse(string line)
        {
            var stops = new List<StepFileStop>();

            var split = line.Split(',');

            foreach (var stop in split)
            {
                if (string.IsNullOrEmpty(stop))
                    continue;

                var stopSplit = stop.Replace(",", "").Replace(";", "").Split('=');

                if (stopSplit.Length != 2)
                    continue;

                stops.Add(new StepFileStop(float.Parse(stopSplit[0]), float.Parse(stopSplit[1])));
            }

            return stops;
        }
    }
}