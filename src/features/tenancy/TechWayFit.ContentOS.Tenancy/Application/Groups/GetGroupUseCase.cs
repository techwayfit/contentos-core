using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Groups;

/// <summary>
/// Use case: Get a group by ID
/// </summary>
public sealed class GetGroupUseCase
{
    private readonly IGroupRepository _repository;

    public GetGroupUseCase(IGroupRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Group, string>> ExecuteAsync(
        Guid groupId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var group = await _repository.GetByIdAsync(groupId, cancellationToken);
        
        if (group == null || group.TenantId != tenantId)
        {
            return Result.Fail<Group, string>($"Group with ID '{groupId}' not found in this tenant");
        }

        return Result.Ok<Group, string>(group);
    }
}
