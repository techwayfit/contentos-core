using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Tenancy.Application;

/// <summary>
/// Use case: Create a new tenant
/// </summary>
public sealed class CreateTenantUseCase
{
    private readonly ITenantRepository _repository;

    public CreateTenantUseCase(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<Guid> ExecuteAsync(string key, string name, CancellationToken cancellationToken = default)
    {
        // Check if key already exists
        if (await _repository.KeyExistsAsync(key, cancellationToken))
        {
            throw new InvalidOperationException($"Tenant with key '{key}' already exists");
        }

        // Create tenant domain entity
        var tenant = Tenant.Create(key, name);

        // Persist
        var id = await _repository.AddAsync(tenant, cancellationToken);

        return id;
    }
}
