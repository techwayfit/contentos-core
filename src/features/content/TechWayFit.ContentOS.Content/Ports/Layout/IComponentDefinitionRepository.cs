using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Content.Ports.Layout;

public interface IComponentDefinitionRepository : IRepository<Domain.Layout.ComponentDefinition, Guid>
{
    Task<Domain.Layout.ComponentDefinition?> GetByKeyAsync(Guid tenantId, string componentKey, int? version = null);
    Task<IEnumerable<Domain.Layout.ComponentDefinition>> GetByModuleAsync(Guid tenantId, string ownerModule);
}
