using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Groups;

/// <summary>
/// Use case: Create a new user group
/// </summary>
public sealed class CreateGroupUseCase
{
    private readonly IGroupRepository _groupRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateGroupUseCase(
        IGroupRepository groupRepository,
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork)
    {
        _groupRepository = groupRepository;
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
            return Result.Fail<Guid, string>("Group name cannot be empty");

        // Validate tenant exists
        var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null)
        {
            return Result.Fail<Guid, string>($"Tenant with ID '{tenantId}' not found");
        }

        // TODO: Check if group name already exists within tenant

        // Create group entity
        var group = new Group
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
        await _groupRepository.AddAsync(group, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish GroupCreated domain event

        return Result.Ok<Guid, string>(group.Id);
    }
}
