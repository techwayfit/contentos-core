using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Kernel.Ports.Modules;

public interface IModuleSettingRepository : IRepository<Domain.Modules.ModuleSetting, Guid>
{
    Task<IEnumerable<Domain.Modules.ModuleSetting>> GetByModuleAsync(Guid tenantId, Guid moduleId);
    Task<Domain.Modules.ModuleSetting?> GetByKeyAsync(Guid tenantId, Guid moduleId, string settingKey);
}
