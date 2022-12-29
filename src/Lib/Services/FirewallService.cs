using PixelByProxy.Asus.Router.Configuration;
using PixelByProxy.Asus.Router.Models;
using System.Text.RegularExpressions;
using System.Web;
using PixelByProxy.Asus.Router.Extensions;

namespace PixelByProxy.Asus.Router.Services;

/// <inheritdoc cref="IFirewallService" />
public class FirewallService : ServiceBase, IFirewallService
{
    #region Static Fields

    private static readonly Regex FirewallRegex = new(@"<script>[\s\S]+(firewall_enable = '(?<FirewallEnable>.*)';)[\s\S]+(ipv6_fw_rulelist_array = ""(?<IpV6RuleList>.*)"";)[\s\S]+(ipv4_fw_rulelist_array = ""(?<IpV4RuleList>.*)"";)", RegexOptions.Compiled);

    #endregion

    /// <summary>
    /// Creates a new instance of the service.
    /// </summary>
    /// <param name="asusConfig">The Asus router configuration settings.</param>
    /// <param name="httpClient">Http Client for communication with the Asus APIs.</param>
    public FirewallService(AsusConfiguration asusConfig, HttpClient httpClient)
        : base(asusConfig, httpClient)
    {
    }

    /// <inheritdoc />
    public async Task<FirewallSettings> GetFirewallSettingsAsync(CancellationToken cancellationToken = default)
    {
        var response = await GetResponseAsync("/Advanced_BasicFirewall_Content.asp", HttpMethod.Get, null, true, cancellationToken).ConfigureAwait(false);

        var match = FirewallRegex.Match(response);

        if (!match.Success)
            throw new EntryPointNotFoundException();

        var settings = new FirewallSettings();

        // parse firewall enabled
        var enabled = match.Groups["FirewallEnable"].Value;
        settings.Enabled = string.Equals(enabled, "1", StringComparison.Ordinal);

        // parse IPV6 firewall rules
        var ipv6Rules = match.Groups["IpV6RuleList"].Value;
        if (!string.IsNullOrEmpty(ipv6Rules))
        {
            ipv6Rules = HttpUtility.UrlDecode(ipv6Rules);
            var entries = ipv6Rules.Split('<', StringSplitOptions.RemoveEmptyEntries);

            foreach (var entry in entries)
            {
                var ruleParts = entry.Split('>');

                var rule = new FirewallRuleIpV6
                {
                    ServiceName = ruleParts.IndexAtOrDefault(0),
                    RemoteIp = ruleParts.IndexAtOrDefault(1),
                    LocalIp = ruleParts.IndexAtOrDefault(2),
                    PortRange = ruleParts.IndexAtOrDefault(3),
                    Protocol = ruleParts.IndexAtOrDefault(4)
                };

                settings.IpV6FirewallRules.Add(rule);
            }
        }

        // parse IPV4 firewall rules
        var ipv4Rules = match.Groups["IpV4RuleList"].Value;
        if (!string.IsNullOrEmpty(ipv4Rules))
        {
            ipv4Rules = HttpUtility.UrlDecode(ipv4Rules);
            var entries = ipv4Rules.Split('<', StringSplitOptions.RemoveEmptyEntries);

            foreach (var entry in entries)
            {
                var ruleParts = entry.Split('>');

                var rule = new FirewallRuleIpV4
                {
                    Protocol = ruleParts.IndexAtOrDefault(0),
                    SourceIp = ruleParts.IndexAtOrDefault(2),
                    PortRange = ruleParts.IndexAtOrDefault(5)
                };

                settings.IpV4FirewallRules.Add(rule);
            }
        }

        return settings;
    }
}