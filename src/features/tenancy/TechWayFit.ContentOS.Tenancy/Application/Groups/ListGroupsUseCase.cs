using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Groups;

/// <summary>
/// Use case: List all groups for a tenant
/// </summary>
public sealed class ListGroupsUseCase
{
    private readonly IGroupRepository _repository;

    public ListGroupsUseCase(IGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<Group>, string>> ExecuteAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // TODO: IGroupRepository needs GetByTenantIdAsync method
        // For now, returning empty list as placeholder
        var groups = new List<Group>().AsReadOnly();
        
        return Result.Ok<IReadOnlyList<Group>, string>(groups);
    }
}
