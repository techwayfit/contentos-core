using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Core;
using TechWayFit.ContentOS.Tenancy.Ports.Core;

namespace TechWayFit.ContentOS.Tenancy.Application.Sites;

/// <summary>
/// Use case: List all sites for a tenant
/// </summary>
public sealed class ListSitesUseCase
{
    private readonly ISiteRepository _repository;

    public ListSitesUseCase(ISiteRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<Site>, string>> ExecuteAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var sites = await _repository.GetByTenantIdAsync(tenantId, cancellationToken);
        
        return Result.Ok<IReadOnlyList<Site>, string>(sites);
    }
}
