namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Jobs;

/// <summary>
/// Long-term audit trail for completed/failed executions.
/// Archived from JOB_EXECUTION to keep that table lean.
/// </summary>
public class JobExecutionHistoryRow
{
    public Guid Id { get; set; }
    public required Guid TenantId { get; set; }
    public required Guid JobDefinitionId { get; set; }
    
    /// <summary>
    /// Denormalized job name for queries
    /// </summary>
    public required string JobName { get; set; }
    
    /// <summary>
    /// Denormalized job type for queries
    /// </summary>
    public required string JobType { get; set; }
    
    public int ExecutionNumber { get; set; }
    public DateTime ScheduledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? DurationMs { get; set; }
    
    public required string Status { get; set; }
    public string? ClaimedBy { get; set; }
    
    /// <summary>
    /// Result data (JSON)
    /// </summary>
    public string? ResultData { get; set; }
    
    public string? ErrorMessage { get; set; }
    public string? ErrorType { get; set; }
    
    public int RetryCount { get; set; }
    
    /// <summary>
    /// When this execution was archived
    /// </summary>
    public required DateTime ArchivedAt { get; set; }
}
