using Microsoft.Extensions.DependencyInjection;

namespace TechWayFit.ContentOS.Kernel;

/// <summary>
/// Extension methods for registering Kernel services.
/// Kernel now contains only primitives and domain-agnostic types.
/// Runtime context has been moved to API layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddKernelServices(this IServiceCollection services)
    {
        // Kernel now contains only primitives and domain constants
        // No runtime services registered here anymore
        return services;
    }
}
