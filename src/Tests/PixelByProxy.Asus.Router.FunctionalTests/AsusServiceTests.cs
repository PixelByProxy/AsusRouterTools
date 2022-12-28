using FluentAssertions;
using PixelByProxy.Asus.Router.Services;

namespace PixelByProxy.Asus.Router.FunctionalTests;

[Collection(nameof(ConfigurationFixture))]
public class AsusServiceTests
{
    private readonly AsusService _asusService;

    public AsusServiceTests(ConfigurationFixture fixture)
    {
        _asusService = new AsusService(fixture.AsusConfiguration, fixture.HttpClient);
    }

    [Fact]
    public async Task GetUptimeAsync_ReturnsValuesThatAreNotNull()
    {
        // Act
        var uptime = await _asusService.GetUptimeAsync().ConfigureAwait(false);

        // Assert
        uptime.Should().NotBeNull();
        uptime!.Since.Should().NotBeNull();
        uptime.TotalSeconds.Should().NotBeNull();
    }
}