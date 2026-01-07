using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Content.Domain.Layout;

public class ContentLayout
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ContentVersionId { get; set; }
    public Guid? LayoutDefinitionId { get; set; }
    public string CompositionJson { get; set; } = "{}";
    public AuditInfo Audit { get; set; } = new();
}
