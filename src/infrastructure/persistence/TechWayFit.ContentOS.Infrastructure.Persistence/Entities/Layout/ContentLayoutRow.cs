namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Layout;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

/// <summary>
/// Stores the composed layout JSON per content version.
/// </summary>
public class ContentLayoutRow : BaseTenantEntity
{
    public Guid ContentVersionId { get; set; }
    public Guid? LayoutDefinitionId { get; set; }
    public string CompositionJson { get; set; } = "{}";
    
    // Navigation
    public ContentVersionRow? ContentVersion { get; set; }
    public LayoutDefinitionRow? LayoutDefinition { get; set; }
}
