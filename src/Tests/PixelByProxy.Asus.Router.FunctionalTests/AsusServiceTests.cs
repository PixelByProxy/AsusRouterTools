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
    public async Task GeTokenAsync_ReturnsToken()
    {
        // Act
        var token = await _asusService.GetTokenAsync().ConfigureAwait(false);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();
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
    public async Task GetCpuUsageAsync_ReturnsPropertiesWithValues()
    {
        // Act
        var usage = await _asusService.GetCpuUsageAsync().ConfigureAwait(false);

        // Assert
        usage.Should().NotBeNull();
        usage!.Cpu1Total.Should().BeGreaterThan(0);
        usage.Cpu1Usage.Should().BeGreaterThan(0);
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
    public async Task GetWanStatusAsync_ReturnsPropertiesWithValues()
    {
        // Act
        var status = await _asusService.GetWanStatusAsync().ConfigureAwait(false);

        // Assert
        status.Should().NotBeNull();
        status!.Status.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetRouterSettingsAsync_ReturnsPropertiesWithValues()
    {
        // Act
        var settings = await _asusService.GetRouterSettingsAsync().ConfigureAwait(false);

        // Assert
        settings.Should().NotBeNull();
        settings!.LanIp.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetLanStatusAsync_ReturnsPropertiesWithValues()
    {
        // Act
        var status = await _asusService.GetLanStatusAsync().ConfigureAwait(false);

        // Assert
        status.Should().NotBeNull();
        status!.LanIp.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetDhcpLeaseMacListAsync_ReturnsNonEmptyList()
    {
        // Act
        var macList = await _asusService.GetDhcpLeaseMacListAsync().ConfigureAwait(false);

        // Assert
        macList.Should().NotBeNullOrEmpty();
        macList.First().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetWebHistoryAsync_ReturnsNonEmptyList()
    {
        // Act
        var history = await _asusService.GetWebHistory().ConfigureAwait(false);

        // Assert
        history.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetWebHistoryAsync_WithSpecificClient_OnlyReturnsHistoryForThatClient()
    {
        // Arrange
        var allHistory = await _asusService.GetWebHistory().ConfigureAwait(false);

        var macWithHistory = allHistory.First(h => !string.IsNullOrWhiteSpace(h.Mac)).Mac;

        // Act
        var history = await _asusService.GetWebHistory(macWithHistory).ConfigureAwait(false);

        // Assert
        history.Should().NotBeEmpty()
            .And.AllSatisfy(client =>
            {
                client.Mac.Should().Be(macWithHistory);
            });
    }
}