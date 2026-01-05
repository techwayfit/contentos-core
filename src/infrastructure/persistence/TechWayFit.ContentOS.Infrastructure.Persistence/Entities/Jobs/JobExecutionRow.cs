namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Jobs;

/// <summary>
/// Individual job execution instances with distributed lock support.
/// One row per execution attempt.
/// </summary>
public class JobExecutionRow : BaseTenantEntity
{
    /// <summary>
    /// Foreign key to job definition
    /// </summary>
    public required Guid JobDefinitionId { get; set; }
    
    /// <summary>
    /// Execution attempt number (1st, 2nd, 3rd)
    /// </summary>
    public int ExecutionNumber { get; set; } = 1;
    
    /// <summary>
    /// When this execution was supposed to run
    /// </summary>
    public required DateTime ScheduledAt { get; set; }
    
    /// <summary>
    /// When it was added to queue
    /// </summary>
    public required DateTime EnqueuedAt { get; set; }
    
    /// <summary>
    /// Execution status: Pending, Claimed, Running, Completed, Failed, Cancelled, TimedOut
    /// </summary>
    public required string Status { get; set; } = "Pending";
    
    /// <summary>
    /// Worker instance ID that claimed this job: 'server-a-pod-123', 'hostname-pid-guid'
    /// </summary>
    public string? ClaimedBy { get; set; }
    
    /// <summary>
    /// When worker claimed this job
    /// </summary>
    public DateTime? ClaimedAt { get; set; }
    
    /// <summary>
    /// Heartbeat timeout - if NOW() > this, job is orphaned
    /// </summary>
    public DateTime? LockExpiresAt { get; set; }
    
    /// <summary>
    /// Last heartbeat from worker (updated every 30s)
    /// </summary>
    public DateTime? HeartbeatAt { get; set; }
    
    /// <summary>
    /// When job execution started
    /// </summary>
    public DateTime? StartedAt { get; set; }
    
    /// <summary>
    /// When job execution completed
    /// </summary>
    public DateTime? CompletedAt { get; set; }
    
    /// <summary>
    /// Execution duration in milliseconds
    /// </summary>
    public long? DurationMs { get; set; }
    
    /// <summary>
    /// Success data, progress updates (JSON)
    /// </summary>
    public string? ResultData { get; set; }
    
    /// <summary>
    /// Error message if failed
    /// </summary>
    public string? ErrorMessage { get; set; }
    
    /// <summary>
    /// Exception type for retry logic
    /// </summary>
    public string? ErrorType { get; set; }
    
    /// <summary>
    /// Full stack trace
    /// </summary>
    public string? StackTrace { get; set; }
    
    /// <summary>
    /// True if this execution is a retry of another
    /// </summary>
    public bool IsRetry { get; set; } = false;
    
    /// <summary>
    /// Foreign key to original execution if this is a retry
    /// </summary>
    public Guid? RetryOfExecutionId { get; set; }
    
    /// <summary>
    /// Number of retries attempted
    /// </summary>
    public int RetryCount { get; set; } = 0;
    
    /// <summary>
    /// Whether this execution can be retried
    /// </summary>
    public bool CanRetry { get; set; } = true;
    
    // Navigation properties
    public JobDefinitionRow? JobDefinition { get; set; }
    public JobExecutionRow? RetryOfExecution { get; set; }
    public ICollection<JobExecutionRow> Retries { get; set; } = new List<JobExecutionRow>();
}
