using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Roles;

/// <summary>
/// Use case: List all roles for a tenant
/// </summary>
public sealed class ListRolesUseCase
{
    private readonly IRoleRepository _repository;

    public ListRolesUseCase(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<Role>, string>> ExecuteAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var roles = await _repository.GetByTenantIdAsync(tenantId, cancellationToken);
        
        return Result.Ok<IReadOnlyList<Role>, string>(roles);
    }
}
