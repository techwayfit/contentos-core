using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain.Core;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Content;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Content;

public class ContentTypeFieldRepository : EfCoreRepository<ContentTypeField, ContentTypeFieldRow, Guid>, IContentTypeFieldRepository
{
    public ContentTypeFieldRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override ContentTypeField MapToDomain(ContentTypeFieldRow row)
    {
        return new ContentTypeField
        {
            Id = row.Id,
            TenantId = row.TenantId,
            ContentTypeId = row.ContentTypeId,
            FieldKey = row.FieldKey,
            DataType = row.DataType,
            IsRequired = row.IsRequired,
            IsLocalized = row.IsLocalized,
            ConstraintsJson = row.ConstraintsJson,
            SortOrder = row.SortOrder,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override ContentTypeFieldRow MapToRow(ContentTypeField entity)
    {
        var row = new ContentTypeFieldRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            ContentTypeId = entity.ContentTypeId,
            FieldKey = entity.FieldKey,
            DataType = entity.DataType,
            IsRequired = entity.IsRequired,
            IsLocalized = entity.IsLocalized,
            ConstraintsJson = entity.ConstraintsJson,
            SortOrder = entity.SortOrder
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<ContentTypeFieldRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<IEnumerable<ContentTypeField>> GetByContentTypeAsync(Guid tenantId, Guid contentTypeId)
    {
        var rows = await Context.Set<ContentTypeFieldRow>()
            .Where(r => r.TenantId == tenantId && r.ContentTypeId == contentTypeId)
            .OrderBy(r => r.SortOrder)
            .ToListAsync();
        return rows.Select(MapToDomain);
    }

    public async Task<ContentTypeField?> GetByFieldKeyAsync(Guid tenantId, Guid contentTypeId, string fieldKey)
    {
        var row = await Context.Set<ContentTypeFieldRow>()
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.ContentTypeId == contentTypeId && r.FieldKey == fieldKey);
        return row != null ? MapToDomain(row) : null;
    }
}
