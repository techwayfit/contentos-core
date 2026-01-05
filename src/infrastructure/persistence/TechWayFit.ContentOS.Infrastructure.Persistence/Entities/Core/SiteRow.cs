namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;

/// <summary>
/// Multi-site within a tenant (hostnames, locales, delivery scope).
/// </summary>
public class SiteRow : BaseTenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string DefaultLocale { get; set; } = string.Empty;
    
    // Navigation
    public TenantRow? Tenant { get; set; }
}
