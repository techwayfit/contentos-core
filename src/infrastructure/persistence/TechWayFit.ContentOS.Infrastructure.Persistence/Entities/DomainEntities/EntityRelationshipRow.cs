namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.DomainEntities;

/// <summary>
/// Cross-module entity relationships.
/// </summary>
public class EntityRelationshipRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SourceInstanceId { get; set; }
    public Guid TargetInstanceId { get; set; }
    public string RelationshipType { get; set; } = string.Empty;
    public string RelationshipName { get; set; } = string.Empty;
    public string MetadataJson { get; set; } = "{}";
    public DateTime CreatedOn { get; set; }
}
