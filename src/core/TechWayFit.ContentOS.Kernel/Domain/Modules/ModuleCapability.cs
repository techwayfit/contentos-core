using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Kernel.Domain.Modules;

public class ModuleCapability
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid ModuleId { get; set; }
    public string CapabilityKey { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool RequiredByDependents { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
