using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Roles;

/// <summary>
/// Use case: Delete a role
/// </summary>
public sealed class DeleteRoleUseCase
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoleUseCase(
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid roleId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Retrieve role
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null || role.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"Role with ID '{roleId}' not found in this tenant");
        }

        // TODO: Check if users are assigned to this role
        // TODO: Prevent deletion or require migration to another role

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // TODO: Remove all user-role assignments
            
            // Delete role
            await _roleRepository.DeleteAsync(roleId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // TODO: Publish RoleDeleted domain event

            return Result.Ok<bool, string>(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Fail<bool, string>($"Failed to delete role: {ex.Message}");
        }
    }
}
