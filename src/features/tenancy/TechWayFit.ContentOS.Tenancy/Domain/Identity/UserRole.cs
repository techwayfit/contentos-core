using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Tenancy.Domain.Identity;

public class UserRole
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public AuditInfo Audit { get; set; } = new();
}
