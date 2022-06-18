using Newtonsoft.Json;

namespace PixelByProxy.Asus.Router.Models
{
    public class MemoryUsage
    {
        [JsonProperty("mem_total")]
        public long Total { get; set; }

        [JsonProperty("mem_free")]
        public long Free { get; set; }

        [JsonProperty("mem_used")]
        public long Used { get; set; }
    }
}
