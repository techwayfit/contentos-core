using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Groups;

/// <summary>
/// Use case: Update group information
/// </summary>
public sealed class UpdateGroupUseCase
{
    private readonly IGroupRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateGroupUseCase(IGroupRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid groupId,
        Guid tenantId,
        string? name = null,
        CancellationToken cancellationToken = default)
    {
        // Retrieve group
        var group = await _repository.GetByIdAsync(groupId, cancellationToken);
        if (group == null || group.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"Group with ID '{groupId}' not found in this tenant");
        }

        // Update name if provided
        if (!string.IsNullOrWhiteSpace(name))
        {
            // TODO: Check name uniqueness
            group.Name = name;
        }

        // Update audit info
        group.Audit.UpdatedOn = DateTime.UtcNow;

        // Persist
        await _repository.UpdateAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish GroupUpdated domain event

        return Result.Ok<bool, string>(true);
    }
}
