using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Tenancy.Application;

/// <summary>
/// Use case: Update an existing tenant
/// </summary>
public sealed class UpdateTenantUseCase
{
    private readonly ITenantRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantUseCase(ITenantRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(Guid id, string name, TenantStatus status, CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name cannot be empty", nameof(name));

        var tenant = await _repository.GetByIdAsync(id, cancellationToken);
        
        if (tenant == null)
        {
            throw new InvalidOperationException($"Tenant {id} not found");
        }

        // Update tenant properties
        tenant.Name = name;
        tenant.Status = status;
        tenant.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.UpdateAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
