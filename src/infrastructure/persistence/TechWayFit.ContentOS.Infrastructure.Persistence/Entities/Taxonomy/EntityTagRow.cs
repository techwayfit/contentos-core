namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Taxonomy;

/// <summary>
/// Many-to-many: tags to entities.
/// </summary>
public class EntityTagRow : BaseTenantEntity
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public Guid TagId { get; set; }
    
    // Navigation
    public TagRow? Tag { get; set; }
}
