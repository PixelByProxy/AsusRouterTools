using System.Diagnostics;
using Newtonsoft.Json;
using PixelByProxy.Asus.Router.Json;

namespace PixelByProxy.Asus.Router.Models
{
    [DebuggerDisplay("{Ip}:{Mac}:{DisplayName}")]
    public class Client
    {
        public string? DisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(NickName))
                    return NickName;

                if (!string.IsNullOrWhiteSpace(Name))
                    return Name;

                return Mac;
            }
        }

        public string? Rog { get; set; }

        [JsonProperty("amesh_bind_band")]
        public string? AmeshBindBand { get; set; }

        [JsonProperty("amesh_bind_mac")]
        public string? AmeshBindMac { get; set; }

        public string? Callback { get; set; }

        public string? CurRx { get; set; }

        public string? CurTx { get; set; }

        public string? DefaultType { get; set; }

        public string? DpiDevice { get; set; }

        public string? DbiType { get; set; }

        public string? From { get; set; }

        public string? Group { get; set; }

        public string? InternetMode { get; set; }

        public string? InternetState { get; set; }

        public string? Ip { get; set; }

        public string? IpMethod { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsGn { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsGateway { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsITunes { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsLogin { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsOnline { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsPrinter { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsWl { get; set; }

        [JsonConverter(typeof(BoolConverter))]
        public bool IsWebServer { get; set; }

        public string? Keeparp { get; set; }

        public string? Mac { get; set; }

        public string? MacRepeat { get; set; }

        public string? Name { get; set; }

        public string? NickName { get; set; }

        public string? OpMode { get; set; }

        public string? QosLevel { get; set; }

        public string? Rssi { get; set; }

        public string? Ssid { get; set; }

        public string? TotalRx { get; set; }

        public string? TotalTx { get; set; }

        public string? Type { get; set; }

        public string? Vendor { get; set; }

        public string? WlConnectTime { get; set; }

        public string? WtFast { get; set; }
    }
}
