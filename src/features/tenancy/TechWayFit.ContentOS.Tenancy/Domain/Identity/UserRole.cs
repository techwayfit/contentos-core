namespace TechWayFit.ContentOS.Tenancy.Domain.Identity;

/// <summary>
/// User-Role association (many-to-many)
/// </summary>
public class UserRole
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTimeOffset AssignedAt { get; set; }
}
