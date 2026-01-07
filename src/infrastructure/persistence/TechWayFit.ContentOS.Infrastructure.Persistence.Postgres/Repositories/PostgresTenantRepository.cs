using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
using TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Core;
using TechWayFit.ContentOS.Tenancy.Domain;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Repositories;

/// <summary>
/// PostgreSQL-specific implementation of TenantRepository
/// Inherits from the provider-agnostic TenantRepository and adds Postgres-specific optimizations
/// </summary>
public sealed class PostgresTenantRepository : TenantRepository
{
    public PostgresTenantRepository(DbContext context) : base(context)
    {
    }

    // Example: Override GetByKeyAsync to use Postgres-specific case-insensitive search
    public override async Task<Tenant?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        // Use Postgres ILIKE operator for case-insensitive comparison
        // This is more efficient than ToLower() which forces a function scan
        var row = await Context.Set<TenantRow>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => EF.Functions.ILike(t.Key, key), cancellationToken);

        return row == null ? null : MapToDomain(row);
    }

    // Example: Could add Postgres-specific full-text search
    // public async Task<IReadOnlyList<Tenant>> FullTextSearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    // {
    //     var rows = await Context.Set<TenantRow>()
    //         .Where(t => EF.Functions.ToTsVector("english", t.Name + " " + t.Key)
    //             .Matches(EF.Functions.PlainToTsQuery("english", searchTerm)))
    //         .ToListAsync(cancellationToken);
    //     
    //     return rows.Select(MapToDomain).ToList();
    // }
}
