using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Workflow.Domain.Core;

/// <summary>
/// WorkflowDefinition domain entity - Pure POCO
/// Defines a workflow graph (e.g., Draft → Review → Publish)
/// </summary>
public sealed class WorkflowDefinition
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string WorkflowKey { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public bool IsDefault { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
