using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Tenancy.Application.Tenants;

/// <summary>
/// Use case: Suspend a tenant (disable access)
/// </summary>
public sealed class SuspendTenantUseCase
{
    private readonly ITenantRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SuspendTenantUseCase(ITenantRepository repository, IUnitOfWork unitOfWork)
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

        // Check if already suspended
        if (tenant.Status == TenantStatus.Suspended)
        {
            return Result.Fail<bool, string>($"Tenant '{tenant.Name}' is already suspended");
        }

        // Update status
        tenant.Status = TenantStatus.Suspended;
        tenant.UpdatedAt = DateTimeOffset.UtcNow;

        // Persist
        await _repository.UpdateAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish TenantSuspended domain event
        // TODO: Notify tenant administrators
        // TODO: Invalidate all active sessions for this tenant

        return Result.Ok<bool, string>(true);
    }
}
