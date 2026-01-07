using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Content.Domain.Layout;

public class ComponentDefinition
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ComponentKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PropsSchemaJson { get; set; } = "{}";
    public string OwnerModule { get; set; } = string.Empty;
    public int Version { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
