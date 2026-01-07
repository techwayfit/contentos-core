using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Content.Ports.Core;

public interface IContentFieldValueRepository : IRepository<Domain.Core.ContentFieldValue, Guid>
{
    Task<IEnumerable<Domain.Core.ContentFieldValue>> GetByVersionAsync(Guid tenantId, Guid contentVersionId);
    Task<Domain.Core.ContentFieldValue?> GetByFieldKeyAsync(Guid tenantId, Guid contentVersionId, string fieldKey, string? locale = null);
    Task<IEnumerable<Domain.Core.ContentFieldValue>> GetLocalizedAsync(Guid tenantId, Guid contentVersionId, string locale);
}
