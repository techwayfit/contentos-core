namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Search;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.DomainEntities;

/// <summary>
/// Unified search across all modules and entities.
/// </summary>
public class SearchIndexEntryRow : BaseTenantSiteEntity
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Locale { get; set; } = string.Empty;
    public string SearchableText { get; set; } = string.Empty;
    public string MetadataJson { get; set; } = "{}";
    public DateTime IndexedAt { get; set; }
    
    // Navigation
    public SiteRow? Site { get; set; }
    public ModuleRow? Module { get; set; }
}
