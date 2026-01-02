using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Infrastructure.Runtime;

namespace TechWayFit.ContentOS.Api.Tenancy;

/// <summary>
/// Provides current tenant context to infrastructure layer
/// Adapter from runtime TenantContext to ICurrentTenantProvider port
/// </summary>
public class CurrentTenantProvider : ICurrentTenantProvider
{
    private readonly TenantContext _tenantContext;

    public CurrentTenantProvider(TenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public Guid TenantId => _tenantContext.TenantId;
    public Guid SiteId => _tenantContext.SiteId;
    public string Environment => _tenantContext.Environment;
    public string DefaultLanguage => _tenantContext.DefaultLanguage;
    public string[] SupportedLanguages => _tenantContext.SupportedLanguages;
}
