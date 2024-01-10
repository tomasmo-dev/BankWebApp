using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BankWebApp.Services;

/// <summary>
/// Class DiskHealthService.
/// Implements the <see cref="Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck" /> interface.
/// Used to check if the disk has enough free space.
/// </summary>
public class DiskHealthService : IHealthCheck
{
    /// <summary>
    /// The minimum free space in bytes (1 GB)
    /// For normal operation, the disk should have at least this much free space.
    /// </summary>
    private const long MinimumFreeSpace = 1000000000; // 1 GB
    
    /// <summary>
    /// The critical free space in bytes (500 MB)
    /// The application will continue to function normally, but there may be some performance degradation.
    /// And / Or the application may crash if the disk runs out of space and loss of data may occur.
    /// </summary>
    private const long CriticalFreeSpace = 500000000; // 500 MB
    
    
    /// <summary>
    /// Checks the remaining space on the disk asynchronously.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task representing the asynchronous operation. The Task result contains the HealthCheckResult.</returns>
    /// <exception cref="Exception">Thrown when failed to check disk health.</exception>

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        try
        {
            var driveInfo = new DriveInfo(Path.GetPathRoot(Directory.GetCurrentDirectory()));
            var free = driveInfo.AvailableFreeSpace;

            if (free < MinimumFreeSpace)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"Not enough free disk space. (Minimum: {MinimumFreeSpace} bytes, Actual: {free} bytes)"));
            }
            else if (free < CriticalFreeSpace)
            {
                return Task.FromResult(HealthCheckResult.Degraded($"Low disk space. (Minimum: {MinimumFreeSpace} bytes, Actual: {free} bytes)"));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Healthy($"Disk has enough free space. (Minimum: {MinimumFreeSpace} bytes, Actual: {free} bytes)"));
            }

        }
        catch (Exception e)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Failed to check disk health. (Exception)", e));
        }
    }
}