using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Tenancy.Application.Tenants;

/// <summary>
/// Use case: Activate a suspended tenant
/// </summary>
public sealed class ActivateTenantUseCase
{
    private readonly ITenantRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateTenantUseCase(ITenantRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Retrieve tenant
        var tenant = await _repository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null)
        {
            return Result.Fail<bool, string>($"Tenant with ID '{tenantId}' not found");
        }

        // Check if already active
        if (tenant.Status == TenantStatus.Active)
        {
            return Result.Fail<bool, string>($"Tenant '{tenant.Name}' is already active");
        }

        // Update status
        tenant.Status = TenantStatus.Active;
        tenant.UpdatedAt = DateTimeOffset.UtcNow;

        // Persist
        await _repository.UpdateAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish TenantActivated domain event
        // TODO: Notify tenant administrators

        return Result.Ok<bool, string>(true);
    }
}
