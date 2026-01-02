using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TechWayFit.ContentOS.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=contentos.db";

        services.AddDbContext<ContentOsDbContext>(options =>
            options.UseSqlite(connectionString));

        // Register repositories here
        // services.AddScoped<IContentRepository, ContentRepository>();
        // services.AddScoped<IMediaRepository, MediaRepository>();

        return services;
    }
}
