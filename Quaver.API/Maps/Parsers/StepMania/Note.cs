/**
 * StepMania Parser provided by: zardoru
 * https://gist.github.com/zardoru/5298155#file-fixed_converter-L498
 */

namespace Quaver.API.Maps.Parsers.StepMania
{
    public struct Note
    {
        public ENoteType NoteType { get; set; }

        public float BeatStart { get; set; }

        public float BeatEnd { get; set; }

        public float TrackStart { get; set; }

        public float TrackEnd { get; set; }

        public uint Track { get; set; }
    }
}