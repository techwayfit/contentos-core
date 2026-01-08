using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Users;

/// <summary>
/// Use case: Get a user by ID
/// </summary>
public sealed class GetUserUseCase
{
    private readonly IUserRepository _repository;

    public GetUserUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<User, string>> ExecuteAsync(
        Guid userId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(userId, cancellationToken);
        
        if (user == null || user.TenantId != tenantId)
        {
            return Result.Fail<User, string>($"User with ID '{userId}' not found in this tenant");
        }

        return Result.Ok<User, string>(user);
    }
}
