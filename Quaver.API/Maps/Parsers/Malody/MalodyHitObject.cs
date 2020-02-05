using Newtonsoft.Json;
using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.Malody
{
    /// <summary>
    /// </summary>
    public class MalodyHitObject
    {
        [JsonProperty("beat")]
        public List<int> Beat { get; set; }

        [JsonProperty("endbeat")]
        public List<int> BeatEnd { get; set; }

        [JsonProperty("column")]
        public int Column { get; set; }

        [JsonProperty("volume")]
        public int Volume { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("sound")]
        public string Sound { get; set; }
    }
}
