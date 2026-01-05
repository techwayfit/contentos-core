namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

/// <summary>
/// Many-to-many mapping of users to groups.
/// Primary key: Id, Unique constraint: (TenantId, UserId, GroupId)
/// </summary>
public class UserGroupRow : BaseTenantEntity
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    
    // Navigation
    public TenantRow? Tenant { get; set; }
    public UserRow? User { get; set; }
    public GroupRow? Group { get; set; }
}
