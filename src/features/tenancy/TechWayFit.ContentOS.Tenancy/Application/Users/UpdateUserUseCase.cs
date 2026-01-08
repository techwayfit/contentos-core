using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Users;

/// <summary>
/// Use case: Update user profile information
/// </summary>
public sealed class UpdateUserUseCase
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserUseCase(IUserRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid userId,
        Guid tenantId,
        string? email = null,
        string? displayName = null,
        CancellationToken cancellationToken = default)
    {
        // Retrieve user
        var user = await _repository.GetByIdAsync(userId, cancellationToken);
        if (user == null || user.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"User with ID '{userId}' not found in this tenant");
        }

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailLower = email.ToLowerInvariant();
            if (user.Email != emailLower)
            {
                // Check email uniqueness
                if (await _repository.EmailExistsAsync(tenantId, emailLower, cancellationToken))
                {
                    return Result.Fail<bool, string>($"Email '{email}' is already in use");
                }
                user.Email = emailLower;
            }
        }

        if (!string.IsNullOrWhiteSpace(displayName))
        {
            user.DisplayName = displayName;
        }

        // Update audit info
        user.Audit.UpdatedOn = DateTime.UtcNow;

        // Persist
        await _repository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish UserUpdated domain event

        return Result.Ok<bool, string>(true);
    }
}
