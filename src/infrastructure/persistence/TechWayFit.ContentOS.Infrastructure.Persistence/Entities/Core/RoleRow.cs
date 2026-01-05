namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

/// <summary>
/// Role-based access control (RBAC).
/// </summary>
public class RoleRow : BaseTenantEntity
{
    public string Name { get; set; } = string.Empty;
    
    // Navigation
    public TenantRow? Tenant { get; set; }
}
