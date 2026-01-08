using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Groups;

/// <summary>
/// Use case: Remove a user from a group
/// </summary>
public sealed class RemoveUserFromGroupUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveUserFromGroupUseCase(
        IUserRepository userRepository,
        IGroupRepository groupRepository,
        IUserGroupRepository userGroupRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _groupRepository = groupRepository;
        _userGroupRepository = userGroupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid userId,
        Guid groupId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"User with ID '{userId}' not found in this tenant");
        }

        // Validate group exists
        var group = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        if (group == null || group.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"Group with ID '{groupId}' not found in this tenant");
        }

        // TODO: Find and delete the user-group association
        // For now, we'll need to add a method to IUserGroupRepository

        // Persist
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish UserRemovedFromGroup domain event

        return Result.Ok<bool, string>(true);
    }
}
