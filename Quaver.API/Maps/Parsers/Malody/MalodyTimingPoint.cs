using Newtonsoft.Json;
using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.Malody
{
    /// <summary>
    /// </summary>
    public class MalodyTimingPoint
    {
        [JsonProperty("beat")]
        public List<int> Beat { get; set; }

        [JsonProperty("bpm")]
        public float Bpm { get; set; }
    }
}
