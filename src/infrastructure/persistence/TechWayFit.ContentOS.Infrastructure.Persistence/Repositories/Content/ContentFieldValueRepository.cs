using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Content;

public class ContentFieldValueRepository : EfCoreRepository<ContentFieldValue, ContentFieldValueRow, Guid>, IContentFieldValueRepository
{
    public ContentFieldValueRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override ContentFieldValue MapToDomain(ContentFieldValueRow row)
    {
        return new ContentFieldValue
        {
            Id = row.Id,
            TenantId = row.TenantId,
            ContentVersionId = row.ContentVersionId,
            FieldKey = row.FieldKey,
            Locale = row.Locale,
            ValueJson = row.ValueJson,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ContentFieldValueRow MapToRow(ContentFieldValue entity)
    {
        var row = new ContentFieldValueRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            ContentVersionId = entity.ContentVersionId,
            FieldKey = entity.FieldKey,
            Locale = entity.Locale,
            ValueJson = entity.ValueJson
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ContentFieldValueRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<ContentFieldValue>> GetByVersionAsync(Guid tenantId, Guid contentVersionId)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.ContentVersionId == contentVersionId)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<ContentFieldValue?> GetByFieldKeyAsync(Guid tenantId, Guid contentVersionId, string fieldKey, string? locale = null)
    {
        var query = DbSet.Where(r => r.TenantId == tenantId && r.ContentVersionId == contentVersionId && r.FieldKey == fieldKey);
        
        if (locale != null)
        {
            query = query.Where(r => r.Locale == locale);
        }
        else
        {
            query = query.Where(r => r.Locale == null || r.Locale == string.Empty);
        }
        
        var row = await query.FirstOrDefaultAsync();
        return row != null ? MapToDomain(row) : null;
    }

    public async Task<IEnumerable<ContentFieldValue>> GetLocalizedAsync(Guid tenantId, Guid contentVersionId, string locale)
    {
        var rows = await DbSet
            .Where(r => r.TenantId == tenantId && r.ContentVersionId == contentVersionId && r.Locale == locale)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }
}
