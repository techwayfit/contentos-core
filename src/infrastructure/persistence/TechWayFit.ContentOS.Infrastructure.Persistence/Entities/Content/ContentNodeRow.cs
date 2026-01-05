namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

/// <summary>
/// Content hierarchy ("tree") for organizing content - folders, items, links, and mounts.
/// </summary>
public class ContentNodeRow : BaseTenantSiteEntity
{
    public Guid? ParentId { get; set; }
    public string NodeType { get; set; } = string.Empty; // Folder|Item|Link|Mount
    public Guid? ContentItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool InheritAcl { get; set; }
    
    // Navigation
    public ContentNodeRow? Parent { get; set; }
    public ContentItemRow? ContentItem { get; set; }
}
