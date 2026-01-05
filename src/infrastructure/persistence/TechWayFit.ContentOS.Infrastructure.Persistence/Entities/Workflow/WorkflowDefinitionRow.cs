namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Workflow;

/// <summary>
/// Defines a workflow graph (e.g., Draft → Review → Publish).
/// </summary>
public class WorkflowDefinitionRow : BaseTenantEntity
{
    public string WorkflowKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
