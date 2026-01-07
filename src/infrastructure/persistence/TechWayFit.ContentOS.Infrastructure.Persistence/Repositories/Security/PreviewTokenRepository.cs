using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Preview;
using TechWayFit.ContentOS.Kernel.Domain.Security;
using TechWayFit.ContentOS.Kernel.Ports.Security;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Security;

public class PreviewTokenRepository : EfCoreRepository<PreviewToken, PreviewTokenRow, Guid>, IPreviewTokenRepository
{
    public PreviewTokenRepository(DbContext dbContext) : base(dbContext)
    {
    }

    protected override PreviewToken MapToDomain(PreviewTokenRow row)
    {
        return new PreviewToken
        {
            Id = row.Id,
            TenantId = row.TenantId,
            SiteId = row.SiteId,
            NodeId = row.NodeId,
            ContentVersionId = row.ContentVersionId,
            TokenHash = row.TokenHash,
            ExpiresAt = row.ExpiresAt,
            IssuedToEmail = row.IssuedToEmail,
            OneTimeUse = row.OneTimeUse,
            UsedAt = row.UsedAt,
            Audit = MapAuditToDomain(row)
        };
    }

    protected override PreviewTokenRow MapToRow(PreviewToken entity)
    {
        var row = new PreviewTokenRow
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            SiteId = entity.SiteId,
            NodeId = entity.NodeId,
            ContentVersionId = entity.ContentVersionId,
            TokenHash = entity.TokenHash,
            ExpiresAt = entity.ExpiresAt,
            IssuedToEmail = entity.IssuedToEmail,
            OneTimeUse = entity.OneTimeUse,
            UsedAt = entity.UsedAt
        };
        
        MapAuditToRow(entity.Audit, row);
        return row;
    }

    protected override Expression<Func<PreviewTokenRow, bool>> CreateRowPredicate(Guid id)
    {
        return row => row.Id == id;
    }

    public async Task<PreviewToken?> GetByTokenHashAsync(Guid tenantId, string tokenHash)
    {
        var row = await DbSet
            .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.TokenHash == tokenHash && r.IsActive);
        return row != null ? MapToDomain(row) : null;
    }

    public async Task MarkUsedAsync(Guid tokenId)
    {
        var row = await DbSet.FindAsync(tokenId);
        if (row != null)
        {
            row.UsedAt = DateTime.UtcNow;
            row.IsActive = false;
        }
    }

    public async Task<int> CleanupExpiredAsync()
    {
        var expiredTokens = await DbSet
            .Where(r => r.ExpiresAt < DateTime.UtcNow && r.IsActive)
            .ToListAsync();
        
        foreach (var token in expiredTokens)
        {
            token.IsActive = false;
        }
        
        return expiredTokens.Count;
    }
}
