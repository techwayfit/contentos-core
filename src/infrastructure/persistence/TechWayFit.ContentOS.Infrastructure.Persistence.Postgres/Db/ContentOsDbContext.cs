using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Infrastructure.Persistence.Internal;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres;

/// <summary>
/// Main database context for ContentOS
/// Implements tenant isolation via global query filters
/// </summary>
public class ContentOsDbContext : DbContext
{
    private readonly ICurrentTenantProvider? _tenantProvider;

    public DbSet<ContentItemRow> ContentItems => Set<ContentItemRow>();
    public DbSet<ContentLocalizationRow> ContentLocalizations => Set<ContentLocalizationRow>();
    public DbSet<WorkflowStateRow> WorkflowStates => Set<WorkflowStateRow>();
    public DbSet<TenantRow> Tenants => Set<TenantRow>();

    public ContentOsDbContext(
        DbContextOptions options,
        ICurrentTenantProvider? tenantProvider = null) // Nullable for migrations
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContentOsDbContext).Assembly);

        // Global query filter for tenant isolation on ALL tenant-owned entities
        if (_tenantProvider?.TenantId != null)
        {
            // Apply filter to all entities implementing ITenantOwnedRow
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ITenantOwnedRow).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                    var tenantFilter = System.Linq.Expressions.Expression.Lambda(
                        System.Linq.Expressions.Expression.AndAlso(
                            System.Linq.Expressions.Expression.AndAlso(
                                System.Linq.Expressions.Expression.Equal(
                                    System.Linq.Expressions.Expression.Property(parameter, nameof(ITenantOwnedRow.TenantId)),
                                    System.Linq.Expressions.Expression.Constant(_tenantProvider.TenantId)),
                                System.Linq.Expressions.Expression.Equal(
                                    System.Linq.Expressions.Expression.Property(parameter, nameof(ITenantOwnedRow.SiteId)),
                                    System.Linq.Expressions.Expression.Constant(_tenantProvider.SiteId))),
                            System.Linq.Expressions.Expression.Equal(
                                System.Linq.Expressions.Expression.Property(parameter, nameof(ITenantOwnedRow.Environment)),
                                System.Linq.Expressions.Expression.Constant(_tenantProvider.Environment))),
                        parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(tenantFilter);
                }
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-stamp TenantId, SiteId, Environment on ALL new tenant-owned entities
        if (_tenantProvider != null && _tenantProvider.TenantId != Guid.Empty)
        {
            foreach (var entry in ChangeTracker.Entries<ITenantOwnedRow>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.TenantId = _tenantProvider.TenantId;
                    entry.Entity.SiteId = _tenantProvider.SiteId;
                    entry.Entity.Environment = _tenantProvider.Environment;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
