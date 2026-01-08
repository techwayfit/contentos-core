using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Roles;

/// <summary>
/// Use case: Remove a role from a user
/// </summary>
public sealed class RemoveRoleFromUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveRoleFromUserUseCase(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid userId,
        Guid roleId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"User with ID '{userId}' not found in this tenant");
        }

        // Validate role exists
        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null || role.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"Role with ID '{roleId}' not found in this tenant");
        }

        // TODO: Find and delete the user-role association
        // For now, we'll need to add a method to IUserRoleRepository

        // Persist
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish RoleRemovedFromUser domain event
        // TODO: Invalidate user's permission cache

        return Result.Ok<bool, string>(true);
    }
}
