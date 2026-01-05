namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Workflow;

/// <summary>
/// Allowed transitions between states, controlled by required action.
/// </summary>
public class WorkflowTransitionRow : BaseTenantEntity
{
    public Guid WorkflowDefinitionId { get; set; }
    public Guid FromStateId { get; set; }
    public Guid ToStateId { get; set; }
    public string RequiredAction { get; set; } = string.Empty;
    
    // Navigation
    public WorkflowDefinitionRow? WorkflowDefinition { get; set; }
    public WorkflowStateRow? FromState { get; set; }
    public WorkflowStateRow? ToState { get; set; }
}
