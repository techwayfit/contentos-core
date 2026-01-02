using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Kernel.Tenancy;

namespace TechWayFit.ContentOS.Api.Middleware;

/// <summary>
/// Middleware to resolve tenant context from HTTP headers
/// Skips /api/admin routes (SuperAdmin scope doesn't require tenant)
/// </summary>
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Skip tenant resolution for admin routes
        if (path.StartsWith("/api/admin", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Skip tenant resolution for root/health endpoints
        if (path == "/" || path.StartsWith("/health", StringComparison.OrdinalIgnoreCase) || 
            path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Read tenant headers
        var tenantIdHeader = context.Request.Headers["X-Tenant-Id"].ToString();
        var siteIdHeader = context.Request.Headers["X-Site-Id"].ToString();
        var environmentHeader = context.Request.Headers["X-Environment"].ToString();

        // Validate tenant headers are present for non-admin routes
        if (string.IsNullOrEmpty(tenantIdHeader))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { error = "X-Tenant-Id header is required" });
            return;
        }

        if (!Guid.TryParse(tenantIdHeader, out var tenantId))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { error = "X-Tenant-Id must be a valid GUID" });
            return;
        }

        // Site and environment are optional, default to tenant's default site
        var siteId = Guid.TryParse(siteIdHeader, out var parsedSiteId) ? parsedSiteId : Guid.NewGuid();
        var environment = string.IsNullOrEmpty(environmentHeader) ? "production" : environmentHeader;

        // Set tenant context for the request scope (cast to concrete type)
        if (tenantContext is TenantContext concreteContext)
        {
            concreteContext.SetContext(
                tenantId,
                siteId,
                environment,
                "en-US", // TODO: Load from tenant configuration
                new[] { "en-US" }); // TODO: Load from tenant configuration
        }

        await _next(context);
    }
}
