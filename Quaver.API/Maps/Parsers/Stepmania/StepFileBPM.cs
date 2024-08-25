using System;
using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.Stepmania
{
    public class StepFileBPM : IStepWithBeat
    {
        /// <summary>
        /// </summary>
        public float Beat { get; }

        /// <summary>
        /// </summary>
        public float BPM { get; }

        /// <summary>
        /// </summary>
        /// <param name="beat"></param>
        /// <param name="bpm"></param>
        public StepFileBPM(float beat, float bpm)
        {
            Beat = beat;
            BPM = bpm;
        }

        /// <summary>
        ///     Parses the `#BPMS` line in a stepfile, and returns the list of bpms.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static List<StepFileBPM> Parse(string line)
        {
            var bpms = new List<StepFileBPM>();

            var split = line.Split(',');

            foreach (var bpm in split)
            {
                if (string.IsNullOrEmpty(bpm))
                    continue;

                var bpmSplit = bpm.Replace(",", "").Replace(";", "").Split('=');

                if (bpmSplit.Length != 2)
                    continue;

                bpms.Add(new StepFileBPM(float.Parse(bpmSplit[0]), float.Parse(bpmSplit[1])));
            }

            return bpms;
        }
    }
}