using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Core;

namespace TechWayFit.ContentOS.Content.Ports.Core;

/// <summary>
/// Repository interface for ContentType entity persistence
/// </summary>
public interface IContentTypeRepository : IRepository<ContentType, Guid>
{
    Task<ContentType?> GetByTypeKeyAsync(Guid tenantId, string typeKey, CancellationToken cancellationToken = default);
    Task<bool> TypeKeyExistsAsync(Guid tenantId, string typeKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentType>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
}
