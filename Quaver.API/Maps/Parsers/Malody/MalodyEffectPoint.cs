using Newtonsoft.Json;
using System.Collections.Generic;

namespace Quaver.API.Maps.Parsers.Malody
{
    /// <summary>
    /// </summary>
    public class MalodyEffectPoint
    {
        [JsonProperty("beat")]
        public List<int> Beat { get; set; }

        [JsonProperty("scroll")]
        public float? Scroll { get; set; }
    }
}
