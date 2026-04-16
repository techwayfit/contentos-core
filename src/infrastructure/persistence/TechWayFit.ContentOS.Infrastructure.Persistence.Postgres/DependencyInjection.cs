using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Repositories;
using TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Content;
using TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Core;
using TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Identity;
using TechWayFit.ContentOS.Content.Ports.Core;
using TechWayFit.ContentOS.Content.Ports.Hierarchy;
using TechWayFit.ContentOS.Tenancy.Ports;
using TechWayFit.ContentOS.Tenancy.Ports.Core;
using TechWayFit.ContentOS.Tenancy.Ports.Identity;

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

        // CRITICAL: Register DbContext for repositories that expect DbContext parameter
        // This allows the base persistence layer repositories to work with any DbContext implementation
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<PostgresDbContext>());

        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // =============================================================
        // TENANCY REPOSITORIES
        // =============================================================
      
        // Register Tenant repository
        services.AddScoped<ITenantRepository, PostgresTenantRepository>();
        
        // Register Site repository
        services.AddScoped<ISiteRepository, SiteRepository>();
    
        // Register Identity repositories (Users, Roles, Groups)
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUserGroupRepository, UserGroupRepository>();

        // =============================================================
        // CONTENT REPOSITORIES
        // =============================================================
 
        services.AddScoped<IContentTypeRepository, ContentTypeRepository>();
        services.AddScoped<IContentTypeFieldRepository, ContentTypeFieldRepository>();
        services.AddScoped<IContentItemRepository, ContentItemRepository>();
        services.AddScoped<IContentVersionRepository, ContentVersionRepository>();
        services.AddScoped<IContentFieldValueRepository, ContentFieldValueRepository>();
        services.AddScoped<IContentNodeRepository, ContentNodeRepository>();
        services.AddScoped<IRouteRepository, RouteRepository>();

        return services;
    }
}

