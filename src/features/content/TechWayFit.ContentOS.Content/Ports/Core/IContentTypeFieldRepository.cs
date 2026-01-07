using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Content.Ports.Core;

public interface IContentTypeFieldRepository : IRepository<Domain.Core.ContentTypeField, Guid>
{
    Task<IEnumerable<Domain.Core.ContentTypeField>> GetByContentTypeAsync(Guid tenantId, Guid contentTypeId);
    Task<Domain.Core.ContentTypeField?> GetByFieldKeyAsync(Guid tenantId, Guid contentTypeId, string fieldKey);
}
