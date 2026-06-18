using Newtonsoft.Json;

namespace PixelByProxy.Asus.Router.Models
{
    /// <summary>
    /// Represents the router's per-core CPU usage statistics.
    /// </summary>
    public class CpuUsage
    {
        /// <summary>
        /// Gets or sets the total CPU ticks for core 1.
        /// </summary>
        [JsonProperty("cpu1_total")]
        public long Cpu1Total { get; set; }

        /// <summary>
        /// Gets or sets the used CPU ticks for core 1.
        /// </summary>
        [JsonProperty("cpu1_usage")]
        public long Cpu1Usage { get; set; }

        /// <summary>
        /// Gets or sets the total CPU ticks for core 2.
        /// </summary>
        [JsonProperty("cpu2_total")]
        public long Cpu2Total { get; set; }

        /// <summary>
        /// Gets or sets the used CPU ticks for core 2.
        /// </summary>
        [JsonProperty("cpu2_usage")]
        public long Cpu2Usage { get; set; }

        /// <summary>
        /// Gets or sets the total CPU ticks for core 3.
        /// </summary>
        [JsonProperty("cpu3_total")]
        public long Cpu3Total { get; set; }

        /// <summary>
        /// Gets or sets the used CPU ticks for core 3.
        /// </summary>
        [JsonProperty("cpu3_usage")]
        public long Cpu3Usage { get; set; }

        /// <summary>
        /// Gets or sets the total CPU ticks for core 4.
        /// </summary>
        [JsonProperty("cpu4_total")]
        public long Cpu4Total { get; set; }

        /// <summary>
        /// Gets or sets the used CPU ticks for core 4.
        /// </summary>
        [JsonProperty("cpu4_usage")]
        public long Cpu4Usage { get; set; }
    }
}
