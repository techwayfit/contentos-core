namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

/// <summary>
/// Feature flags and capabilities per module.
/// </summary>
public class ModuleCapabilityRow : BaseTenantEntity
{
    public Guid ModuleId { get; set; }
    public string CapabilityKey { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool RequiredByDependents { get; set; }
    
    // Navigation
    public ModuleRow? Module { get; set; }
}
