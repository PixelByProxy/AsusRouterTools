using Newtonsoft.Json;

namespace PixelByProxy.Asus.Router.Models
{
    public class LanStatus
    {
        [JsonProperty("lan_hwaddr")]
        public string? LanHwAddr { get; set; }

        [JsonProperty("lan_ipaddr")]
        public string? LanIp { get; set; }

        [JsonProperty("lan_proto")]
        public string? LanProto { get; set; }

        [JsonProperty("lan_netmask")]
        public string? LanNetMask { get; set; }

        [JsonProperty("lan_gateway")]
        public string? LanGateway { get; set; }
    }
}
