namespace PixelByProxy.Asus.Router.Models
{
    /// <summary>
    /// Represents network traffic counters for the router.
    /// </summary>
    public class Traffic
    {
        /// <summary>
        /// Gets or sets the amount of data sent.
        /// </summary>
        public long Sent { get; set; }

        /// <summary>
        /// Gets or sets the amount of data received.
        /// </summary>
        public long Received { get; set; }
    }
}
