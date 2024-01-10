using System.Text.Json;
using BankWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace BankWebApp.Controllers;

/// <summary>
/// Controller for handling health checks of the application.
/// </summary>
[Route("api/[controller]/[action]")]
public class HealthCheckController : ControllerBase
{
    private readonly ILogger<HealthCheckController> _logger;
    
    // Services used to check the health of the database, disk, and memory.
    private readonly DatabaseHealthService _dbService;
    private readonly DiskHealthService _diskService;
    private readonly MemoryHealthService _memoryService;
    
    
    /// <summary>
    /// Initializes a new instance of the <see cref="HealthCheckController"/> class.
    /// </summary>
    /// <param name="logger">The logger used to log information about program execution.</param>
    /// <param name="dbService">The service used to check the health of the database.</param>
    /// <param name="diskService">The service used to check the health of the disk.</param>
    /// <param name="memoryService">The service used to check the health of the memory.</param>
    public HealthCheckController(ILogger<HealthCheckController> logger, 
        DatabaseHealthService dbService,
        DiskHealthService diskService,
        MemoryHealthService memoryService)
    {
        _logger = logger;
        _dbService = dbService;
        _diskService = diskService;
        _memoryService = memoryService;
    }
    
    
    /// <summary>
    /// Gets the health status of all components.
    /// If any component is unhealthy, the overall status will be unhealthy.
    /// If any component is degraded, the overall status will be degraded.
    /// </summary>
    /// <returns>The health status of all components as a JSON object.</returns>
    [HttpGet]
    public async Task<IActionResult> All()
    {
        var DatabaseHealth = await CheckDatabaseHealth();
        var DiskHealth = await CheckDiskHealth();
        var RAMHealth = await CheckRAMHealth();
        
        var healthArray = new HealthCheckResult[]
        {
            DatabaseHealth.Item2,
            DiskHealth.Item2,
            RAMHealth.Item2
        };

        #region OverallHealthCheck
        HealthStatus overallHealth = HealthStatus.Healthy;

        foreach (var healthValue in healthArray)
        {
            if (healthValue.Status == HealthStatus.Unhealthy)
            {
                overallHealth = HealthStatus.Unhealthy;
                break;
            }
            else if (healthValue.Status == HealthStatus.Degraded)
            {
                overallHealth = HealthStatus.Degraded;
            }
        }
        #endregion

        var obj = new
        {
            OverallStatusInt = overallHealth,
            OverallStatus = overallHealth.ToString(),
            
            StatusEnum = new {
                HealthStatus.Healthy,
                HealthStatus.Degraded,
                HealthStatus.Unhealthy
            },

            DatabaseHealth = DatabaseHealth.Item2,
            DiskHealth = DiskHealth.Item2,
            RAMHealth = RAMHealth.Item2
        };
        
        var json = JsonSerializer.Serialize(obj);
     
        return overallHealth switch
        {
            HealthStatus.Healthy => Ok(json),
            HealthStatus.Degraded => BadRequest(json),
            HealthStatus.Unhealthy => BadRequest(json)
        };
    }

    /// <summary>
    /// Gets the health status of the database.
    /// </summary>
    /// <returns>The health status of the database as a JSON object.</returns>
    [HttpGet]
    public async Task<IActionResult> Database()
    {
        return (await CheckDatabaseHealth()).Item1;
    }
    
    /// <summary>
    /// Gets the health status of the disk.
    /// </summary>
    /// <returns>The health status of the disk as a JSON object.</returns>
    [HttpGet]
    public async Task<IActionResult> Disk()
    {
        return (await CheckDiskHealth()).Item1;
    }

    /// <summary>
    /// Gets the health status of the RAM.
    /// </summary>
    /// <returns>The health status of the RAM as a JSON object.</returns>
    [HttpGet]
    public async Task<IActionResult> RAM()
    {
        return (await CheckRAMHealth()).Item1;
    }

    /// <summary>
    /// Checks the health of the database.
    /// </summary>
    /// <returns>The health status and report of the database as a JSON object.</returns>
    private async Task<(IActionResult, HealthCheckResult)> CheckDatabaseHealth()
    {
        var report = await _dbService.CheckHealthAsync(new (), new ());
        string json = JsonSerializer.Serialize(report);

        return report.Status switch
        {
            HealthStatus.Healthy => (Ok(json), report),
            HealthStatus.Degraded => (BadRequest(json), report),
            HealthStatus.Unhealthy => (BadRequest(json), report)
        };
    }

    /// <summary>
    /// Checks the if the capacity of the disk is sufficient.
    /// </summary>
    /// <returns>The health status and report of the disk as a JSON object.</returns>
    private async Task<(IActionResult, HealthCheckResult)> CheckDiskHealth()
    {
        var report = await _diskService.CheckHealthAsync(new (), new ());
        string json = JsonSerializer.Serialize(report);

        return report.Status switch
        {
            HealthStatus.Healthy => (Ok(json), report),
            HealthStatus.Degraded => (BadRequest(json), report),
            HealthStatus.Unhealthy => (BadRequest(json), report)
        
        };
    }

    /// <summary>
    /// Checks if the application has enough free RAM.
    /// </summary>
    /// <returns>The health status and report of the RAM as a JSON object.</returns>
    private async Task<(IActionResult, HealthCheckResult)> CheckRAMHealth()
    {
        var report = await _memoryService.CheckHealthAsync(new (), new ());
        string json = JsonSerializer.Serialize(report);
        
        return report.Status switch
        {
            HealthStatus.Healthy => (Ok(json), report),
            HealthStatus.Degraded => (BadRequest(json), report),
            HealthStatus.Unhealthy => (BadRequest(json), report)
        };
    }
}