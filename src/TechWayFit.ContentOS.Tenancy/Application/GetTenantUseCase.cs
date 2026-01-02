using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Tenancy.Application;

/// <summary>
/// Use case: Get a single tenant by ID
/// </summary>
public sealed class GetTenantUseCase
{
    private readonly ITenantRepository _repository;

    public GetTenantUseCase(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Tenant?> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }
}
