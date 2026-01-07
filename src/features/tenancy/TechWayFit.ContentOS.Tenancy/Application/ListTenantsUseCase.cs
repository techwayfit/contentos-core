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
        if (status.HasValue)
        {
            return await _repository.ListByStatusAsync(status.Value, cancellationToken);
        }

        return await _repository.GetAllAsync(cancellationToken);
    }
}
