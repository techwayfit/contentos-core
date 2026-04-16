using Microsoft.AspNetCore.Http;

namespace TechWayFit.ContentOS.Api.Middleware;

/// <summary>
/// Resolves tenant context from request headers
/// Validates tenant ID is present for non-admin routes
/// </summary>
public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
  _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();

        // Skip tenant resolution for:
        // - Health checks
        // - Swagger/OpenAPI
     // - Admin endpoints (SuperAdmin only)
  // - Root endpoint
        if (path == "/" ||
       path?.StartsWith("/health") == true ||
     path?.StartsWith("/swagger") == true ||
    path?.StartsWith("/api/admin") == true)
        {
      await _next(context);
    return;
        }

        // Require tenant header for all other endpoints
  var tenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        
  if (string.IsNullOrEmpty(tenantId))
        {
    context.Response.StatusCode = StatusCodes.Status400BadRequest;
       await context.Response.WriteAsJsonAsync(new
     {
        error = "Missing required header: X-Tenant-Id"
          });
      return;
  }

        if (!Guid.TryParse(tenantId, out _))
        {
    context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new
  {
       error = "Invalid X-Tenant-Id format. Must be a valid GUID."
        });
            return;
        }

        // Tenant ID is valid, continue processing
        await _next(context);
    }
}
