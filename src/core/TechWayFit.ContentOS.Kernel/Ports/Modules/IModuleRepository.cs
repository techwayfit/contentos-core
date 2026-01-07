using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Kernel.Ports.Modules;

public interface IModuleRepository : IRepository<Domain.Modules.Module, Guid>
{
    Task<Domain.Modules.Module?> GetByKeyAsync(Guid tenantId, string moduleKey);
    Task<IEnumerable<Domain.Modules.Module>> GetInstalledAsync(Guid tenantId);
    Task InstallAsync(Domain.Modules.Module module);
    Task UninstallAsync(Guid moduleId);
    Task UpdateStatusAsync(Guid moduleId, string status);
}
