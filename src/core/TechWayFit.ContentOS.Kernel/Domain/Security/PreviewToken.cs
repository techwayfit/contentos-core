using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Kernel.Domain.Security;

public class PreviewToken
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
    public AuditInfo Audit { get; set; } = new();
}
