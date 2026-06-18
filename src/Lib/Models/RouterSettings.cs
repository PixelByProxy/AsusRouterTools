using Newtonsoft.Json;

namespace PixelByProxy.Asus.Router.Models
{
    /// <summary>
    /// Represents a collection of general router settings.
    /// </summary>
    public class RouterSettings
    {
        /// <summary>
        /// Gets or sets the configured time zone.
        /// </summary>
        [JsonProperty("time_zone")]
        public string? TimeZone { get; set; }

        /// <summary>
        /// Gets or sets the daylight saving time rule for the time zone.
        /// </summary>
        [JsonProperty("time_zone_dst")]
        public string? TimeZoneDst { get; set; }

        /// <summary>
        /// Gets or sets the extended time zone value.
        /// </summary>
        [JsonProperty("time_zone_x")]
        public string? TimeZoneX { get; set; }

        /// <summary>
        /// Gets or sets the daylight saving time offset.
        /// </summary>
        [JsonProperty("time_zone_dstoff")]
        public string? TimeZoneDstOff { get; set; }

        /// <summary>
        /// Gets or sets the primary NTP server used for time synchronization.
        /// </summary>
        [JsonProperty("ntp_server0")]
        public string? NtpServer0 { get; set; }

        /// <summary>
        /// Gets or sets the Auto Channel Selection (ACS) DFS setting.
        /// </summary>
        [JsonProperty("acs_dfs")]
        public string? AcsDfs { get; set; }

        /// <summary>
        /// Gets or sets the router's product identifier (model).
        /// </summary>
        [JsonProperty("productid")]
        public string? ProductId { get; set; }

        /// <summary>
        /// Gets or sets the apps signature query value.
        /// </summary>
        [JsonProperty("apps_sq")]
        public string? AppsSq { get; set; }

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
        /// Gets or sets the initial setup flag indicating whether the router has been configured.
        /// </summary>
        [JsonProperty("x_Setting")]
        public string? XSetting { get; set; }

        /// <summary>
        /// Gets or sets the MAC address printed on the router's label.
        /// </summary>
        [JsonProperty("label_mac")]
        public string? LabelMac { get; set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether HTTP access to the web admin console is enabled.
        /// </summary>
        [JsonProperty("http_enable")]
        public string? HttpEnable { get; set; }

        /// <summary>
        /// Gets or sets the LAN port used for HTTPS access to the web admin console.
        /// </summary>
        [JsonProperty("https_lanport")]
        public string? HttpsLanPort { get; set; }

        /// <summary>
        /// Gets or sets the country code for the first wireless radio.
        /// </summary>
        [JsonProperty("wl0_country_code")]
        public string? Wl0CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the country code for the second wireless radio.
        /// </summary>
        [JsonProperty("wl1_country_code")]
        public string? Wl1CountryCode { get; set; }
    }
}
