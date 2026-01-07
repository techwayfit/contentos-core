using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;
using TechWayFit.ContentOS.Kernel.Domain.Modules;
using TechWayFit.ContentOS.Kernel.Ports.Modules;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Modules;

public class ModuleSettingRepository : EfCoreRepository<ModuleSetting, ModuleSettingRow, Guid>, IModuleSettingRepository
{
    public ModuleSettingRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override ModuleSetting MapToDomain(ModuleSettingRow row)
    {
        return new ModuleSetting
        {
            Id = row.Id,
            TenantId = row.TenantId,
            ModuleId = row.ModuleId,
            SettingKey = row.SettingKey,
            ValueJson = row.ValueJson,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ModuleSettingRow MapToRow(ModuleSetting entity)
    {
        var row = new ModuleSettingRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            ModuleId = entity.ModuleId,
            SettingKey = entity.SettingKey,
            ValueJson = entity.ValueJson
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ModuleSettingRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<ModuleSetting>> GetByModuleAsync(Guid tenantId, Guid moduleId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.ModuleId == moduleId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<ModuleSetting?> GetByKeyAsync(Guid tenantId, Guid moduleId, string settingKey)
    {
        var row = await DbSet
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.ModuleId == moduleId && r.SettingKey == settingKey);
        return row != null ? MapToDomain(row) : null;
    }
}
