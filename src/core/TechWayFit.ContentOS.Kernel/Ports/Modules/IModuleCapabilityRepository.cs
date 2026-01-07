using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Kernel.Ports.Modules;

public interface IModuleCapabilityRepository : IRepository<Domain.Modules.ModuleCapability, Guid>
{
    Task<IEnumerable<Domain.Modules.ModuleCapability>> GetByModuleAsync(Guid tenantId, Guid moduleId);
    Task<Domain.Modules.ModuleCapability?> GetByKeyAsync(Guid tenantId, Guid moduleId, string capabilityKey);
    Task EnableAsync(Guid capabilityId);
    Task DisableAsync(Guid capabilityId);
}
