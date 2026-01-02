using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Infrastructure.Persistence;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres;

/// <summary>
/// PostgreSQL-specific database context extending the base ContentOsDbContext.
/// Enables PostgreSQL-specific features and optimizations.
/// </summary>
public class PostgresDbContext : ContentOsDbContext
{
    public PostgresDbContext(DbContextOptions<ContentOsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PostgreSQL-specific configurations
        // Example: Use PostgreSQL schemas for multi-tenancy
        // modelBuilder.HasDefaultSchema("public");
        
        // Example: Enable PostgreSQL full-text search
        // modelBuilder.Entity<ContentItemRow>()
        //     .HasGeneratedTsVectorColumn(
        //         p => p.SearchVector,
        //         "english",
        //         p => new { p.Title, p.Body });
    }
}
