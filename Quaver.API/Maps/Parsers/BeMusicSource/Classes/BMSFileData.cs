using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.BeMusicSource.Classes
{
    public class BMSFileData
    {
        public BMSMetadata Metadata { get; set; }

        public List<string> SoundFiles { get; set; }

        public List<string> SoundHexPairs { get; set; }

        public double StartingBPM { get; set; }

        public string LNObject { get; set; }

        public Dictionary<string, double> BPMChangeIndex { get; set; }

        public Dictionary<string, double> StopIndex { get; set; }
    }
}