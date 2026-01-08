using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Tenancy.Application.Tenants;

/// <summary>
/// Use case: Create a new tenant
/// </summary>
public sealed class CreateTenantUseCase
{
    private readonly ITenantRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenantUseCase(ITenantRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> ExecuteAsync(string key, string name, CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Tenant key cannot be empty", nameof(key));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name cannot be empty", nameof(name));

        // Validate key format: lowercase alphanumeric with hyphens
        if (!System.Text.RegularExpressions.Regex.IsMatch(key, "^[a-z0-9-]+$"))
            throw new ArgumentException("Tenant key must be lowercase alphanumeric with hyphens only", nameof(key));

        // Check if key already exists
        if (await _repository.KeyExistsAsync(key, cancellationToken))
        {
            throw new InvalidOperationException($"Tenant with key '{key}' already exists");
        }

        // Create tenant entity
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Key = key,
            Name = name,
            Status = TenantStatus.Active,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Persist
        await _repository.AddAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tenant.Id;
    }
}
