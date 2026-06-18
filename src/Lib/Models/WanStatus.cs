using Newtonsoft.Json;
using PixelByProxy.Asus.Router.Json;

namespace PixelByProxy.Asus.Router.Models
{
    /// <summary>
    /// Represents the router's WAN (internet) connection status.
    /// </summary>
    public class WanStatus
    {
        /// <summary>
        /// Gets or sets a value indicating whether the WAN link is online.
        /// </summary>
        [JsonProperty("wanlink_status"), JsonConverter(typeof(BoolConverter))]
        public bool IsOnline { get; set; }

        /// <summary>
        /// Gets or sets the textual description of the WAN link status.
        /// </summary>
        [JsonProperty("wanlink_statusstr")]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the WAN connection type (for example, "dhcp" or "static").
        /// </summary>
        [JsonProperty("wanlink_type")]
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the WAN IP address.
        /// </summary>
        [JsonProperty("wanlink_ipaddr")]
        public string? Ip { get; set; }

        /// <summary>
        /// Gets or sets the WAN subnet mask.
        /// </summary>
        [JsonProperty("wanlink_netmask")]
        public string? NetMask { get; set; }

        /// <summary>
        /// Gets or sets the WAN gateway address.
        /// </summary>
        [JsonProperty("wanlink_gateway")]
        public string? Gateway { get; set; }

        /// <summary>
        /// Gets or sets the WAN DNS server address(es).
        /// </summary>
        [JsonProperty("wanlink_dns")]
        public string? Dns { get; set; }

        /// <summary>
        /// Gets or sets the DHCP lease duration, in seconds.
        /// </summary>
        [JsonProperty("wanlink_lease")]
        public long Lease { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds until the DHCP lease expires.
        /// </summary>
        [JsonProperty("wanlink_expires")]
        public long Expires { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the WAN IP address is on a private subnet.
        /// </summary>
        [JsonProperty("is_private_subnet"), JsonConverter(typeof(BoolConverter))]
        public bool IsPrivateSubnet { get; set; }

        /// <summary>
        /// Gets or sets the secondary (xDSL/extended) WAN IP address.
        /// </summary>
        [JsonProperty("wanlink_xipaddr")]
        public string? XIp { get; set; }

        /// <summary>
        /// Gets or sets the secondary (xDSL/extended) WAN subnet mask.
        /// </summary>
        [JsonProperty("wanlink_xnetmask")]
        public string? XNetMask { get; set; }

        /// <summary>
        /// Gets or sets the secondary (xDSL/extended) WAN gateway address.
        /// </summary>
        [JsonProperty("wanlink_xgateway")]
        public string? XGateway { get; set; }

        /// <summary>
        /// Gets or sets the secondary (xDSL/extended) WAN DHCP lease duration, in seconds.
        /// </summary>
        [JsonProperty("wanlink_xlease")]
        public long XLease { get; set; }

        /// <summary>
        /// Gets or sets the number of seconds until the secondary (xDSL/extended) WAN DHCP lease expires.
        /// </summary>
        [JsonProperty("wanlink_xexpires")]
        public long XExpires { get; set; }
    }
}
