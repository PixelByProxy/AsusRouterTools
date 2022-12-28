using Newtonsoft.Json;

namespace PixelByProxy.Asus.Router.Models
{
    public class CpuUsage
    {
        [JsonProperty("cpu1_total")]
        public long Cpu1Total { get; set; }

        [JsonProperty("cpu1_usage")]
        public long Cpu1Usage { get; set; }

        [JsonProperty("cpu2_total")]
        public long Cpu2Total { get; set; }

        [JsonProperty("cpu2_usage")]
        public long Cpu2Usage { get; set; }

        [JsonProperty("cpu3_total")]
        public long Cpu3Total { get; set; }

        [JsonProperty("cpu3_usage")]
        public long Cpu3Usage { get; set; }

        [JsonProperty("cpu4_total")]
        public long Cpu4Total { get; set; }

        [JsonProperty("cpu4_usage")]
        public long Cpu4Usage { get; set; }
    }
}
