using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Content.Domain.Core;

/// <summary>
/// ContentItem domain entity - Pure POCO
/// A content instance of a given type (stable identity)
/// </summary>
public sealed class ContentItem
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public Guid ContentTypeId { get; set; }
    public string Status { get; set; } = default!;
    public AuditInfo Audit { get; set; } = new();
}
