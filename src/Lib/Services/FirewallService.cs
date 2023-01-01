using PixelByProxy.Asus.Router.Configuration;
using PixelByProxy.Asus.Router.Extensions;
using PixelByProxy.Asus.Router.Models;
using System.Text.RegularExpressions;
using System.Web;

namespace PixelByProxy.Asus.Router.Services;

/// <inheritdoc cref="IFirewallService" />
public class FirewallService : ServiceBase, IFirewallService
{
    #region Constants

    private const int MaxRules = 128;

    #endregion

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
        var response = await GetResponseAsync("/Advanced_BasicFirewall_Content.asp", HttpMethod.Get, null, null, true, cancellationToken).ConfigureAwait(false);

        var match = FirewallRegex.Match(response);

        if (!match.Success)
            throw new IndexOutOfRangeException("Unable to match the Firewall settings.");

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
                    Protocol = ruleParts.IndexAtOrDefault(4).ToEnum<IpV6Protocol>()
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
                    Protocol = ruleParts.IndexAtOrDefault(0).ToEnum<IpV4Protocol>(),
                    SourceIp = ruleParts.IndexAtOrDefault(2),
                    PortRange = ruleParts.IndexAtOrDefault(5)
                };

                settings.IpV4FirewallRules.Add(rule);
            }
        }

        return settings;
    }

    /// <inheritdoc />
    public async Task SetFirewallSettingsAsync(bool? enabled = null, IEnumerable<FirewallRuleIpV6>? ipv6FirewallRules = null, IEnumerable<FirewallRuleIpV4>? ipv4FirewallRules = null, CancellationToken cancellationToken = default)
    {
        var settings = new Dictionary<string, string>
        {
            { "action_mode", "apply" },
            { "action_script", "restart_firewall" }
        };

        if (enabled.HasValue)
            settings.Add("fw_enable_x", enabled.Value ? "1" : "0");

        if (ipv6FirewallRules != null)
        {
            var ruleList = ipv6FirewallRules.ToList();

            // validation
            if (ruleList.Count > MaxRules)
                throw new ArgumentException($"The rule list cannot exceed {MaxRules} items.", nameof(ipv6FirewallRules));

            string? errorMessage = null;
            var invalidRule = ruleList.FirstOrDefault(rule => !rule.IsRuleValid(out errorMessage));
            if (invalidRule != null)
                throw new ArgumentException(errorMessage, nameof(ipv6FirewallRules));

            settings.Add("ipv6_fw_rulelist", string.Join(string.Empty, ruleList.Select(rule => rule.Serialize())));
        }

        if (ipv4FirewallRules != null)
        {
            var ruleList = ipv4FirewallRules.ToList();

            // validation
            if (ruleList.Count > MaxRules)
                throw new ArgumentException($"The rule list cannot exceed {MaxRules} items.", nameof(ipv4FirewallRules));

            string? errorMessage = null;
            var invalidRule = ruleList.FirstOrDefault(rule => !rule.IsRuleValid(out errorMessage));
            if (invalidRule != null)
                throw new ArgumentException(errorMessage, nameof(ipv6FirewallRules));

            settings.Add("filter_wllist", string.Join(string.Empty, ruleList.Select(rule => rule.Serialize())));
        }

        await GetResponseAsync("/start_apply.htm", HttpMethod.Post, null, settings, true, cancellationToken)
            .ConfigureAwait(false);
    }
}