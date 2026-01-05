namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Preview;

using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

/// <summary>
/// Secure preview links (time-bound, optional one-time).
/// Stores only HMAC-SHA256 hash, never raw tokens.
/// </summary>
public class PreviewTokenRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid SiteId { get; set; }
    public Guid NodeId { get; set; }
    public Guid ContentVersionId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string? IssuedToEmail { get; set; }
    public bool OneTimeUse { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation
    public SiteRow? Site { get; set; }
    public ContentNodeRow? Node { get; set; }
    public ContentVersionRow? ContentVersion { get; set; }
}
