using PixelByProxy.Asus.Router.Models;
using System.Net;
using System.Text.RegularExpressions;
using PixelByProxy.Asus.Router.Messages;

namespace PixelByProxy.Asus.Router.Extensions;

internal static class FirewallRuleExtensions
{
    internal static readonly char[] InvalidFirewallChars = { '<', '>', '\'', '%' };

    private static readonly Regex IpV6CidrRegex = new(@"^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*(\/(\d|\d\d|1[0-1]\d|12[0-8]))$");
    private static readonly Regex IpV6Regex = new(@"^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*");
    private static readonly Regex PortRegex = new(@"^([0-9]{1,5})\:([0-9]{1,5})$", RegexOptions.Compiled);

    public static bool IsRuleValid(this FirewallRuleIpV6 rule, out string? message)
    {
        if (string.IsNullOrWhiteSpace(rule.LocalIp) || !IpV6Regex.IsMatch(rule.LocalIp))
        {
            message = ErrorStrings.ValidPropertyIsRequired(nameof(rule.LocalIp));
            return false;
        }

        if (string.IsNullOrWhiteSpace(rule.PortRange))
        {
            message = ErrorStrings.ValidPropertyIsRequired(nameof(rule.PortRange));
            return false;
        }

        if (rule.Protocol == IpV6Protocol.Unknown)
        {
            message = ErrorStrings.ValidPropertyIsRequired(nameof(rule.Protocol));
            return false;
        }

        if (!string.IsNullOrWhiteSpace(rule.RemoteIp) && !IpV6CidrRegex.IsMatch(rule.RemoteIp))
        {
            message = ErrorStrings.ValidPropertyIsRequired(nameof(rule.RemoteIp));
            return false;
        }

        if (HasInvalidChar(rule.ServiceName))
        {
            message = ErrorStrings.PropertyCannotContainCharacters(nameof(rule.ServiceName), InvalidFirewallChars);
            return false;
        }

        var port = ParsePort(rule.PortRange);
        if (!port.IsValid)
        {
            message = ErrorStrings.ValidPropertyIsRequired(nameof(rule.PortRange));
            return false;
        }

        const int minPort = 1;
        int maxPort = rule.Protocol == IpV6Protocol.Other ? 255 : 65535;

        if (!port.Start.IsInRange(minPort, maxPort) || (port.IsRange && !port.End.IsInRange(minPort, maxPort)))
        {
            message = ErrorStrings.PortBetween(minPort, maxPort);
            return false;
        }

        if (port.IsRange && port.End <= port.Start)
        {
            message = ErrorStrings.EndPortMustBeGreater(port.Start, port.End);
            return false;
        }

        message = null;
        return true;
    }

    public static bool IsRuleValid(this FirewallRuleIpV4 rule, out string? message)
    {
        if (string.IsNullOrWhiteSpace(rule.SourceIp) || !IPAddress.TryParse(rule.SourceIp, out _))
        {
            message = ErrorStrings.ValidPropertyIsRequired(nameof(rule.SourceIp));
            return false;
        }

        if (string.IsNullOrWhiteSpace(rule.PortRange))
        {
            message = ErrorStrings.ValidPropertyIsRequired(nameof(rule.PortRange));
            return false;
        }

        if (rule.Protocol == IpV4Protocol.Unknown)
        {
            message = ErrorStrings.ValidPropertyIsRequired(nameof(rule.Protocol));
            return false;
        }

        var port = ParsePort(rule.PortRange);
        if (!port.IsValid)
        {
            message = ErrorStrings.ValidPropertyIsRequired(nameof(rule.PortRange));
            return false;
        }

        const int minPort = 1;
        const int maxPort = 65535;

        if (!port.Start.IsInRange(minPort, maxPort) || (port.IsRange && !port.End.IsInRange(minPort, maxPort)))
        {
            message = ErrorStrings.PortBetween(minPort, maxPort);
            return false;
        }

        if (port.IsRange && port.End <= port.Start)
        {
            message = ErrorStrings.EndPortMustBeGreater(port.Start, port.End);
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