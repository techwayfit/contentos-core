using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Users;

/// <summary>
/// Use case: Permanently delete a user (hard delete)
/// </summary>
public sealed class DeleteUserUseCase
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserUseCase(IUserRepository repository, IUnitOfWork unitOfWork)
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

        // TODO: Check for content ownership - reassign or anonymize
        // TODO: Remove from all groups and roles

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Delete user
            await _repository.DeleteAsync(userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // TODO: Publish UserDeleted domain event
            // TODO: Anonymize audit trails
            // TODO: Remove from external identity provider

            return Result.Ok<bool, string>(true);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Fail<bool, string>($"Failed to delete user: {ex.Message}");
        }
    }
}
