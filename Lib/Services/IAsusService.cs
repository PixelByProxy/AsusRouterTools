using PixelByProxy.Asus.Router.Configuration;
using PixelByProxy.Asus.Router.Models;

namespace PixelByProxy.Asus.Router.Services
{
    /// <summary>
    /// Service for working with the Asus router.
    /// </summary>
    public interface IAsusService
    {
        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The auth token from the credentials specified in the <see cref="AsusConfiguration"/>.</returns>
        Task<string> GetTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the uptime and time of last boot.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        Task<Uptime?> GetUptimeAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets memory usage statistics.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        Task<MemoryUsage?> GetMemoryUsageAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets CPU usage statistics.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        Task<CpuUsage?> GetCpuUsageAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets info for all of the connected clients.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        Task<List<Client>> GetClientsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current and total network usage.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The traffic object.</returns>
        Task<Traffic?> GetTrafficAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets WAN status information.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        Task<WanStatus?> GetWanStatusAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets various router settings.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        Task<RouterSettings?> GetRouterSettingsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets LAN status information.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        Task<LanStatus?> GetLanStatusAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a list of DHCP leases given out.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        Task<List<List<string>>> GetDhcpLeaseMacListAsync(CancellationToken cancellationToken = default);
    }
}
