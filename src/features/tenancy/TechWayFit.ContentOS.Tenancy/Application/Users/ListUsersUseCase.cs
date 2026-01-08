using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Users;

/// <summary>
/// Use case: List users for a tenant with optional filtering
/// </summary>
public sealed class ListUsersUseCase
{
    private readonly IUserRepository _repository;

    public ListUsersUseCase(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<User>, string>> ExecuteAsync(
        Guid tenantId,
        string? statusFilter = null,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<User> users;

        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            users = await _repository.GetByStatusAsync(tenantId, statusFilter, cancellationToken);
        }
        else
        {
            users = await _repository.GetByTenantIdAsync(tenantId, cancellationToken);
        }
        
        return Result.Ok<IReadOnlyList<User>, string>(users);
    }
}
