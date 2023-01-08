using Microsoft.Extensions.Configuration;
using PixelByProxy.Asus.Router.Configuration;
using PixelByProxy.Asus.Router.Models;
using PixelByProxy.Asus.Router.Services;

namespace PixelByProxy.Asus.Router.FunctionalTests.Configuration;

public class ConfigurationFixture : IAsyncLifetime
{
    private readonly FirewallService _firewallService;
    private FirewallSettings? _initialFirewallSettings;

    public ConfigurationFixture()
    {
        HttpClient = new HttpClient();

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<AsusConfigurationSecrets>(false)
            .Build();

        AsusConfiguration = configuration.GetSection(nameof(AsusConfiguration)).Get<AsusConfiguration>()!;
        AsusConfiguration.EnableWrite = true;

        _firewallService = new FirewallService(AsusConfiguration, HttpClient);
    }

    public HttpClient HttpClient { get; }

    public AsusConfiguration AsusConfiguration { get; }


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