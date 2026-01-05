namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Collaboration;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

/// <summary>
/// Generic commenting system.
/// </summary>
public class CommentRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid EntityInstanceId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public string CommentText { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public bool IsDeleted { get; set; }
    
    // Navigation
    public CommentRow? ParentComment { get; set; }
    public UserRow? Creator { get; set; }
}
