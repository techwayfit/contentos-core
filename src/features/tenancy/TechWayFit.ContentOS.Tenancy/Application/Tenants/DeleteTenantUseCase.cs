using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Tenancy.Application.Tenants;

/// <summary>
/// Use case: Permanently delete a tenant (SuperAdmin only)
/// WARNING: This is a destructive operation
/// </summary>
public sealed class DeleteTenantUseCase
{
    private readonly ITenantRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTenantUseCase(ITenantRepository repository, IUnitOfWork unitOfWork)
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

        // TODO: Check for dependencies (Sites, Users, Content, etc.)
        // TODO: Archive all tenant data before deletion
        // TODO: Policy check - must be SuperAdmin

        // Begin transaction for cascading deletions
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Delete tenant
            await _repository.DeleteAsync(tenantId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // TODO: Publish TenantDeleted domain event
            // TODO: Remove tenant data from search indexes
            // TODO: Clean up storage (media files, etc.)

            return Result.Ok<bool, string>(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Fail<bool, string>($"Failed to delete tenant: {ex.Message}");
        }
    }
}
