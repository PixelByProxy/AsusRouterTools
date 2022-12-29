namespace PixelByProxy.Asus.Router.Models;

public class FirewallRuleIpV4
{
    public string? SourceIp { get; set; }

    public string? PortRange { get; set; }

    public string? Protocol { get; set; }

    internal string Serialize()
    {
        return $"<{Protocol}>>{SourceIp}>>>{PortRange}";
    }
}