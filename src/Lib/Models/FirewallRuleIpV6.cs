namespace PixelByProxy.Asus.Router.Models;

public class FirewallRuleIpV6
{
    public string? ServiceName { get; set; }

    public string? RemoteIp { get; set; }

    public string? LocalIp { get; set; }

    public string? PortRange { get; set; }

    public string? Protocol { get; set; }

    internal string Serialize()
    {
        return $"<{ServiceName}>{RemoteIp}>{LocalIp}>{PortRange}>{Protocol}";
    }
}