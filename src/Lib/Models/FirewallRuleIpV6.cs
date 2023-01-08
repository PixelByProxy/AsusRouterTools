namespace PixelByProxy.Asus.Router.Models;

public enum IpV6Protocol
{
    Unknown,
    Tcp,
    Udp,
    Both,
    Other
}

public class FirewallRuleIpV6
{
    public string? ServiceName { get; set; }

    public string? RemoteIp { get; set; }

    public string? LocalIp { get; set; }

    public string? PortRange { get; set; }

    public IpV6Protocol Protocol { get; set; }

    internal string Serialize()
    {
        return $"<{ServiceName}>{RemoteIp}>{LocalIp}>{PortRange}>{Protocol.ToString().ToUpperInvariant()}";
    }
}