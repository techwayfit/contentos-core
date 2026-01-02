using Microsoft.EntityFrameworkCore;

namespace TechWayFit.ContentOS.Infrastructure.Persistence;

/// <summary>
/// Main database context for ContentOS.
/// Contains EF entities (DB models) and configurations.
/// </summary>
public class ContentOsDbContext : DbContext
{
    public ContentOsDbContext(DbContextOptions<ContentOsDbContext> options)
        : base(options)
    {
    }

    // DbSets will be added here as needed
    // public DbSet<ContentItemRow> ContentItems => Set<ContentItemRow>();
    // public DbSet<MediaAssetRow> MediaAssets => Set<MediaAssetRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContentOsDbContext).Assembly);

        // Global query filters for multi-tenancy can be added here
    }
}
