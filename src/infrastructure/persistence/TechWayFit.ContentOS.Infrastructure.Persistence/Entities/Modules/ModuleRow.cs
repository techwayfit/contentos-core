namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

/// <summary>
/// Module registry and lifecycle management.
/// </summary>
public class ModuleRow : BaseTenantEntity
{
    public string ModuleKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string InstallationStatus { get; set; } = string.Empty;
    public DateTime InstalledOn { get; set; }
    public string DependenciesJson { get; set; } = "[]";
}
