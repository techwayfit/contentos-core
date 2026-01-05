namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities;

/// <summary>
/// Base entity with TenantId for multi-tenant entities.
/// </summary>
public abstract class BaseTenantEntity : BaseEntity
{
    public Guid TenantId { get; set; }
}
