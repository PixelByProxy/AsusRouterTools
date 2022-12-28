using Newtonsoft.Json;
using PixelByProxy.Asus.Router.Json;

namespace PixelByProxy.Asus.Router.Models
{
    public class WanStatus
    {
        [JsonProperty("wanlink_status"), JsonConverter(typeof(BoolConverter))]
        public bool IsOnline { get; set; }

        [JsonProperty("wanlink_statusstr")]
        public string? Status { get; set; }

        [JsonProperty("wanlink_type")]
        public string? Type { get; set; }

        [JsonProperty("wanlink_ipaddr")]
        public string? Ip { get; set; }

        [JsonProperty("wanlink_netmask")]
        public string? NetMask { get; set; }

        [JsonProperty("wanlink_gateway")]
        public string? Gateway { get; set; }

        [JsonProperty("wanlink_dns")]
        public string? Dns { get; set; }

        [JsonProperty("wanlink_lease")]
        public long Lease { get; set; }

        [JsonProperty("wanlink_expires")]
        public long Expires { get; set; }

        [JsonProperty("is_private_subnet"), JsonConverter(typeof(BoolConverter))]
        public bool IsPrivateSubnet { get; set; }

        [JsonProperty("wanlink_xipaddr")]
        public string? XIp { get; set; }

        [JsonProperty("wanlink_xnetmask")]
        public string? XNetMask { get; set; }

        [JsonProperty("wanlink_xgateway")]
        public string? XGateway { get; set; }

        [JsonProperty("wanlink_xlease")]
        public long XLease { get; set; }

        [JsonProperty("wanlink_xexpires")]
        public long XExpires { get; set; }
    }
}
