using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Content;

public class ContentTypeRepository : EfCoreRepository<ContentType, ContentTypeRow, Guid>, IContentTypeRepository
{
    public ContentTypeRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override ContentType MapToDomain(ContentTypeRow row)
    {
        return new ContentType
        {
            Id = row.Id,
    TenantId = row.TenantId,
            TypeKey = row.TypeKey,
          DisplayName = row.DisplayName,
            SchemaVersion = row.SchemaVersion,
            SettingsJson = row.SettingsJson,
 Audit = MapAuditToDomain(row)
        };
    }

    protected override ContentTypeRow MapToRow(ContentType entity)
    {
        var row = new ContentTypeRow
        {
            Id = entity.Id,
          TenantId = entity.TenantId,
            TypeKey = entity.TypeKey,
          DisplayName = entity.DisplayName,
            SchemaVersion = entity.SchemaVersion,
    SettingsJson = entity.SettingsJson
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ContentTypeRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<ContentType?> GetByTypeKeyAsync(Guid tenantId, string typeKey, CancellationToken cancellationToken = default)
    {
      var row = await Context.Set<ContentTypeRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.TypeKey == typeKey, cancellationToken);
        return row != null ? MapToDomain(row) : null;
    }

 public async Task<bool> TypeKeyExistsAsync(Guid tenantId, string typeKey, CancellationToken cancellationToken = default)
 {
        return await Context.Set<ContentTypeRow>()
   .AnyAsync(r => r.TenantId == tenantId && r.TypeKey == typeKey, cancellationToken);
    }

    public async Task<IReadOnlyList<ContentType>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
var rows = await Context.Set<ContentTypeRow>()
   .Where(r => r.TenantId == tenantId)
        .OrderBy(r => r.DisplayName)
      .ToListAsync(cancellationToken);
   return rows.Select(MapToDomain).ToList();
    }
}
