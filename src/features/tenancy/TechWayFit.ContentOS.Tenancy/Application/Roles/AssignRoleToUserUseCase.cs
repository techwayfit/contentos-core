using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Roles;

/// <summary>
/// Use case: Assign a role to a user
/// </summary>
public sealed class AssignRoleToUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleToUserUseCase(
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

        // TODO: Check if user already has this role

        // Create user-role association
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTimeOffset.UtcNow
        };

        // Persist
        await _userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish RoleAssignedToUser domain event
        // TODO: Invalidate user's permission cache

        return Result.Ok<bool, string>(true);
    }
}
