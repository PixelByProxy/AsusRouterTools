using FluentAssertions;
using PixelByProxy.Asus.Router.Services;

namespace PixelByProxy.Asus.Router.FunctionalTests;

[Collection(nameof(ConfigurationFixture))]
public class FirewallServiceTests
{
    private readonly FirewallService _firewallService;

    public FirewallServiceTests(ConfigurationFixture fixture)
    {
        _firewallService = new FirewallService(fixture.AsusConfiguration, fixture.HttpClient);
    }

    [Fact]
    public async Task GetFirewallSettingsAsyncAsync_ReturnsPropertiesWithValues()
    {
        // Act
        var settings = await _firewallService.GetFirewallSettingsAsync().ConfigureAwait(false);

        // Assert
        settings.Should().NotBeNull();
    }
}