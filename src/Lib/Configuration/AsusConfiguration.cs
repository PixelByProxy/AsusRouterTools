
namespace PixelByProxy.Asus.Router.Configuration
{
    public class AsusConfiguration
    {
        public string HostName { get; set; } = "router.asus.com";

        public bool UseHttps { get; set; }

        public string? UserName { get; set; }

        public string? Password { get; set; }

        /// <summary>
        /// Enables access to the write methods. Write access to the router is experimental, use at your own risk.
        /// </summary>
        public bool EnableWrite { get; set; }
    }
}
