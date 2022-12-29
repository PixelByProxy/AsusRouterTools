using System.Net;
using PixelByProxy.Asus.Router.Configuration;
using PixelByProxy.Asus.Router.Models;
using System.Text.RegularExpressions;
using System.Web;
using PixelByProxy.Asus.Router.Extensions;

namespace PixelByProxy.Asus.Router.Services;

/// <inheritdoc cref="IFirewallService" />
public class FirewallService : ServiceBase, IFirewallService
{
    #region Constants

    private const int MaxRules = 128;

    #endregion

    #region Static Fields

    private static readonly Regex FirewallRegex = new(@"<script>[\s\S]+(firewall_enable = '(?<FirewallEnable>.*)';)[\s\S]+(ipv6_fw_rulelist_array = ""(?<IpV6RuleList>.*)"";)[\s\S]+(ipv4_fw_rulelist_array = ""(?<IpV4RuleList>.*)"";)", RegexOptions.Compiled);
    private static readonly Regex IpV6CidrRegex = new(@"^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*(\/(\d|\d\d|1[0-1]\d|12[0-8]))$");
    private static readonly Regex IpV6Regex = new(@"^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*");
    private static readonly Regex PortRegex = new(@"^([0-9]{1,5})\:([0-9]{1,5})$", RegexOptions.Compiled);
    private static readonly char[] InvalidFirewallChars = { '<', '>', '\'', '%' };

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
            var invalidRule = ruleList.FirstOrDefault(rule => !IsRuleValid(rule, out errorMessage));
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
            var invalidRule = ruleList.FirstOrDefault(rule => !IsRuleValid(rule, out errorMessage));
            if (invalidRule != null)
                throw new ArgumentException(errorMessage, nameof(ipv6FirewallRules));

            settings.Add("filter_wllist", string.Join(string.Empty, ruleList.Select(rule => rule.Serialize())));
        }

        await GetResponseAsync("/start_apply.htm", HttpMethod.Post, null, settings, true, cancellationToken)
            .ConfigureAwait(false);
    }

    private static bool IsRuleValid(FirewallRuleIpV6 rule, out string? message)
    {
        if (string.IsNullOrWhiteSpace(rule.LocalIp))
        {
            message = $"{nameof(rule.LocalIp)} is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(rule.PortRange))
        {
            message = $"{nameof(rule.PortRange)} is required.";
            return false;
        }

        if (rule.Protocol == IpV6Protocol.Unknown)
        {
            message = $"A valid {nameof(rule.Protocol)} is required.";
            return false;
        }

        if (HasInvalidChar(rule.ServiceName))
        {
            message = $"{nameof(rule.ServiceName)} cannot contain these characters '{string.Join(',', InvalidFirewallChars)}'.";
            return false;
        }

        if (HasInvalidChar(rule.RemoteIp))
        {
            message = $"{nameof(rule.RemoteIp)} cannot contain these characters '{string.Join(',', InvalidFirewallChars)}'.";
            return false;
        }

        if (HasInvalidChar(rule.LocalIp))
        {
            message = $"{nameof(rule.LocalIp)} cannot contain these characters '{string.Join(',', InvalidFirewallChars)}'.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(rule.RemoteIp) && !IpV6CidrRegex.IsMatch(rule.RemoteIp))
        {
            message = $"A valid {nameof(rule.RemoteIp)} is required.";
            return false;
        }

        if (!IpV6Regex.IsMatch(rule.LocalIp))
        {
            message = $"A valid {nameof(rule.LocalIp)} is required.";
            return false;
        }

        var port = ParsePort(rule.PortRange);
        if (!port.IsValid)
        {
            message = $"A valid {nameof(rule.PortRange)} is required.";
            return false;
        }

        const int minPort = 1;
        int maxPort = rule.Protocol == IpV6Protocol.Other ? 255 : 65535;

        if (!port.Start.IsInRange(minPort, maxPort) || (port.IsRange && !port.End.IsInRange(minPort, maxPort)))
        {
            message = $"The port must be between {minPort} and {maxPort}.";
            return false;
        }

        if (port.IsRange && port.End <= port.Start)
        {
            message = $"The end port ({port.End}) must greater than the start port ({port.Start}).";
            return false;
        }

        message = null;
        return true;
    }

    private static bool IsRuleValid(FirewallRuleIpV4 rule, out string? message)
    {
        if (string.IsNullOrWhiteSpace(rule.SourceIp))
        {
            message = $"{nameof(rule.SourceIp)} is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(rule.PortRange))
        {
            message = $"{nameof(rule.PortRange)} is required.";
            return false;
        }

        if (rule.Protocol == IpV4Protocol.Unknown)
        {
            message = $"A valid {nameof(rule.Protocol)} is required.";
            return false;
        }

        if (HasInvalidChar(rule.SourceIp))
        {
            message = $"{nameof(rule.SourceIp)} cannot contain these characters '{string.Join(',', InvalidFirewallChars)}'.";
            return false;
        }

        if (!IPAddress.TryParse(rule.SourceIp, out _))
        {
            message = $"A valid {nameof(rule.SourceIp)} is required.";
            return false;
        }

        var port = ParsePort(rule.PortRange);
        if (!port.IsValid)
        {
            message = $"A valid {nameof(rule.PortRange)} is required.";
            return false;
        }

        const int minPort = 1;
        const int maxPort = 65535;

        if (!port.Start.IsInRange(minPort, maxPort) || (port.IsRange && !port.End.IsInRange(minPort, maxPort)))
        {
            message = $"The port must be between {minPort} and {maxPort}.";
            return false;
        }

        if (port.IsRange && port.End <= port.Start)
        {
            message = $"The end port ({port.End}) must greater than the start port ({port.Start}).";
            return false;
        }

        message = null;
        return true;
    }

    private static bool HasInvalidChar(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var invalid = value.Any(InvalidFirewallChars.Contains);

        return invalid;
    }

    private static (bool IsValid, bool IsRange, int Start, int End) ParsePort(string? port)
    {
        if (string.IsNullOrWhiteSpace(port))
            return (false, false, 0, 0);

        if (int.TryParse(port, out var result))
            return (true, false, result, 0);

        var match = PortRegex.Match(port);

        if (!match.Success)
            return (false, false, 0, 0);

        if (!int.TryParse(match.Groups[1].Value, out var start))
            return (false, true, 0, 0);

        if (!int.TryParse(match.Groups[2].Value, out var end))
            return (false, true, 0, 0);

        return (true, true, start, end);
    }
}