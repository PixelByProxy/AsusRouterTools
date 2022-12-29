using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PixelByProxy.Asus.Router.Configuration;
using PixelByProxy.Asus.Router.Models;
using PixelByProxy.Asus.Router.Models.Responses;
using System.Text.RegularExpressions;

namespace PixelByProxy.Asus.Router.Services
{
    /// <inheritdoc cref="IAsusService" />
    public class AsusService : ServiceBase, IAsusService
    {
        #region Static Fields

        private static readonly string[] RouterSettingsKeys = {
            "time_zone", "time_zone_dst", "time_zone_x", "time_zone_dstoff",
            "ntp_server0", "acs_dfs", "productid", "apps_sq", "lan_hwaddr", "lan_ipaddr",
            "lan_proto", "x_Setting", "label_mac", "lan_netmask", "lan_gateway",
            "http_enable", "https_lanport", "wl0_country_code", "wl1_country_code" };

        private static readonly string[] LanStatusKeys = {
            "lan_hwaddr", "lan_ipaddr", "lan_proto", "lan_netmask", "lan_gateway" };

        private static readonly Regex UptimeRegex = new(@"(""uptime"":""(?<Date>.*)\((?<Seconds>[0-9]*).*\))", RegexOptions.Compiled);
        private static readonly Regex WanStatusRegex = new(@"(.*function (?<Name>.*)\(\).*return '?(?<Value>.*[^'])('?;)})", RegexOptions.Compiled);

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the service.
        /// </summary>
        /// <param name="asusConfig">The Asus router configuration settings.</param>
        /// <param name="httpClient">Http Client for communication with the Asus APIs.</param>
        public AsusService(AsusConfiguration asusConfig, HttpClient httpClient)
            : base(asusConfig, httpClient)
        {
        }

        #endregion

        #region IAsusService Members

        /// <inheritdoc />
        public Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            return GetAuthTokenAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Uptime?> GetUptimeAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetHookResponseAsync("uptime", cancellationToken).ConfigureAwait(false);

            // uptime doesn't return valid json so we use a regex
            var regex = UptimeRegex.Match(response);
            if (regex.Success)
            {
                var uptime = new Uptime
                {
                    TotalSeconds = regex.Groups["Seconds"].Value
                };

                if (DateTime.TryParse(regex.Groups["Date"].Value, out var parsedDate))
                {
                    uptime.Since = parsedDate;
                }

                return uptime;
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<MemoryUsage?> GetMemoryUsageAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetHookResponseAsync("memory_usage", cancellationToken).ConfigureAwait(false);

            // create valid json
            response = response.Replace("\"memory_usage\":", string.Empty);

            var obj = JsonConvert.DeserializeObject<MemoryUsage>(response);
            return obj;
        }

        /// <inheritdoc />
        public async Task<CpuUsage?> GetCpuUsageAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetHookResponseAsync("cpu_usage", cancellationToken).ConfigureAwait(false);

            // create valid json
            response = response.Replace("\"cpu_usage\":", string.Empty);

            var obj = JsonConvert.DeserializeObject<CpuUsage>(response);
            return obj;
        }

        /// <inheritdoc />
        public async Task<List<Client>> GetClientsAsync(CancellationToken cancellationToken = default)
        {
            var json = await GetHookResponseJsonAsync("get_clientlist", cancellationToken).ConfigureAwait(false);
            var clientsJson = json["get_clientlist"]?.ToObject<GetClientListResponse>();
            var clients = clientsJson?.ClientsJson?.Values.Select(j => j.ToObject<Client>()).Where(c => c != null && !string.IsNullOrEmpty(c.Mac)).Select(c => c!).ToList();
            return clients ?? new List<Client>();
        }

        /// <inheritdoc />
        public async Task<Traffic?> GetTrafficAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetHookResponseJsonAsync("netdev(appobj)", cancellationToken).ConfigureAwait(false);

            var netDev = response["netdev"]!;
            var sentHex = netDev["INTERNET_tx"]!.ToString();
            var receivedHex = netDev["INTERNET_rx"]!.ToString();

            // convert the hex to int
            var sent = Convert.ToInt64(sentHex.Substring(2), 16) * 8 / 1024 / 1024 / 2;
            var received = Convert.ToInt64(receivedHex.Substring(2), 16) * 8 / 1024 / 1024 / 2;
            var traffic = new Traffic { Sent = sent, Received = received };

            return traffic;
        }

        /// <inheritdoc />
        public async Task<WanStatus?> GetWanStatusAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetHookResponseAsync("wanlink()", cancellationToken).ConfigureAwait(false);

            // create valid json
            var matches = WanStatusRegex.Matches(response);

            if (matches.Any())
            {
                var json = new JObject();

                foreach (Match match in matches)
                {
                    var name = match.Groups["Name"].Value;
                    var value = match.Groups["Value"].Value;

                    json.Add(name, value);
                }

                var wan = json.ToObject<WanStatus>();
                return wan;
            }

            return null;
        }

        /// <inheritdoc />
        public async Task<RouterSettings?> GetRouterSettingsAsync(CancellationToken cancellationToken = default)
        {
            var requests = new List<Task<JObject>>();

            foreach (var key in RouterSettingsKeys)
            {
                requests.Add(GetHookResponseJsonAsync($"nvram_get({key})", cancellationToken));
            }

            var responses = await Task.WhenAll(requests).ConfigureAwait(false);
            if (!responses.Any())
                return null;

            var json = new JObject();

            foreach (var response in responses)
            {
                foreach (var prop in response)
                {
                    json.Add(prop.Key, prop.Value);
                }
            }

            var settings = json.ToObject<RouterSettings>();
            return settings;
        }

        /// <inheritdoc />
        public async Task<LanStatus?> GetLanStatusAsync(CancellationToken cancellationToken = default)
        {
            var requests = new List<Task<JObject>>();

            foreach (var key in LanStatusKeys)
            {
                requests.Add(GetHookResponseJsonAsync($"nvram_get({key})", cancellationToken));
            }

            var responses = await Task.WhenAll(requests).ConfigureAwait(false);
            if (!responses.Any())
                return null;

            var json = new JObject();

            foreach (var response in responses)
            {
                foreach (var prop in response)
                {
                    json.Add(prop.Key, prop.Value);
                }
            }

            var lan = json.ToObject<LanStatus>();
            return lan;
        }

        /// <inheritdoc />
        public async Task<List<List<string>>> GetDhcpLeaseMacListAsync(CancellationToken cancellationToken = default)
        {
            var response = await GetHookResponseJsonAsync("dhcpLeaseMacList", cancellationToken).ConfigureAwait(false);

            var list = response["dhcpLeaseMacList"]?.ToObject<List<List<string>>>();
            return list ?? new List<List<string>>();
        }

        /// <inheritdoc />
        public async Task<List<WebHistory>> GetWebHistory(string? clientMac = null, int page = 1, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(clientMac))
                clientMac = "all";

            if (page <= 0)
                page = 1;

            var response = await GetResponseAsync($"/getWebHistory.asp?client={clientMac}&page={page}", HttpMethod.Get, null, true, cancellationToken).ConfigureAwait(false);

            var webHistory = new List<WebHistory>();

            var jsonStart = response.IndexOf('[', StringComparison.Ordinal);
            if (jsonStart > -1)
            {
                var json = response[jsonStart..].Trim().TrimEnd(';');
                var entries = JsonConvert.DeserializeObject<List<List<string>>>(json);

                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        if (entry.Count < 3)
                            continue;

                        var history = new WebHistory
                        {
                            Mac = entry[0],
                            AccessTime = ParseUnixTime(entry[1]),
                            Domain = entry[2]
                        };

                        webHistory.Add(history);
                    }
                }
            }

            return webHistory;
        }

        #endregion
    }
}
