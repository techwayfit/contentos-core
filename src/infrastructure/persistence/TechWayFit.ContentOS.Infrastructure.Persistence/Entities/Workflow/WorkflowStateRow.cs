namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Workflow;

/// <summary>
/// Workflow states within a definition.
/// </summary>
public class WorkflowStateRow : BaseTenantEntity
{
    public Guid WorkflowDefinitionId { get; set; }
    public string StateKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsTerminal { get; set; }
    
    // Navigation
    public WorkflowDefinitionRow? WorkflowDefinition { get; set; }
}
