namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

/// <summary>
/// Directory-style grouping of users for team-based permissions.
/// </summary>
public class GroupRow : BaseTenantEntity
{
    public string Name { get; set; } = string.Empty;
    
    // Navigation
    public TenantRow? Tenant { get; set; }
}
