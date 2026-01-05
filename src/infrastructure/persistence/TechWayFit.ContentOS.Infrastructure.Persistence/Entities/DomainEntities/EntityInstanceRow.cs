namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.DomainEntities;

/// <summary>
/// Stores instances of domain entities with flexible JSONB storage.
/// </summary>
public class EntityInstanceRow : BaseTenantSiteEntity
{
    public Guid EntityDefinitionId { get; set; }
    public string InstanceKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string DataJson { get; set; } = "{}";
    public Guid? ParentInstanceId { get; set; }
    
    // Navigation
    public EntityDefinitionRow? EntityDefinition { get; set; }
    public EntityInstanceRow? Parent { get; set; }
}
