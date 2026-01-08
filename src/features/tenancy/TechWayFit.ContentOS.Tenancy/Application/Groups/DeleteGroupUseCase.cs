using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Groups;

/// <summary>
/// Use case: Delete a user group
/// </summary>
public sealed class DeleteGroupUseCase
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserGroupRepository _userGroupRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteGroupUseCase(
        IGroupRepository groupRepository,
        IUserGroupRepository userGroupRepository,
        IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
        _userGroupRepository = userGroupRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid groupId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Retrieve group
        var group = await _groupRepository.GetByIdAsync(groupId, cancellationToken);
        if (group == null || group.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"Group with ID '{groupId}' not found in this tenant");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // TODO: Remove all user-group memberships
            
            // Delete group
            await _groupRepository.DeleteAsync(groupId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // TODO: Publish GroupDeleted domain event

            return Result.Ok<bool, string>(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Fail<bool, string>($"Failed to delete group: {ex.Message}");
        }
    }
}
