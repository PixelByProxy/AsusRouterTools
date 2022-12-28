using Microsoft.Extensions.Configuration;
using PixelByProxy.Asus.Router.Configuration;

namespace PixelByProxy.Asus.Router.FunctionalTests.Configuration;

public class ConfigurationFixture
{
    public ConfigurationFixture()
    {
        HttpClient = new HttpClient();

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<AsusConfigurationSecrets>(false)
            .Build();

        AsusConfiguration = configuration.GetSection(nameof(Router.Configuration.AsusConfiguration)).Get<AsusConfiguration>()!;
    }

    public HttpClient HttpClient { get; }

    public AsusConfiguration AsusConfiguration { get; }
}