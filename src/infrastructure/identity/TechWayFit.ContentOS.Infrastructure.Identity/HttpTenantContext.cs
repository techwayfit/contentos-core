using Microsoft.AspNetCore.Http;
using TechWayFit.ContentOS.Abstractions.Security;

namespace TechWayFit.ContentOS.Infrastructure.Identity;

/// <summary>
/// HTTP-based implementation of ITenantContext
/// Reads tenant information from request headers
/// </summary>
public sealed class HttpTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _cachedTenantId;
    private string? _cachedTenantKey;

    public HttpTenantContext(IHttpContextAccessor httpContextAccessor)
    {
     _httpContextAccessor = httpContextAccessor;
    }

    public Guid CurrentTenantId
    {
   get
    {
      if (_cachedTenantId.HasValue)
                return _cachedTenantId.Value;

    var tenantIdHeader = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"]
   .FirstOrDefault();

   if (Guid.TryParse(tenantIdHeader, out var tenantId))
    {
            _cachedTenantId = tenantId;
                return tenantId;
  }

       throw new InvalidOperationException("Tenant context not resolved. X-Tenant-Id header is required.");
   }
    }

    public string? CurrentTenantKey
 {
   get
    {
     if (!string.IsNullOrEmpty(_cachedTenantKey))
         return _cachedTenantKey;

  var tenantKey = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Key"]
            .FirstOrDefault();

            if (!string.IsNullOrEmpty(tenantKey))
     {
              _cachedTenantKey = tenantKey;
      return tenantKey;
}

  return null;
  }
  }

    public bool IsResolved => _cachedTenantId.HasValue;
}
