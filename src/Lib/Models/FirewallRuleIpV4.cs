namespace PixelByProxy.Asus.Router.Models;

public enum IpV4Protocol
{
    Unknown,
    Tcp,
    Udp
}

public class FirewallRuleIpV4
{
    public string? SourceIp { get; set; }

    public string? PortRange { get; set; }

    public IpV4Protocol Protocol { get; set; }

    internal string Serialize()
    {
        return $"<{Protocol.ToString().ToUpperInvariant()}>>{SourceIp}>>>{PortRange}";
    }
}