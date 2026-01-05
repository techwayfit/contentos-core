namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Processes;

/// <summary>
/// Running instances of business processes.
/// </summary>
public class ProcessInstanceRow : BaseTenantEntity
{
    public Guid ProcessDefinitionId { get; set; }
    public Guid EntityInstanceId { get; set; }
    public string CurrentState { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartedOn { get; set; }
    public DateTime? DueOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public string ContextDataJson { get; set; } = "{}";
    
    // Navigation
    public ProcessDefinitionRow? ProcessDefinition { get; set; }
}
