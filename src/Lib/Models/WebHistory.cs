namespace PixelByProxy.Asus.Router.Models;

/// <summary>
/// Represents a single web history entry recorded by the router.
/// </summary>
public class WebHistory
{
    /// <summary>
    /// Gets or sets the MAC address of the client that made the request.
    /// </summary>
    public string? Mac { get; set; }

    /// <summary>
    /// Gets or sets the date and time the site was accessed.
    /// </summary>
    public DateTime AccessTime { get; set; }

    /// <summary>
    /// Gets or sets the domain that was accessed.
    /// </summary>
    public string? Domain { get; set; }
}
