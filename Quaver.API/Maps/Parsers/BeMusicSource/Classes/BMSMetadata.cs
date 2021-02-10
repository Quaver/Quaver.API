using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.BeMusicSource.Classes
{
    public class BMSMetadata
    {
        public string Title { get; set; }

        public string Artist { get; set; }

        public string Tags { get; set; }

        public string Difficulty { get; set; }

        public string StageFile { get; set; }

        public string SubTitle { get; set; }

        public List<string> SubArtists { get; set; }
    }
}