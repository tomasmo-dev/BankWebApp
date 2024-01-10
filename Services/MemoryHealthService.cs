using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BankWebApp.Services;

/// <summary>
/// Class MemoryHealthService.
/// Implements the <see cref="Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck" /> interface.
/// Used to check if the RAM has enough free space.
/// </summary>

public class MemoryHealthService : IHealthCheck
{
    /// <summary>
    /// The minimum memory for normal operation in bytes (5 MB)
    /// </summary>
    private const long MinimumFreeMemory = 5_000_000; // 5 MB
    
    /// <summary>
    /// The critical memory for normal operation in bytes (1 MB)
    /// When running on less than (5 MB) of memory, the application will continue to function normally,
    /// but there may be some performance degradation
    /// and / or the application may crash if the memory runs out of space and loss of data will occur.
    private const long CriticalFreeMemory = 1_000_000; // 1 MB
    
    /// <summary>
    /// Checks the remaining space in the memory asynchronously.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task representing the asynchronous operation. The Task result contains the HealthCheckResult.</returns>
    /// <exception cref="Exception">Thrown when failed to check memory health.</exception>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
        try
        {
            var free = GC.GetTotalMemory(false);

            if (free < MinimumFreeMemory)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"Not enough free memory. (Minimum: {MinimumFreeMemory} bytes, Actual: {free} bytes)"));
            }
            else if (free < CriticalFreeMemory)
            {
                return Task.FromResult(HealthCheckResult.Degraded($"Low memory. (Minimum: {MinimumFreeMemory} bytes, Actual: {free} bytes)"));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Healthy($"Memory has enough free space. (Minimum: {MinimumFreeMemory} bytes, Actual: {free} bytes)"));
            }

        }
        catch (Exception e)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Failed to check memory health. (Exception)", e));
        }
    }
}