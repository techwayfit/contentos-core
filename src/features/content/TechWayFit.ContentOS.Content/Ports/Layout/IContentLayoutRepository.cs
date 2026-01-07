using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Content.Ports.Layout;

public interface IContentLayoutRepository : IRepository<Domain.Layout.ContentLayout, Guid>
{
    Task<Domain.Layout.ContentLayout?> GetByVersionAsync(Guid tenantId, Guid contentVersionId);
}
