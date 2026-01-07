using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Workflow.Domain.Core;

public class WorkflowTransition
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid WorkflowDefinitionId { get; set; }
    public Guid FromStateId { get; set; }
    public Guid ToStateId { get; set; }
    public string RequiredAction { get; set; } = string.Empty;
    public AuditInfo Audit { get; set; } = new();
}
