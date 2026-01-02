using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Tenancy.Application;

/// <summary>
/// Use case: Update an existing tenant
/// </summary>
public sealed class UpdateTenantUseCase
{
    private readonly ITenantRepository _repository;

    public UpdateTenantUseCase(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task ExecuteAsync(Guid id, string name, TenantStatus status, CancellationToken cancellationToken = default)
    {
        var tenant = await _repository.GetByIdAsync(id, cancellationToken);
        
        if (tenant == null)
        {
            throw new InvalidOperationException($"Tenant {id} not found");
        }

        tenant.Update(name, status);

        await _repository.UpdateAsync(tenant, cancellationToken);
    }
}
