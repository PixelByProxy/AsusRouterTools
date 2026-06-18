
namespace PixelByProxy.Asus.Router.Configuration
{
    /// <summary>
    /// Connection settings used to communicate with the Asus router.
    /// </summary>
    public class AsusConfiguration
    {
        /// <summary>
        /// Gets or sets the host name or IP address of the router. Defaults to "router.asus.com".
        /// </summary>
        public string HostName { get; set; } = "router.asus.com";

        /// <summary>
        /// Gets or sets a value indicating whether to connect to the router over HTTPS instead of HTTP.
        /// </summary>
        public bool UseHttps { get; set; }

        /// <summary>
        /// Gets or sets the user name used to authenticate with the router.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the password used to authenticate with the router.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Enables access to the write methods. Write access to the router is experimental, use at your own risk.
        /// </summary>
        public bool EnableWrite { get; set; }
    }
}
