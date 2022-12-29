using FluentAssertions;
using PixelByProxy.Asus.Router.Models;
using PixelByProxy.Asus.Router.Services;

namespace PixelByProxy.Asus.Router.FunctionalTests;

[Collection(nameof(ConfigurationFixture))]
public class FirewallServiceTests : IAsyncLifetime
{
    private readonly FirewallService _firewallService;
    private FirewallSettings? _initialFirewallSettings;

    public FirewallServiceTests(ConfigurationFixture fixture)
    {
        _firewallService = new FirewallService(fixture.AsusConfiguration, fixture.HttpClient);
    }

    [Fact]
    public async Task GetFirewallSettingsAsync_ReturnsPropertiesWithValues()
    {
        // Act
        var settings = await _firewallService.GetFirewallSettingsAsync().ConfigureAwait(false);

        // Assert
        settings.Should().NotBeNull();
    }

    [Fact]
    public async Task SetFirewallSettingsAsync_WithEnableDisableFirewall_UpdatesFirewallEnabledDisabledState()
    {
        // Arrange
        var currentSettings = await _firewallService.GetFirewallSettingsAsync().ConfigureAwait(false);
        var expectedEnabled = !currentSettings.Enabled;

        // Act
        await _firewallService.SetFirewallSettingsAsync(enabled: expectedEnabled)
            .ConfigureAwait(false);

        var newSettings = await _firewallService.GetFirewallSettingsAsync().ConfigureAwait(false);

        // Assert
        newSettings.Enabled.Should().Be(expectedEnabled);
    }

    [Fact]
    public async Task SetFirewallSettingsAsync_WithIpV6Rules_UpdatesIpV6FirewallRules()
    {
        // Arrange
        var currentSettings = await _firewallService.GetFirewallSettingsAsync().ConfigureAwait(false);

        var newRule = new FirewallRuleIpV6
        {
            ServiceName = "FirewallFunctionalTests",
            RemoteIp = "2001::1111:2222:3333/64",
            LocalIp = "2001::1111:2222:3333/64",
            PortRange = "3333:3334",
            Protocol = "TCP"
        };

        currentSettings.IpV6FirewallRules.Add(newRule);

        // Act
        await _firewallService.SetFirewallSettingsAsync(ipv6FirewallRules: currentSettings.IpV6FirewallRules)
            .ConfigureAwait(false);

        var newSettings = await _firewallService.GetFirewallSettingsAsync().ConfigureAwait(false);

        // Assert
        newSettings.IpV6FirewallRules.Should().ContainEquivalentOf(newRule);
    }

    [Fact]
    public async Task SetFirewallSettingsAsync_WithIpV4Rules_UpdatesIpV4FirewallRules()
    {
        // Arrange
        var currentSettings = await _firewallService.GetFirewallSettingsAsync().ConfigureAwait(false);

        var newRule = new FirewallRuleIpV4
        {
            SourceIp = "255.255.255.255",
            PortRange = "3333:3334",
            Protocol = "TCP"
        };

        currentSettings.IpV4FirewallRules.Add(newRule);

        // Act
        await _firewallService.SetFirewallSettingsAsync(ipv4FirewallRules: currentSettings.IpV4FirewallRules)
            .ConfigureAwait(false);

        var newSettings = await _firewallService.GetFirewallSettingsAsync().ConfigureAwait(false);

        // Assert
        newSettings.IpV4FirewallRules.Should().ContainEquivalentOf(newRule);
    }

    public async Task InitializeAsync()
    {
        _initialFirewallSettings = await _firewallService.GetFirewallSettingsAsync().ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        if (_initialFirewallSettings != null)
            await _firewallService.SetFirewallSettingsAsync(_initialFirewallSettings.Enabled, _initialFirewallSettings.IpV6FirewallRules, _initialFirewallSettings.IpV4FirewallRules).ConfigureAwait(false);
    }
}