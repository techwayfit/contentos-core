using Microsoft.Extensions.DependencyInjection;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel.Events;
using TechWayFit.ContentOS.Kernel.Localization;
using TechWayFit.ContentOS.Kernel.Security;
using TechWayFit.ContentOS.Kernel.Tenancy;

namespace TechWayFit.ContentOS.Kernel;

public static class DependencyInjection
{
    public static IServiceCollection AddKernelServices(this IServiceCollection services)
    {
        // Register core platform services
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<ILanguageContext, LanguageContext>();
        
        // Register event bus as singleton for in-memory implementation
        services.AddSingleton<IEventBus, InMemoryEventBus>();

        return services;
    }
}
