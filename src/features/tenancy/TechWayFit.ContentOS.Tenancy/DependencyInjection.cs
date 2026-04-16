using Microsoft.Extensions.DependencyInjection;
using TechWayFit.ContentOS.Tenancy.Application;
using TechWayFit.ContentOS.Tenancy.Application.Tenants;

namespace TechWayFit.ContentOS.Tenancy;

public static class DependencyInjection
{
    public static IServiceCollection AddTenancy(this IServiceCollection services)
    {
        // Register use cases
        services.AddScoped<CreateTenantUseCase>();
        services.AddScoped<UpdateTenantUseCase>();
        services.AddScoped<GetTenantUseCase>();
        services.AddScoped<ListTenantsUseCase>();

        return services;
    }
}
