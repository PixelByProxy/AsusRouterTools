using System.Diagnostics;
using Newtonsoft.Json;
using PixelByProxy.Asus.Router.Json;

namespace PixelByProxy.Asus.Router.Models
{
    /// <summary>
    /// Represents a device (client) known to the router.
    /// </summary>
    [DebuggerDisplay("{Ip}:{Mac}:{DisplayName}")]
    public class Client
    {
        /// <summary>
        /// Gets the best available friendly name for the client, preferring the nickname, then the name, then the MAC address.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the ROG (Republic of Gamers) device indicator.
        /// </summary>
        public string? Rog { get; set; }

        /// <summary>
        /// Gets or sets the band the client is bound to in an AiMesh network.
        /// </summary>
        [JsonProperty("amesh_bind_band")]
        public string? AmeshBindBand { get; set; }

        /// <summary>
        /// Gets or sets the MAC address of the AiMesh node the client is bound to.
        /// </summary>
        [JsonProperty("amesh_bind_mac")]
        public string? AmeshBindMac { get; set; }

        /// <summary>
        /// Gets or sets the callback value reported by the router.
        /// </summary>
        public string? Callback { get; set; }

        /// <summary>
        /// Gets or sets the current receive rate.
        /// </summary>
        public string? CurRx { get; set; }

        /// <summary>
        /// Gets or sets the current transmit rate.
        /// </summary>
        public string? CurTx { get; set; }

        /// <summary>
        /// Gets or sets the default device type.
        /// </summary>
        public string? DefaultType { get; set; }

        /// <summary>
        /// Gets or sets the device name detected via deep packet inspection (DPI).
        /// </summary>
        public string? DpiDevice { get; set; }

        /// <summary>
        /// Gets or sets the device type detected via deep packet inspection (DPI).
        /// </summary>
        public string? DbiType { get; set; }

        /// <summary>
        /// Gets or sets the source the client information was reported from.
        /// </summary>
        public string? From { get; set; }

        /// <summary>
        /// Gets or sets the group the client belongs to.
        /// </summary>
        public string? Group { get; set; }

        /// <summary>
        /// Gets or sets the client's internet access mode.
        /// </summary>
        public string? InternetMode { get; set; }

        /// <summary>
        /// Gets or sets the client's current internet access state.
        /// </summary>
        public string? InternetState { get; set; }

        /// <summary>
        /// Gets or sets the client's IP address.
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// Gets or sets the method by which the client obtained its IP address (for example, "dhcp" or "static").
        /// </summary>
        public string? IpMethod { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client is connected to a guest network.
        /// </summary>
        [JsonConverter(typeof(BoolConverter))]
        public bool IsGn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client is a gateway.
        /// </summary>
        [JsonConverter(typeof(BoolConverter))]
        public bool IsGateway { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client is an iTunes (AirPlay/DAAP) device.
        /// </summary>
        [JsonConverter(typeof(BoolConverter))]
        public bool IsITunes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client is currently logged in to the router.
        /// </summary>
        [JsonConverter(typeof(BoolConverter))]
        public bool IsLogin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client is currently online.
        /// </summary>
        [JsonConverter(typeof(BoolConverter))]
        public bool IsOnline { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client is a printer.
        /// </summary>
        [JsonConverter(typeof(BoolConverter))]
        public bool IsPrinter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client is connected wirelessly.
        /// </summary>
        [JsonConverter(typeof(BoolConverter))]
        public bool IsWl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the client is a web server.
        /// </summary>
        [JsonConverter(typeof(BoolConverter))]
        public bool IsWebServer { get; set; }

        /// <summary>
        /// Gets or sets the keep-ARP value reported by the router.
        /// </summary>
        public string? Keeparp { get; set; }

        /// <summary>
        /// Gets or sets the client's MAC address.
        /// </summary>
        public string? Mac { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the MAC address is repeated.
        /// </summary>
        public string? MacRepeat { get; set; }

        /// <summary>
        /// Gets or sets the client's detected name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the user-assigned nickname for the client.
        /// </summary>
        public string? NickName { get; set; }

        /// <summary>
        /// Gets or sets the operation mode of the client.
        /// </summary>
        public string? OpMode { get; set; }

        /// <summary>
        /// Gets or sets the Quality of Service (QoS) level applied to the client.
        /// </summary>
        public string? QosLevel { get; set; }

        /// <summary>
        /// Gets or sets the wireless signal strength (RSSI) for the client.
        /// </summary>
        public string? Rssi { get; set; }

        /// <summary>
        /// Gets or sets the SSID the client is connected to.
        /// </summary>
        public string? Ssid { get; set; }

        /// <summary>
        /// Gets or sets the total amount of data received by the client.
        /// </summary>
        public string? TotalRx { get; set; }

        /// <summary>
        /// Gets or sets the total amount of data transmitted by the client.
        /// </summary>
        public string? TotalTx { get; set; }

        /// <summary>
        /// Gets or sets the client's device type.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// Gets or sets the client's hardware vendor.
        /// </summary>
        public string? Vendor { get; set; }

        /// <summary>
        /// Gets or sets the length of time the client has been connected wirelessly.
        /// </summary>
        public string? WlConnectTime { get; set; }

        /// <summary>
        /// Gets or sets the WTFast (gamers private network) status for the client.
        /// </summary>
        public string? WtFast { get; set; }
    }
}
