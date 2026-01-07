using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Tenancy.Domain.Identity;

public class UserGroup
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
