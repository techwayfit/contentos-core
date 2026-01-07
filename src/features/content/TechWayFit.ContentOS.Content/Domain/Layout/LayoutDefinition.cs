using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Content.Domain.Layout;

public class LayoutDefinition
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string LayoutKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string RegionsRulesJson { get; set; } = "{}";
    public int Version { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
