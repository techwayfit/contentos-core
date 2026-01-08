using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Roles;

/// <summary>
/// Use case: Get a role by ID
/// </summary>
public sealed class GetRoleUseCase
{
    private readonly IRoleRepository _repository;

    public GetRoleUseCase(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Role, string>> ExecuteAsync(
        Guid roleId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var role = await _repository.GetByIdAsync(roleId, cancellationToken);
        
        if (role == null || role.TenantId != tenantId)
        {
            return Result.Fail<Role, string>($"Role with ID '{roleId}' not found in this tenant");
        }

        return Result.Ok<Role, string>(role);
    }
}
