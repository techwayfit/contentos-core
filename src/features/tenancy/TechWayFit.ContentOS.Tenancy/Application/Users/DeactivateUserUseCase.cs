using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Users;

/// <summary>
/// Use case: Deactivate a user account (soft delete)
/// </summary>
public sealed class DeactivateUserUseCase
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateUserUseCase(IUserRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Retrieve user
        var user = await _repository.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"User with ID '{userId}' not found in this tenant");
        }

        // Check if already deactivated
        if (user.Status == "Inactive")
        {
            return Result.Fail<bool, string>($"User '{user.DisplayName}' is already deactivated");
        }

        // Update status
        user.Status = "Inactive";
        user.Audit.UpdatedOn = DateTime.UtcNow;

        // Persist
        await _repository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish UserDeactivated domain event
        // TODO: Invalidate all active sessions
        // TODO: Remove from active group memberships

        return Result.Ok<bool, string>(true);
    }
}
