namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Jobs;

/// <summary>
/// Job metadata and scheduling configuration.
/// Defines WHAT to run, WHEN, and HOW.
/// </summary>
public class JobDefinitionRow : BaseTenantEntity
{
    /// <summary>
    /// Human-readable job name: 'CleanupExpiredTokens', 'GenerateMonthlyReport'
    /// </summary>
    public required string JobName { get; set; }
    
    /// <summary>
    /// Unique identifier for idempotency: 'cleanup-tokens', 'monthly-report'
    /// </summary>
    public required string JobKey { get; set; }
    
    /// <summary>
    /// Fully qualified type name
    /// </summary>
    public required string JobType { get; set; }
    
    /// <summary>
    /// Default/template parameters for job execution (JSON)
    /// </summary>
    public string? JobParameters { get; set; }
    
    /// <summary>
    /// Schedule type: 'Once', 'Recurring', 'Cron', 'Delayed', 'Manual'
    /// </summary>
    public required string ScheduleType { get; set; }
    
    /// <summary>
    /// Cron expression for recurring jobs: '0 0 * * *', '*/5 * * * *'
    /// </summary>
    public string? CronExpression { get; set; }
    
    /// <summary>
    /// Alternative to cron: run every N seconds
    /// </summary>
    public int? IntervalSeconds { get; set; }
    
    /// <summary>
    /// Priority (0-100). Higher = runs first.
    /// </summary>
    public int Priority { get; set; } = 0;
    
    /// <summary>
    /// Execution scope: 'Cluster' (run once across farm) or 'Instance' (run on every server)
    /// </summary>
    public string ExecutionScope { get; set; } = "Cluster";
    
    /// <summary>
    /// Maximum number of retries on failure
    /// </summary>
    public int MaxRetries { get; set; } = 3;
    
    /// <summary>
    /// Job timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 3600;
    
    /// <summary>
    /// Max concurrent executions: 0 = unlimited, 1 = sequential only
    /// </summary>
    public int MaxConcurrentExecutions { get; set; } = 1;
    
    /// <summary>
    /// Whether job is enabled (can be disabled without deleting)
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    
    /// <summary>
    /// Foreign key to most recent execution
    /// </summary>
    public Guid? LastExecutionId { get; set; }
    
    /// <summary>
    /// When next execution should occur
    /// </summary>
    public DateTime? NextRunAt { get; set; }
    
    // Navigation properties
    public JobExecutionRow? LastExecution { get; set; }
    public ICollection<JobExecutionRow> Executions { get; set; } = new List<JobExecutionRow>();
}
