namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

/// <summary>
/// Schema registry (Contentful-like modeling, module-owned types).
/// </summary>
public class ContentTypeRow : BaseTenantEntity
{
    public string TypeKey { get; set; } = string.Empty; // e.g., page.article
    public string DisplayName { get; set; } = string.Empty;
    public int SchemaVersion { get; set; }
    public string SettingsJson { get; set; } = "{}";
}
