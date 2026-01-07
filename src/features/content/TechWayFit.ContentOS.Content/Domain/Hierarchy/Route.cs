using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Content.Domain.Hierarchy;

public class Route
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public Guid NodeId { get; set; }
    public string RoutePath { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
