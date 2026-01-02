using TechWayFit.ContentOS.Api.Context;
using TechWayFit.ContentOS.Infrastructure.Runtime;
using TechWayFit.ContentOS.Api.Security;
using TechWayFit.ContentOS.Api.Tenancy;

namespace TechWayFit.ContentOS.Api.Middleware;

/// <summary>
/// Middleware to initialize and populate ApiRequestContext for each request.
/// Must be registered early in the pipeline to ensure context is available for all subsequent middleware.
/// </summary>
public class ApiRequestContextMiddleware
{
    private readonly RequestDelegate _next;

    public ApiRequestContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        CurrentUser? currentUser = null,
        TenantContext? tenantContext = null,
        LanguageContext? languageContext = null)
    {
        try
        {
            // Generate or retrieve correlation ID
            var correlationId = httpContext.Request.Headers["X-Correlation-Id"].FirstOrDefault()
                ?? Guid.NewGuid().ToString();

            // Add correlation ID to response headers for tracing
            httpContext.Response.Headers["X-Correlation-Id"] = correlationId;

            // Get client IP address
            var clientIp = httpContext.Connection.RemoteIpAddress?.ToString()
                ?? httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();

            // Get user agent
            var userAgent = httpContext.Request.Headers["User-Agent"].FirstOrDefault();

            // Create and initialize the request context
            var requestContext = ApiRequestContext.Create(
                correlationId,
                httpContext.Request.Method,
                httpContext.Request.Path.Value ?? "/",
                clientIp,
                userAgent);

            // Populate context from resolved services
            requestContext.SetCurrentUser(currentUser);
            requestContext.SetTenantContext(tenantContext);
            requestContext.SetLanguageContext(languageContext);

            // Check for SuperAdmin header (MVP implementation)
            var isSuperAdmin = httpContext.Request.Headers["X-SuperAdmin"].FirstOrDefault() == "true";
            requestContext.SetSuperAdmin(isSuperAdmin);

            // Store in HttpContext for easy access
            httpContext.Items["ApiRequestContext"] = requestContext;

            await _next(httpContext);
        }
        finally
        {
            // Clear AsyncLocal context after request completes
            ApiRequestContext.Clear();
        }
    }
}
