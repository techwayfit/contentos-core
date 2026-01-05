namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

/// <summary>
/// Versioned lifecycle (draft/review/published).
/// Enables safe editing, approvals, history, rollback.
/// </summary>
public class ContentVersionRow : BaseTenantEntity
{
    public Guid ContentItemId { get; set; }
    public int VersionNumber { get; set; }
    public string Lifecycle { get; set; } = string.Empty; // draft|review|published|archived
    public Guid? WorkflowStateId { get; set; }
    public DateTime? PublishedAt { get; set; }
    
    // Navigation
    public ContentItemRow? ContentItem { get; set; }
}
