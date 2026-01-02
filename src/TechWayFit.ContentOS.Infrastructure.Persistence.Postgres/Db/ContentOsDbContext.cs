using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Infrastructure.Persistence.Contracts;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres;

/// <summary>
/// Main database context for ContentOS
/// Implements tenant isolation via global query filters
/// </summary>
public class ContentOsDbContext : DbContext
{
    private readonly ITenantContext? _tenantContext;

    public DbSet<ContentItemRow> ContentItems => Set<ContentItemRow>();
    public DbSet<ContentLocalizationRow> ContentLocalizations => Set<ContentLocalizationRow>();
    public DbSet<WorkflowStateRow> WorkflowStates => Set<WorkflowStateRow>();
    public DbSet<TenantRow> Tenants => Set<TenantRow>();

    public ContentOsDbContext(
        DbContextOptions options,
        ITenantContext? tenantContext = null) // Nullable for migrations
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContentOsDbContext).Assembly);

        // Global query filter for tenant isolation on ALL tenant-owned entities
        if (_tenantContext != null)
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
                                    System.Linq.Expressions.Expression.Constant(_tenantContext.TenantId)),
                                System.Linq.Expressions.Expression.Equal(
                                    System.Linq.Expressions.Expression.Property(parameter, nameof(ITenantOwnedRow.SiteId)),
                                    System.Linq.Expressions.Expression.Constant(_tenantContext.SiteId))),
                            System.Linq.Expressions.Expression.Equal(
                                System.Linq.Expressions.Expression.Property(parameter, nameof(ITenantOwnedRow.Environment)),
                                System.Linq.Expressions.Expression.Constant(_tenantContext.Environment))),
                        parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(tenantFilter);
                }
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-stamp TenantId, SiteId, Environment on ALL new tenant-owned entities
        if (_tenantContext != null)
        {
            foreach (var entry in ChangeTracker.Entries<ITenantOwnedRow>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.TenantId = _tenantContext.TenantId;
                    entry.Entity.SiteId = _tenantContext.SiteId;
                    entry.Entity.Environment = _tenantContext.Environment;
                }
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
