using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Core;
using TechWayFit.ContentOS.Tenancy.Ports.Core;

namespace TechWayFit.ContentOS.Tenancy.Application.Sites;

/// <summary>
/// Use case: Get a site by ID
/// </summary>
public sealed class GetSiteUseCase
{
    private readonly ISiteRepository _repository;

    public GetSiteUseCase(ISiteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Site, string>> ExecuteAsync(
        Guid siteId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var site = await _repository.GetByIdAsync(siteId, cancellationToken);
        
        if (site == null || site.TenantId != tenantId)
        {
            return Result.Fail<Site, string>($"Site with ID '{siteId}' not found in this tenant");
        }

        return Result.Ok<Site, string>(site);
    }
}
