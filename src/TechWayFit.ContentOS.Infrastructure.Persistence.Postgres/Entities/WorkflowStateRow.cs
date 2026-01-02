using TechWayFit.ContentOS.Infrastructure.Persistence.Contracts;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

/// <summary>
/// Database row model for workflow state (NOT domain entity)
/// Tracks current workflow status and transition history
/// </summary>
public sealed class WorkflowStateRow : ITenantOwnedRow
{
    public Guid Id { get; set; }
    
    // Multi-tenant fields (from ITenantOwnedRow)
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public string Environment { get; set; } = default!;
    
    public Guid ContentItemId { get; set; }
    public int CurrentStatus { get; set; }
    public int? PreviousStatus { get; set; }
    public Guid? TransitionedBy { get; set; }
    public DateTimeOffset TransitionedAt { get; set; }
    public string? Comment { get; set; }
}
