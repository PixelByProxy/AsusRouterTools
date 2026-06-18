using Newtonsoft.Json;

namespace PixelByProxy.Asus.Router.Models
{
    /// <summary>
    /// Represents the router's LAN network status.
    /// </summary>
    public class LanStatus
    {
        /// <summary>
        /// Gets or sets the hardware (MAC) address of the LAN interface.
        /// </summary>
        [JsonProperty("lan_hwaddr")]
        public string? LanHwAddr { get; set; }

        /// <summary>
        /// Gets or sets the LAN IP address.
        /// </summary>
        [JsonProperty("lan_ipaddr")]
        public string? LanIp { get; set; }

        /// <summary>
        /// Gets or sets the LAN protocol (for example, "static" or "dhcp").
        /// </summary>
        [JsonProperty("lan_proto")]
        public string? LanProto { get; set; }

        /// <summary>
        /// Gets or sets the LAN subnet mask.
        /// </summary>
        [JsonProperty("lan_netmask")]
        public string? LanNetMask { get; set; }

        /// <summary>
        /// Gets or sets the LAN gateway address.
        /// </summary>
        [JsonProperty("lan_gateway")]
        public string? LanGateway { get; set; }
    }
}
