using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Content.Domain.Core;

public class ContentVersion
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ContentItemId { get; set; }
    public int VersionNumber { get; set; }
    public string Lifecycle { get; set; } = string.Empty; // draft|review|published|archived
    public Guid? WorkflowStateId { get; set; }
    public DateTime? PublishedAt { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
