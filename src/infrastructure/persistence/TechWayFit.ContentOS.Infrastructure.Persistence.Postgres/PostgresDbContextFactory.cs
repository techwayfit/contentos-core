using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres;

/// <summary>
/// Design-time factory for creating PostgresDbContext instances during migrations.
/// This is only used by EF Core tools (dotnet ef) and not at runtime.
/// Reads connection string from appsettings.json in the API project.
/// </summary>
public class PostgresDbContextFactory : IDesignTimeDbContextFactory<PostgresDbContext>
{
    public PostgresDbContext CreateDbContext(string[] args)
    {
        // Build configuration to read from API project's appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TechWayFit.ContentOS.Api"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile("appsettings.local.json", optional: true) // Local credentials, not in git
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<PostgresDbContext>();
        
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("PostgreSQL connection string not found in appsettings.json");
        
        optionsBuilder.UseNpgsql(connectionString);

        return new PostgresDbContext(optionsBuilder.Options);
    }
}
