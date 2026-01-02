using Microsoft.Extensions.DependencyInjection;
using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Infrastructure.Events;

/// <summary>
/// Extension methods for registering event infrastructure services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers the in-memory event bus implementation.
    /// Use this for single-instance deployments.
    /// For distributed scenarios, replace with a message broker implementation.
    /// </summary>
    public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services)
    {
        services.AddSingleton<IEventBus, InMemoryEventBus>();
        return services;
    }
}
