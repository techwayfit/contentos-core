using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Users;

/// <summary>
/// Use case: Reactivate a deactivated user account
/// </summary>
public sealed class ReactivateUserUseCase
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ReactivateUserUseCase(IUserRepository repository, IUnitOfWork unitOfWork)
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

        // Check if already active
        if (user.Status == "Active")
        {
            return Result.Fail<bool, string>($"User '{user.DisplayName}' is already active");
        }

        // Update status
        user.Status = "Active";
        user.Audit.UpdatedOn = DateTime.UtcNow;

        // Persist
        await _repository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish UserReactivated domain event

        return Result.Ok<bool, string>(true);
    }
}
