namespace PixelByProxy.Asus.Router.Models;

/// <summary>
/// Represents the router's firewall settings and rule lists.
/// </summary>
public class FirewallSettings
{
    /// <summary>
    /// Gets or sets a value indicating whether the firewall is enabled.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Gets the list of IPv6 firewall rules.
    /// </summary>
    public IList<FirewallRuleIpV6> IpV6FirewallRules { get; } = new List<FirewallRuleIpV6>();

    /// <summary>
    /// Gets the list of IPv4 firewall rules.
    /// </summary>
    public IList<FirewallRuleIpV4> IpV4FirewallRules { get; } = new List<FirewallRuleIpV4>();
}
