namespace PixelByProxy.Asus.Router.Models;

/// <summary>
/// The transport protocol an IPv4 firewall rule applies to.
/// </summary>
public enum IpV4Protocol
{
    /// <summary>
    /// The protocol is unknown or unspecified.
    /// </summary>
    Unknown,

    /// <summary>
    /// The Transmission Control Protocol (TCP).
    /// </summary>
    Tcp,

    /// <summary>
    /// The User Datagram Protocol (UDP).
    /// </summary>
    Udp
}

/// <summary>
/// Represents a single IPv4 firewall rule.
/// </summary>
public class FirewallRuleIpV4
{
    /// <summary>
    /// Gets or sets the source IP address or CIDR range the rule applies to.
    /// </summary>
    public string? SourceIp { get; set; }

    /// <summary>
    /// Gets or sets the port or port range the rule applies to.
    /// </summary>
    public string? PortRange { get; set; }

    /// <summary>
    /// Gets or sets the transport protocol the rule applies to.
    /// </summary>
    public IpV4Protocol Protocol { get; set; }

    internal string Serialize()
    {
        return $"<{Protocol.ToString().ToUpperInvariant()}>>{SourceIp}>>>{PortRange}";
    }
}
