using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Roles;

/// <summary>
/// Use case: Update role information
/// </summary>
public sealed class UpdateRoleUseCase
{
    private readonly IRoleRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleUseCase(IRoleRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool, string>> ExecuteAsync(
        Guid roleId,
        Guid tenantId,
        string? name = null,
        CancellationToken cancellationToken = default)
    {
        // Retrieve role
        var role = await _repository.GetByIdAsync(roleId, cancellationToken);
        if (role == null || role.TenantId != tenantId)
        {
            return Result.Fail<bool, string>($"Role with ID '{roleId}' not found in this tenant");
        }

        // Update name if provided
        if (!string.IsNullOrWhiteSpace(name) && role.Name != name)
        {
            // Check name uniqueness
            if (await _repository.NameExistsAsync(tenantId, name, cancellationToken))
            {
                return Result.Fail<bool, string>($"Role name '{name}' is already in use");
            }
            role.Name = name;
        }

        // Update audit info
        role.Audit.UpdatedOn = DateTime.UtcNow;

        // Persist
        await _repository.UpdateAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish RoleUpdated domain event
        // TODO: Update permissions if modified

        return Result.Ok<bool, string>(true);
    }
}
