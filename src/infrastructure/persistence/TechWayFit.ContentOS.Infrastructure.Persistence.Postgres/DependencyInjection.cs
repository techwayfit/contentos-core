using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Repositories;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgresPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? configuration.GetConnectionString("ContentOsDb")
            ?? throw new InvalidOperationException("PostgreSQL connection string is required");

        // Register PostgresDbContext (which inherits from ContentOsDbContext)
        services.AddDbContext<PostgresDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(PostgresDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            }));
        
        // Register ContentOsDbContext as an alias to PostgresDbContext
        services.AddScoped<ContentOsDbContext>(sp => sp.GetRequiredService<PostgresDbContext>());

        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Register repositories with Postgres-specific implementations
        services.AddScoped<ITenantRepository, PostgresTenantRepository>();

        return services;
    }
}

