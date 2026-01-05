namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

/// <summary>
/// Many-to-many mapping of users to roles.
/// Primary key: Id, Unique constraint: (TenantId, UserId, RoleId)
/// </summary>
public class UserRoleRow : BaseTenantEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    
    // Navigation
    public TenantRow? Tenant { get; set; }
    public UserRow? User { get; set; }
    public RoleRow? Role { get; set; }
}
