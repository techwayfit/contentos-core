using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Modules;
using TechWayFit.ContentOS.Kernel.Domain.Modules;
using TechWayFit.ContentOS.Kernel.Ports.Modules;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Modules;

public class ModuleCapabilityRepository : EfCoreRepository<ModuleCapability, ModuleCapabilityRow, Guid>, IModuleCapabilityRepository
{
    public ModuleCapabilityRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override ModuleCapability MapToDomain(ModuleCapabilityRow row)
    {
        return new ModuleCapability
        {
            Id = row.Id,
            TenantId = row.TenantId,
            ModuleId = row.ModuleId,
            CapabilityKey = row.CapabilityKey,
            Description = row.Description,
            RequiredByDependents = row.RequiredByDependents,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ModuleCapabilityRow MapToRow(ModuleCapability entity)
    {
        var row = new ModuleCapabilityRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            ModuleId = entity.ModuleId,
            CapabilityKey = entity.CapabilityKey,
            Description = entity.Description,
            RequiredByDependents = entity.RequiredByDependents
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ModuleCapabilityRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<ModuleCapability>> GetByModuleAsync(Guid tenantId, Guid moduleId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.ModuleId == moduleId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<ModuleCapability?> GetByKeyAsync(Guid tenantId, Guid moduleId, string capabilityKey)
    {
        var row = await DbSet
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.ModuleId == moduleId && r.CapabilityKey == capabilityKey);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task EnableAsync(Guid capabilityId)
    {
        var row = await DbSet.FindAsync(capabilityId);
        if (row != null)
        {
            row.IsActive = true;
            row.UpdatedOn = DateTime.UtcNow;
        }
    }

    public async Task DisableAsync(Guid capabilityId)
    {
        var row = await DbSet.FindAsync(capabilityId);
        if (row != null)
        {
            row.IsActive = false;
            row.UpdatedOn = DateTime.UtcNow;
        }
    }
}
