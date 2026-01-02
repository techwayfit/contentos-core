using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechWayFit.ContentOS.Infrastructure.Persistence;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres;

public static class DependencyInjection
{
    public static IServiceCollection AddPostgresPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new InvalidOperationException("PostgreSQL connection string is required");

        services.AddDbContext<ContentOsDbContext, PostgresDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(PostgresDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);
            }));

        // Register repositories (same as base, they work with DbContext)
        // services.AddScoped<IContentRepository, ContentRepository>();
        // services.AddScoped<IMediaRepository, MediaRepository>();

        return services;
    }
}
