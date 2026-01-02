namespace TechWayFit.ContentOS.Kernel.Tenancy;

/// <summary>
/// Manages tenant context resolution and isolation
/// </summary>
public interface ITenantResolver
{
    /// <summary>
    /// Resolves tenant context from the current request
    /// Resolution strategy:
    /// 1. Subdomain: {tenant}.contentos.com
    /// 2. HTTP Header: X-Tenant-Id
    /// 3. JWT Claim: tenant_id
    /// 4. Query Parameter: ?tenantId=xxx (dev only)
    /// </summary>
    Task<TenantContext> ResolveAsync(CancellationToken cancellationToken = default);
}
