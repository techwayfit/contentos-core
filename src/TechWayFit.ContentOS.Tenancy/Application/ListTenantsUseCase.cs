using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Tenancy.Application;

/// <summary>
/// Use case: List all tenants with optional filtering
/// </summary>
public sealed class ListTenantsUseCase
{
    private readonly ITenantRepository _repository;

    public ListTenantsUseCase(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<Tenant>> ExecuteAsync(TenantStatus? status = null, CancellationToken cancellationToken = default)
    {
        return await _repository.ListAsync(status, cancellationToken);
    }
}
