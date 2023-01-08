namespace PixelByProxy.Asus.Router.Models;

public class FirewallSettings
{
    public bool Enabled { get; set; }

    public IList<FirewallRuleIpV6> IpV6FirewallRules { get; } = new List<FirewallRuleIpV6>();

    public IList<FirewallRuleIpV4> IpV4FirewallRules { get; } = new List<FirewallRuleIpV4>();
}