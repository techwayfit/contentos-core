namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

/// <summary>
/// Core identity record (auth handled externally via IdP).
/// </summary>
public class UserRow : BaseTenantEntity
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    
    // Navigation
    public TenantRow? Tenant { get; set; }
}
