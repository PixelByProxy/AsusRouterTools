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
    public async Task GetUptimeAsync_ReturnsPropertiesWithValues()
    {
        // Act
        var uptime = await _asusService.GetUptimeAsync().ConfigureAwait(false);

        // Assert
        uptime.Should().NotBeNull();
        uptime!.Since.Should().NotBeNull();
        uptime.TotalSeconds.Should().NotBeNull();
    }

    [Fact]
    public async Task GetMemoryUsageAsync_ReturnsPropertiesWithValues()
    {
        // Act
        var usage = await _asusService.GetMemoryUsageAsync().ConfigureAwait(false);

        // Assert
        usage.Should().NotBeNull();
        usage!.Total.Should().BeGreaterThan(0);
        usage.Used.Should().BeGreaterThan(0);
        usage.Free.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetTrafficAsync_ReturnsPropertiesWithValues()
    {
        // Act
        var traffic = await _asusService.GetTrafficAsync().ConfigureAwait(false);

        // Assert
        traffic.Should().NotBeNull();
        traffic!.Sent.Should().BeGreaterThan(0);
        traffic.Received.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetClientsAsync_ReturnsListOfClientsWithValues()
    {
        // Act
        var clients = await _asusService.GetClientsAsync().ConfigureAwait(false);

        // Assert
        clients.Should().NotBeEmpty()
            .And.AllSatisfy(client =>
            {
                client.DisplayName.Should().NotBeNullOrWhiteSpace();
            });
    }
}