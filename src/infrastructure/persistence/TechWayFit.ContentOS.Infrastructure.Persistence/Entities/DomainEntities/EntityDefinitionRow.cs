namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.DomainEntities;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;

/// <summary>
/// Schema registry for domain-specific entities (Tickets, Patients, Orders, etc.).
/// </summary>
public class EntityDefinitionRow : BaseTenantEntity
{
    public Guid ModuleId { get; set; }
    public string EntityKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string SchemaJson { get; set; } = "{}";
    public bool SupportsVersioning { get; set; }
    public bool SupportsWorkflow { get; set; }
    
    // Navigation
    public ModuleRow? Module { get; set; }
}
