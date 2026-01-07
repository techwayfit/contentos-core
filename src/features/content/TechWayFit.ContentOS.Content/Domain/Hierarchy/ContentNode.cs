using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Content.Domain.Hierarchy;

/// <summary>
/// ContentNode domain entity - Pure POCO
/// Tree navigation structure for content (URL paths, hierarchy)
/// </summary>
public sealed class ContentNode
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public Guid? ParentId { get; set; }
    public Guid? ContentItemId { get; set; }
    public string Slug { get; set; } = default!;
    public int SortOrder { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
