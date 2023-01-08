using FluentAssertions;
using PixelByProxy.Asus.Router.Extensions;
using PixelByProxy.Asus.Router.Messages;
using PixelByProxy.Asus.Router.Models;
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
            Protocol = IpV6Protocol.Tcp
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
    public async Task SetFirewallSettingsAsync_WithIpV6Rules_ThatExceedMaxCount_ThrowsArgumentException()
    {
        // Arrange
        const int maxRules = 128;

        var rules = new List<FirewallRuleIpV6>();

        for (int i = 0; i <= maxRules; i++)
        {
            rules.Add(new FirewallRuleIpV6());
        }

        // Act
        Func<Task> action = async () => await _firewallService.SetFirewallSettingsAsync(ipv6FirewallRules: rules);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false);
    }

    [Theory]
    [InlineData("LocalIp", null)]
    [InlineData("LocalIp", "")]
    [InlineData("LocalIp", "abc123")]
    [InlineData("RemoteIp", "abc123")]
    [InlineData("PortRange", null)]
    [InlineData("PortRange", "")]
    [InlineData("PortRange", "1:F")]
    [InlineData("Protocol", IpV6Protocol.Unknown)]
    public async Task SetFirewallSettingsAsync_WithIpV6Rules_ThatHaveInvalidProperties_ThrowsArgumentExceptionWithExpectedMessage(string propertyName, object propertyValue)
    {
        // Arrange
        var rule = new FirewallRuleIpV6
        {
            ServiceName = "FirewallFunctionalTests",
            RemoteIp = "2001::1111:2222:3333/64",
            LocalIp = "2001::1111:2222:3333/64",
            PortRange = "3333:3334",
            Protocol = IpV6Protocol.Tcp
        };

        var prop = rule.GetType().GetProperty(propertyName)!;
        prop.SetValue(rule, propertyValue);

        // Act
        Func<Task> action = async () => await _firewallService.SetFirewallSettingsAsync(ipv6FirewallRules: new List<FirewallRuleIpV6> { rule });

        // Assert
        (await action.Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false))
            .And.Message.Should().Be($"{ErrorStrings.ValidPropertyIsRequired(propertyName)} (Parameter 'ipv6FirewallRules')");
    }

    [Fact]
    public async Task SetFirewallSettingsAsync_WithIpV6Rules_WithServiceNameThatHasInvalidCharacters_ThrowsArgumentExceptionWithExpectedMessage()
    {
        // Arrange
        var rule = new FirewallRuleIpV6
        {
            ServiceName = "FirewallFunctionalTests",
            RemoteIp = "2001::1111:2222:3333/64",
            LocalIp = "2001::1111:2222:3333/64",
            PortRange = "3333:3334",
            Protocol = IpV6Protocol.Tcp
        };

        foreach (var invalidChar in FirewallRuleExtensions.InvalidFirewallChars)
        {
            // Act
            rule.ServiceName = invalidChar.ToString();

            Func<Task> action = async () => await _firewallService.SetFirewallSettingsAsync(ipv6FirewallRules: new List<FirewallRuleIpV6> { rule });

            // Assert
            (await action.Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false))
                .And.Message.Should().Be($"{ErrorStrings.PropertyCannotContainCharacters(nameof(rule.ServiceName), FirewallRuleExtensions.InvalidFirewallChars)} (Parameter 'ipv6FirewallRules')");
        }
    }

    [Fact]
    public async Task SetFirewallSettingsAsync_WithIpV6Rules_WithPortRangeThatWithEndGreaterThanStart_ThrowsArgumentExceptionWithExpectedMessage()
    {
        // Arrange
        const int startPort = 5000;
        const int endPort = 4999;

        var rule = new FirewallRuleIpV6
        {
            ServiceName = "FirewallFunctionalTests",
            RemoteIp = "2001::1111:2222:3333/64",
            LocalIp = "2001::1111:2222:3333/64",
            PortRange = $"{startPort}:{endPort}",
            Protocol = IpV6Protocol.Tcp
        };

        // Act
        Func<Task> action = async () => await _firewallService.SetFirewallSettingsAsync(ipv6FirewallRules: new List<FirewallRuleIpV6> { rule });

        // Assert
        (await action.Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false))
            .And.Message.Should().Be($"{ErrorStrings.EndPortMustBeGreater(startPort, endPort)} (Parameter 'ipv6FirewallRules')");
    }

    [Theory]
    [InlineData(IpV6Protocol.Other, "0")]
    [InlineData(IpV6Protocol.Other, "256")]
    [InlineData(IpV6Protocol.Other, "0:255")]
    [InlineData(IpV6Protocol.Other, "1:256")]
    [InlineData(IpV6Protocol.Tcp, "0")]
    [InlineData(IpV6Protocol.Tcp, "65536")]
    [InlineData(IpV6Protocol.Tcp, "0:255")]
    [InlineData(IpV6Protocol.Tcp, "1:65536")]
    public async Task SetFirewallSettingsAsync_WithIpV6Rules_WithPort_ThrowsArgumentExceptionIfPortIsOutOfRange(IpV6Protocol protocol, string portRange)
    {
        // Arrange
        const int minPort = 1;
        int maxPort = protocol == IpV6Protocol.Other ? 255 : 65535;

        var rule = new FirewallRuleIpV6
        {
            ServiceName = "FirewallFunctionalTests",
            RemoteIp = "2001::1111:2222:3333/64",
            LocalIp = "2001::1111:2222:3333/64",
            PortRange = portRange,
            Protocol = protocol
        };

        // Act
        Func<Task> action = async () => await _firewallService.SetFirewallSettingsAsync(ipv6FirewallRules: new List<FirewallRuleIpV6> { rule });

        // Assert
        (await action.Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false))
            .And.Message.Should().Be($"{ErrorStrings.PortBetween(minPort, maxPort)} (Parameter 'ipv6FirewallRules')");
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
            Protocol = IpV4Protocol.Udp
        };

        currentSettings.IpV4FirewallRules.Add(newRule);

        // Act
        await _firewallService.SetFirewallSettingsAsync(ipv4FirewallRules: currentSettings.IpV4FirewallRules)
            .ConfigureAwait(false);

        var newSettings = await _firewallService.GetFirewallSettingsAsync().ConfigureAwait(false);

        // Assert
        newSettings.IpV4FirewallRules.Should().ContainEquivalentOf(newRule);
    }

    [Fact]
    public async Task SetFirewallSettingsAsync_WithIpV4Rules_ThatExceedMaxCount_ThrowsArgumentException()
    {
        // Arrange
        const int maxRules = 128;

        var rules = new List<FirewallRuleIpV4>();

        for (int i = 0; i <= maxRules; i++)
        {
            rules.Add(new FirewallRuleIpV4());
        }

        // Act
        Func<Task> action = async () => await _firewallService.SetFirewallSettingsAsync(ipv4FirewallRules: rules);

        // Assert
        await action.Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false);
    }

    [Theory]
    [InlineData("SourceIp", null)]
    [InlineData("SourceIp", "")]
    [InlineData("SourceIp", "abc123")]
    [InlineData("PortRange", null)]
    [InlineData("PortRange", "")]
    [InlineData("PortRange", "1:F")]
    [InlineData("Protocol", IpV4Protocol.Unknown)]
    public async Task SetFirewallSettingsAsync_WithIpV4Rules_ThatHaveInvalidProperties_ThrowsArgumentExceptionWithExpectedMessage(string propertyName, object propertyValue)
    {
        // Arrange
        var rule = new FirewallRuleIpV4
        {
            SourceIp = "192.168.255.255",
            PortRange = "3333:3334",
            Protocol = IpV4Protocol.Tcp
        };

        var prop = rule.GetType().GetProperty(propertyName)!;
        prop.SetValue(rule, propertyValue);

        // Act
        Func<Task> action = async () => await _firewallService.SetFirewallSettingsAsync(ipv4FirewallRules: new List<FirewallRuleIpV4> { rule });

        // Assert
        (await action.Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false))
            .And.Message.Should().Be($"{ErrorStrings.ValidPropertyIsRequired(propertyName)} (Parameter 'ipv4FirewallRules')");
    }

    [Fact]
    public async Task SetFirewallSettingsAsync_WithIpV4Rules_WithPortRangeThatWithEndGreaterThanStart_ThrowsArgumentExceptionWithExpectedMessage()
    {
        // Arrange
        const int startPort = 5000;
        const int endPort = 4999;

        var rule = new FirewallRuleIpV4
        {
            SourceIp = "192.168.255.255",
            PortRange = $"{startPort}:{endPort}",
            Protocol = IpV4Protocol.Tcp
        };

        // Act
        Func<Task> action = async () => await _firewallService.SetFirewallSettingsAsync(ipv4FirewallRules: new List<FirewallRuleIpV4> { rule });

        // Assert
        (await action.Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false))
            .And.Message.Should().Be($"{ErrorStrings.EndPortMustBeGreater(startPort, endPort)} (Parameter 'ipv4FirewallRules')");
    }

    [Theory]
    [InlineData("0")]
    [InlineData("65536")]
    [InlineData("0:255")]
    [InlineData("1:65536")]
    public async Task SetFirewallSettingsAsync_WithIpV4Rules_WithPort_ThrowsArgumentExceptionIfPortIsOutOfRange(string portRange)
    {
        // Arrange
        const int minPort = 1;
        const int maxPort = 65535;

        var rule = new FirewallRuleIpV4
        {
            SourceIp = "192.168.255.255",
            PortRange = portRange,
            Protocol = IpV4Protocol.Udp
        };

        // Act
        Func<Task> action = async () => await _firewallService.SetFirewallSettingsAsync(ipv4FirewallRules: new List<FirewallRuleIpV4> { rule });

        // Assert
        (await action.Should().ThrowExactlyAsync<ArgumentException>().ConfigureAwait(false))
            .And.Message.Should().Be($"{ErrorStrings.PortBetween(minPort, maxPort)} (Parameter 'ipv4FirewallRules')");
    }
}