using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Content.Ports.Layout;

public interface ILayoutDefinitionRepository : IRepository<Domain.Layout.LayoutDefinition, Guid>
{
    Task<Domain.Layout.LayoutDefinition?> GetByKeyAsync(Guid tenantId, string layoutKey, int? version = null);
    Task<IEnumerable<Domain.Layout.LayoutDefinition>> GetByTenantAsync(Guid tenantId);
}
