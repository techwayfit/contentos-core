using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;
using TechWayFit.ContentOS.Kernel.Domain.Modules;
using TechWayFit.ContentOS.Kernel.Ports.Modules;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Modules;

public class ModuleMigrationRepository : EfCoreRepository<ModuleMigration, ModuleMigrationRow, Guid>, IModuleMigrationRepository
{
    public ModuleMigrationRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override ModuleMigration MapToDomain(ModuleMigrationRow row)
    {
        return new ModuleMigration
        {
            Id = row.Id,
            TenantId = row.TenantId,
            ModuleId = row.ModuleId,
            MigrationKey = row.MigrationKey,
            AppliedAt = row.AppliedAt,
            AppliedByUserId = row.AppliedByUserId,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ModuleMigrationRow MapToRow(ModuleMigration entity)
    {
        var row = new ModuleMigrationRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            ModuleId = entity.ModuleId,
            MigrationKey = entity.MigrationKey,
            AppliedAt = entity.AppliedAt,
            AppliedByUserId = entity.AppliedByUserId
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ModuleMigrationRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<ModuleMigration>> GetByModuleAsync(Guid tenantId, Guid moduleId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.ModuleId == moduleId)
            .OrderBy(r => r.AppliedAt)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task RecordMigrationAsync(ModuleMigration migration)
    {
        await AddAsync(migration);
    }

    public async Task<IEnumerable<ModuleMigration>> GetPendingAsync(Guid tenantId, Guid moduleId)
    {
        // This is a simplified implementation
        // In production, you would compare against a list of available migrations
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.ModuleId == moduleId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }
}
