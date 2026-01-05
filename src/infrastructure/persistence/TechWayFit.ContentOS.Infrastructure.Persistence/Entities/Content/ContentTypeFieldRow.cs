namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

/// <summary>
/// Field definitions for each content type.
/// </summary>
public class ContentTypeFieldRow : BaseTenantEntity
{
    public Guid ContentTypeId { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty; // string|richtext|number|bool|datetime|ref|json
    public bool IsRequired { get; set; }
    public bool IsLocalized { get; set; }
    public string ConstraintsJson { get; set; } = "{}";
    public int SortOrder { get; set; }
    
    // Navigation
    public ContentTypeRow? ContentType { get; set; }
}
