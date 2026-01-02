using TechWayFit.ContentOS.Api.Context;

namespace TechWayFit.ContentOS.Api.Middleware;

/// <summary>
/// Extension methods for ApiRequestContext
/// </summary>
public static class ApiRequestContextExtensions
{
    /// <summary>
    /// Registers ApiRequestContext middleware
    /// Should be called early in the pipeline
    /// </summary>
    public static IApplicationBuilder UseApiRequestContext(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ApiRequestContextMiddleware>();
    }

    /// <summary>
    /// Registers ApiRequestContext as a scoped service for DI.
    /// Note: Prefer using static ApiRequestContext.Current for direct access.
    /// </summary>
    public static IServiceCollection AddApiRequestContext(this IServiceCollection services)
    {
        services.AddScoped(sp =>
        {
            // Return current AsyncLocal context or throw - context should always exist in a request
            return ApiRequestContext.Current 
                ?? throw new InvalidOperationException("ApiRequestContext not initialized. Ensure ApiRequestContextMiddleware is registered.");
        });

        return services;
    }

    /// <summary>
    /// Gets the ApiRequestContext from HttpContext items
    /// </summary>
    public static ApiRequestContext? GetApiRequestContext(this HttpContext httpContext)
    {
        return httpContext.Items.TryGetValue("ApiRequestContext", out var context)
            ? context as ApiRequestContext
            : null;
    }

    /// <summary>
    /// Gets the current ApiRequestContext from AsyncLocal storage
    /// </summary>
    public static ApiRequestContext? GetCurrentApiRequestContext()
    {
        return ApiRequestContext.Current;
    }
}
