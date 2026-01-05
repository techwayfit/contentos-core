namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

/// <summary>
/// Stores content field values per version with localization support.
/// </summary>
public class ContentFieldValueRow : BaseTenantEntity
{
    public Guid ContentVersionId { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string? Locale { get; set; }
    public string ValueJson { get; set; } = "{}";
    
    // Navigation
    public ContentVersionRow? ContentVersion { get; set; }
}
