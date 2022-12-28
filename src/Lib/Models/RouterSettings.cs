using Newtonsoft.Json;

namespace PixelByProxy.Asus.Router.Models
{
    public class RouterSettings
    {
        [JsonProperty("time_zone")]
        public string? TimeZone { get; set; }

        [JsonProperty("time_zone_dst")]
        public string? TimeZoneDst { get; set; }

        [JsonProperty("time_zone_x")]
        public string? TimeZoneX { get; set; }

        [JsonProperty("time_zone_dstoff")]
        public string? TimeZoneDstOff { get; set; }

        [JsonProperty("ntp_server0")]
        public string? NtpServer0 { get; set; }

        [JsonProperty("acs_dfs")]
        public string? AcsDfs { get; set; }

        [JsonProperty("productid")]
        public string? ProductId { get; set; }

        [JsonProperty("apps_sq")]
        public string? AppsSq { get; set; }

        [JsonProperty("lan_hwaddr")]
        public string? LanHwAddr { get; set; }

        [JsonProperty("lan_ipaddr")]
        public string? LanIp { get; set; }

        [JsonProperty("lan_proto")]
        public string? LanProto { get; set; }

        [JsonProperty("x_Setting")]
        public string? XSetting { get; set; }

        [JsonProperty("label_mac")]
        public string? LabelMac { get; set; }

        [JsonProperty("lan_netmask")]
        public string? LanNetMask { get; set; }

        [JsonProperty("lan_gateway")]
        public string? LanGateway { get; set; }

        [JsonProperty("http_enable")]
        public string? HttpEnable { get; set; }

        [JsonProperty("https_lanport")]
        public string? HttpsLanPort { get; set; }

        [JsonProperty("wl0_country_code")]
        public string? Wl0CountryCode { get; set; }

        [JsonProperty("wl1_country_code")]
        public string? Wl1CountryCode { get; set; }
    }
}
