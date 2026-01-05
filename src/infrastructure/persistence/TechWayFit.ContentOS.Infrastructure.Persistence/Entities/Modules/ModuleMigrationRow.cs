namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

/// <summary>
/// Tracks module schema migrations.
/// </summary>
public class ModuleMigrationRow : BaseTenantEntity
{
    public Guid ModuleId { get; set; }
    public string MigrationKey { get; set; } = string.Empty;
    public DateTime AppliedAt { get; set; }
    public Guid AppliedByUserId { get; set; }
    
    // Navigation
    public ModuleRow? Module { get; set; }
}
