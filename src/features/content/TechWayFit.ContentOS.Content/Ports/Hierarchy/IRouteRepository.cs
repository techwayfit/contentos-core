using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Content.Ports.Hierarchy;

public interface IRouteRepository : IRepository<Domain.Hierarchy.Route, Guid>
{
    Task<IEnumerable<Domain.Hierarchy.Route>> GetByNodeAsync(Guid tenantId, Guid nodeId);
    Task<Domain.Hierarchy.Route?> GetByRoutePathAsync(Guid tenantId, Guid siteId, string routePath);
    Task<Domain.Hierarchy.Route?> GetPrimaryAsync(Guid tenantId, Guid nodeId);
}
