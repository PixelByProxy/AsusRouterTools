using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PixelByProxy.Asus.Router.Configuration;
using PixelByProxy.Asus.Router.Models;
using PixelByProxy.Asus.Router.Models.Responses;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace PixelByProxy.Asus.Router.Services
{
    /// <inheritdoc />
    public class AsusService : IAsusService
    {
        #region Constants

        private const string UserAgent = "asusrouter-Android-DUTUtil-1.0.0.245";

        #endregion

        #region Static Fields

        private static readonly string[] RouterSettingsKeys = {
            "time_zone", "time_zone_dst", "time_zone_x", "time_zone_dstoff",
            "ntp_server0", "acs_dfs", "productid", "apps_sq", "lan_hwaddr", "lan_ipaddr",
            "lan_proto", "x_Setting", "label_mac", "lan_netmask", "lan_gateway",
            "http_enable", "https_lanport", "wl0_country_code", "wl1_country_code" };

        private static readonly string[] LanStatusKeys = {
            "lan_hwaddr", "lan_ipaddr", "lan_proto", "lan_netmask", "lan_gateway" };

        private static readonly Regex UptimeRegex = new(@"(""uptime"":(?<Date>.*)\((?<Seconds>[0-9]*).*\))", RegexOptions.Compiled);
        private static readonly Regex WanStatusRegex = new(@"(.*function (?<Name>.*)\(\).*return '?(?<Value>.*[^'])('?;)})", RegexOptions.Compiled);

        #endregion

        #region Fields

        private readonly AsusConfiguration _asusConfig;
        private readonly HttpClient _httpClient;

        private string? _authToken;
        private string? _authCreds;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the service.
        /// </summary>
        /// <param name="asusConfig">The Asus router configuration settings.</param>
        /// <param name="httpClient">Http Client for communication with the Asus APIs.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AsusService(AsusConfiguration asusConfig, HttpClient httpClient)
        {
            _asusConfig = asusConfig ?? throw new ArgumentNullException(nameof(asusConfig));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        #endregion

        #region IAsusService Members

        /// <inheritdoc />
        public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var creds = $"{_asusConfig.UserName}:{_asusConfig.Password}";
                var login = Convert.ToBase64String(Encoding.ASCII.GetBytes(creds));
                var content = $"login_authorization={login}";

                var json = await GetResponseJsonAsync("/login.cgi", HttpMethod.Post, content, false, cancellationToken).ConfigureAwait(false);
                var token = json["asus_token"]!.ToString();
                return token;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new HttpRequestException("Authentication Failed", ex, HttpStatusCode.Unauthorized);
            }
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

        #region Private Methods

        private async Task<string> AuthenticateAsync(CancellationToken cancellationToken)
        {
            var currentCreds = $"{_asusConfig.UserName}:{_asusConfig.Password}";

            if (string.IsNullOrEmpty(_authToken) || !string.Equals(_authCreds, currentCreds, StringComparison.Ordinal))
            {
                _authToken = await GetTokenAsync(cancellationToken).ConfigureAwait(false);
                _authCreds = currentCreds;
            }

            return _authToken;
        }

        private async Task<JObject> GetHookResponseJsonAsync(string hook, CancellationToken cancellationToken)
        {
            var response = await GetHookResponseAsync(hook, cancellationToken).ConfigureAwait(false);
            var obj = JsonConvert.DeserializeObject<JObject>(response);
            return obj!;
        }

        private async Task<string> GetHookResponseAsync(string hook, CancellationToken cancellationToken)
        {
            var response = await GetResponseAsync($"/appGet.cgi?hook={hook}()", HttpMethod.Get, null, true, cancellationToken).ConfigureAwait(false);
            return response;
        }

        private async Task<JObject> GetResponseJsonAsync(string path, HttpMethod method, string? jsonContent, bool authenticate, CancellationToken cancellationToken)
        {
            var response = await GetResponseAsync(path, method, jsonContent, authenticate, cancellationToken).ConfigureAwait(false);
            var obj = JsonConvert.DeserializeObject<JObject>(response);
            return obj!;
        }

        private async Task<string> GetResponseAsync(string path, HttpMethod method, string? jsonContent, bool authenticate, CancellationToken cancellationToken)
        {
            var protocol = _asusConfig.UseHttps ? "https" : "http";
            var requestUri = new Uri($"{protocol}://{_asusConfig.HostName}{path}");

            var request = new HttpRequestMessage(method, requestUri);

            request.Headers.UserAgent.TryParseAdd(UserAgent);

            if (authenticate)
            {
                var token = await AuthenticateAsync(cancellationToken).ConfigureAwait(false);
                request.Headers.Add("Cookie", $"asus_token={token}");
            }

            if (!string.IsNullOrEmpty(jsonContent))
                request.Content = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);

            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (content.IndexOf("error_status", StringComparison.OrdinalIgnoreCase) > -1)
            {
                var errorJson = JsonConvert.DeserializeObject<JObject>(content)!;
                throw new HttpRequestException($"Error {errorJson["error_status"]}", null, HttpStatusCode.BadRequest);
            }

            return content;
        }

        private static DateTime ParseUnixTime(string unixTimeSeconds)
        {
            if (!long.TryParse(unixTimeSeconds, out var unixTime))
                return DateTime.MinValue;

            var dt = DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            return dt;
        }

        #endregion
    }
}
