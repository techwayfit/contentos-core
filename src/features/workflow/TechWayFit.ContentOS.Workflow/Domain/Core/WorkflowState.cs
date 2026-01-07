using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Workflow.Domain.Core;

/// <summary>
/// WorkflowState domain entity - Pure POCO
/// Workflow states within a definition
/// </summary>
public sealed class WorkflowState
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid WorkflowDefinitionId { get; set; }
    public string StateKey { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public bool IsTerminal { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
