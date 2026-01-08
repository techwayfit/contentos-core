using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Users;

/// <summary>
/// Use case: Get a user by email
/// </summary>
public sealed class GetUserByEmailUseCase
{
    private readonly IUserRepository _repository;

    public GetUserByEmailUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<User, string>> ExecuteAsync(
        Guid tenantId,
        string email,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result.Fail<User, string>("Email cannot be empty");
        }

        var user = await _repository.GetByEmailAsync(tenantId, email.ToLowerInvariant(), cancellationToken);
        
        if (user == null)
        {
            return Result.Fail<User, string>($"User with email '{email}' not found");
        }

        return Result.Ok<User, string>(user);
    }
}
