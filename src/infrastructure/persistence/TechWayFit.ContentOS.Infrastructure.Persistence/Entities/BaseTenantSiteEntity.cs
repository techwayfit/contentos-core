namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities;

/// <summary>
/// Base entity with TenantId and SiteId for site-scoped entities.
/// </summary>
public abstract class BaseTenantSiteEntity : BaseTenantEntity
{
    public Guid SiteId { get; set; }
}
