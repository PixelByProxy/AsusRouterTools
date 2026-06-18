namespace PixelByProxy.Asus.Router.Models
{
    /// <summary>
    /// Represents how long the router has been running since its last boot.
    /// </summary>
    public class Uptime
    {
        /// <summary>
        /// Gets or sets the date and time the router was last booted.
        /// </summary>
        public DateTime? Since { get; set; }

        /// <summary>
        /// Gets or sets the total number of seconds the router has been running since the last boot.
        /// </summary>
        public string? TotalSeconds { get; set; }
    }
}
