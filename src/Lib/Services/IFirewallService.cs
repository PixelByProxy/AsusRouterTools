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
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns></returns>
    Task<FirewallSettings> GetFirewallSettingsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the router firewall settings.
    /// </summary>
    /// <param name="enabled">If set will enable or disable the firewall.</param>
    /// <param name="ipv6FirewallRules">If set will update the ipv6 firewall rules, overwriting any previous values.</param>
    /// <param name="ipv4FirewallRules">If set will update the ipv4 firewall rules, overwriting any previous values.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    Task SetFirewallSettingsAsync(bool? enabled = null, IEnumerable<FirewallRuleIpV6>? ipv6FirewallRules = null, IEnumerable<FirewallRuleIpV4>? ipv4FirewallRules = null, CancellationToken cancellationToken = default);
}