using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Roles;

/// <summary>
/// Use case: Create a new role
/// </summary>
public sealed class CreateRoleUseCase
{
    private readonly IRoleRepository _roleRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleUseCase(
        IRoleRepository roleRepository,
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, string>> ExecuteAsync(
        Guid tenantId,
        string name,
        CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(name))
            return Result.Fail<Guid, string>("Role name cannot be empty");

        // Validate tenant exists
        var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null)
        {
            return Result.Fail<Guid, string>($"Tenant with ID '{tenantId}' not found");
        }

        // Check if role name already exists within tenant
        if (await _roleRepository.NameExistsAsync(tenantId, name, cancellationToken))
        {
            return Result.Fail<Guid, string>($"Role with name '{name}' already exists in this tenant");
        }

        // Create role entity
        var role = new Role
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            Audit = new AuditInfo
            {
                CreatedOn = DateTime.UtcNow
            }
        };

        // Persist
        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish RoleCreated domain event
        // TODO: Initialize default permissions

        return Result.Ok<Guid, string>(role.Id);
    }
}
