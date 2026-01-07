using TechWayFit.ContentOS.Abstractions.Repositories;

namespace TechWayFit.ContentOS.Kernel.Ports.Modules;

public interface IModuleMigrationRepository : IRepository<Domain.Modules.ModuleMigration, Guid>
{
    Task<IEnumerable<Domain.Modules.ModuleMigration>> GetByModuleAsync(Guid tenantId, Guid moduleId);
    Task RecordMigrationAsync(Domain.Modules.ModuleMigration migration);
    Task<IEnumerable<Domain.Modules.ModuleMigration>> GetPendingAsync(Guid tenantId, Guid moduleId);
}
