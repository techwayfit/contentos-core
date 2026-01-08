using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;
using TechWayFit.ContentOS.Tenancy.Ports;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

namespace TechWayFit.ContentOS.Tenancy.Application.Users;

/// <summary>
/// Use case: Create a new user account
/// </summary>
public sealed class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserUseCase(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid, string>> ExecuteAsync(
        Guid tenantId,
        string email,
        string displayName,
        CancellationToken cancellationToken = default)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(email))
            return Result.Fail<Guid, string>("Email cannot be empty");

        if (string.IsNullOrWhiteSpace(displayName))
            return Result.Fail<Guid, string>("Display name cannot be empty");

        // Validate email format
        if (!IsValidEmail(email))
            return Result.Fail<Guid, string>("Invalid email format");

        // Validate tenant exists
        var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant == null)
        {
            return Result.Fail<Guid, string>($"Tenant with ID '{tenantId}' not found");
        }

        // Check if email already exists within tenant
        if (await _userRepository.EmailExistsAsync(tenantId, email, cancellationToken))
        {
            return Result.Fail<Guid, string>($"User with email '{email}' already exists in this tenant");
        }

        // Create user entity
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Email = email.ToLowerInvariant(),
            DisplayName = displayName,
            Status = "Active",
            Audit = new AuditInfo
            {
                CreatedOn = DateTime.UtcNow
            }
        };

        // Persist
        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // TODO: Publish UserCreated domain event
        // TODO: Send welcome email
        // TODO: Assign default role

        return Result.Ok<Guid, string>(user.Id);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
