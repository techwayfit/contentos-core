namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

/// <summary>
/// Module-specific configuration per tenant/site.
/// </summary>
public class ModuleSettingRow : BaseTenantEntity
{
    public Guid ModuleId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string ValueJson { get; set; } = "{}";
    
    // Navigation
    public ModuleRow? Module { get; set; }
}
