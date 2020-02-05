using Newtonsoft.Json;

namespace Quaver.API.Maps.Parsers.Malody
{
    /// <summary>
    /// </summary>
    public class MalodyMetaKeymode
    {
        [JsonProperty("column")]
        public int Keymode { get; set; }
    }
}
