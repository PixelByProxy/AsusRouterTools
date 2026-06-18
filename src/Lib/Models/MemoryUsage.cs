using Newtonsoft.Json;

namespace PixelByProxy.Asus.Router.Models
{
    /// <summary>
    /// Represents the router's memory usage statistics.
    /// </summary>
    public class MemoryUsage
    {
        /// <summary>
        /// Gets or sets the total amount of memory.
        /// </summary>
        [JsonProperty("mem_total")]
        public long Total { get; set; }

        /// <summary>
        /// Gets or sets the amount of free memory.
        /// </summary>
        [JsonProperty("mem_free")]
        public long Free { get; set; }

        /// <summary>
        /// Gets or sets the amount of used memory.
        /// </summary>
        [JsonProperty("mem_used")]
        public long Used { get; set; }
    }
}
