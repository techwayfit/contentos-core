using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Kernel.Domain.Modules;

public class Module
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string ModuleKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string InstallationStatus { get; set; } = string.Empty;
    public DateTime InstalledOn { get; set; }
    public string DependenciesJson { get; set; } = "[]";
    public AuditInfo Audit { get; set; } = new();
}
