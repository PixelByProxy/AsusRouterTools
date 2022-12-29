using PixelByProxy.Asus.Router.Models;

namespace PixelByProxy.Asus.Router.Services;

/// <summary>
/// Service for working with the Asus router firewall.
/// </summary>
public interface IFirewallService
{
    /// <summary>
    /// Gets the router firewall settings.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<FirewallSettings> GetFirewallSettingsAsync(CancellationToken cancellationToken = default);
}