using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BankWebApp.Services;

/// <summary>
/// Service for checking the health of the database.
/// Implements the <see cref="Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck"/> interface.
/// </summary>
public class DatabaseHealthService: IHealthCheck
{
    /// <summary>
    /// Asynchronously checks the health of the database.
    /// </summary>
    /// <param name="context">The context under which the health check is being performed.</param>
    /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken"/> that can be used to cancel the health check.</param>
    /// <returns>A <see cref="System.Threading.Tasks.Task"/> that represents the asynchronous operation, containing the <see cref="Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult"/> of the database health check.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        
        try
        {
            bool alive;
            using (var db = new DatabaseService())
            {
                alive = db.Ping();
            }
            
            if (!alive)
            {
                return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, "Failed to connect to database."));
            }
            
            return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy, "Database connection is healthy."));
        }
        catch (Exception e)
        {
            return Task.FromResult(new HealthCheckResult(HealthStatus.Unhealthy, "Failed to connect to database. (Exception)", e));
        }
    }
}