namespace PixelByProxy.Asus.Router.Models;

/// <summary>
/// The transport protocol an IPv6 firewall rule applies to.
/// </summary>
public enum IpV6Protocol
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
    Udp,

    /// <summary>
    /// Both TCP and UDP.
    /// </summary>
    Both,

    /// <summary>
    /// A protocol other than TCP or UDP.
    /// </summary>
    Other
}

/// <summary>
/// Represents a single IPv6 firewall rule.
/// </summary>
public class FirewallRuleIpV6
{
    /// <summary>
    /// Gets or sets the name of the service the rule applies to.
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// Gets or sets the remote IP address the rule applies to.
    /// </summary>
    public string? RemoteIp { get; set; }

    /// <summary>
    /// Gets or sets the local IP address the rule applies to.
    /// </summary>
    public string? LocalIp { get; set; }

    /// <summary>
    /// Gets or sets the port or port range the rule applies to.
    /// </summary>
    public string? PortRange { get; set; }

    /// <summary>
    /// Gets or sets the transport protocol the rule applies to.
    /// </summary>
    public IpV6Protocol Protocol { get; set; }

    internal string Serialize()
    {
        return $"<{ServiceName}>{RemoteIp}>{LocalIp}>{PortRange}>{Protocol.ToString().ToUpperInvariant()}";
    }
}
