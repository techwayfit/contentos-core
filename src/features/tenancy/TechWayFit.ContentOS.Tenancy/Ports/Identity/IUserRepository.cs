using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Tenancy.Domain.Identity;

namespace TechWayFit.ContentOS.Tenancy.Ports.Identity;

/// <summary>
/// Repository interface for User entity persistence
/// </summary>
public interface IUserRepository : IRepository<User, Guid>
{
    Task<User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(Guid tenantId, string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByStatusAsync(Guid tenantId, string status, CancellationToken cancellationToken = default);
}
