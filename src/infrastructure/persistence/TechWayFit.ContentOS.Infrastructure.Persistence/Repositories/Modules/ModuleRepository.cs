using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;
using TechWayFit.ContentOS.Kernel.Domain.Modules;
using TechWayFit.ContentOS.Kernel.Ports.Modules;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Modules;

public class ModuleRepository : EfCoreRepository<Module, ModuleRow, Guid>, IModuleRepository
{
    public ModuleRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override Module MapToDomain(ModuleRow row)
    {
        return new Module
        {
            Id = row.Id,
            TenantId = row.TenantId,
            ModuleKey = row.ModuleKey,
            DisplayName = row.DisplayName,
            Version = row.Version,
            InstallationStatus = row.InstallationStatus,
            InstalledOn = row.InstalledOn,
            DependenciesJson = row.DependenciesJson,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ModuleRow MapToRow(Module entity)
    {
        var row = new ModuleRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            ModuleKey = entity.ModuleKey,
            DisplayName = entity.DisplayName,
            Version = entity.Version,
            InstallationStatus = entity.InstallationStatus,
            InstalledOn = entity.InstalledOn,
            DependenciesJson = entity.DependenciesJson
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ModuleRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<Module?> GetByKeyAsync(Guid tenantId, string moduleKey)
    {
        var row = await DbSet
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.ModuleKey == moduleKey);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<IEnumerable<Module>> GetInstalledAsync(Guid tenantId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.InstallationStatus == "Installed")
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task InstallAsync(Module module)
    {
        await AddAsync(module);
    }

    public async Task UninstallAsync(Guid moduleId)
    {
        await DeleteAsync(moduleId);
    }

    public async Task UpdateStatusAsync(Guid moduleId, string status)
    {
        var row = await DbSet.FindAsync(moduleId);
        if (row != null)
        {
            row.InstallationStatus = status;
            row.UpdatedOn = DateTime.UtcNow;
        }
    }
}
