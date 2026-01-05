namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

/// <summary>
/// A content instance of a given type (stable identity).
/// Fields live in versions, not here.
/// </summary>
public class ContentItemRow : BaseTenantSiteEntity
{
    public Guid ContentTypeId { get; set; }
    public string Status { get; set; } = string.Empty; // active|archived
    
    // Navigation
    public ContentTypeRow? ContentType { get; set; }
}
