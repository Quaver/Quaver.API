/**
 * StepMania Parser provided by: zardoru
 * https://gist.github.com/zardoru/5298155#file-fixed_converter-L498
 */

using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.StepMania
{
    public class Difficulty
    {
        public string ChartName { get; set; }

        public List<Note> Notes { get; set; }

        public int KeyCount { get; set; }

        public List<BpmPair> TimingSections { get; set; }

        public string Style { get; set; }
    }
}