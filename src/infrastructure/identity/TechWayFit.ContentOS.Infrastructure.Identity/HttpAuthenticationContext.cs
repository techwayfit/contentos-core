using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TechWayFit.ContentOS.Abstractions.Security;

namespace TechWayFit.ContentOS.Infrastructure.Identity;

/// <summary>
/// HTTP-based implementation of IAuthenticationContext
/// Reads authentication data from HttpContext claims
/// </summary>
public sealed class HttpAuthenticationContext : IAuthenticationContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpAuthenticationContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? CurrentUserId
    {
    get
        {
     var userIdClaim = _httpContextAccessor.HttpContext?.User
     .FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
    return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public string? CurrentUserEmail => 
        _httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.Email)?.Value;

    public IReadOnlyList<string> Roles =>
        _httpContextAccessor.HttpContext?.User
  .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
     .ToList() ?? new List<string>();

    public IReadOnlyList<string> Permissions =>
        _httpContextAccessor.HttpContext?.User
    .FindAll("permission")
        .Select(c => c.Value)
        .ToList() ?? new List<string>();

    public bool IsSuperAdmin =>
      _httpContextAccessor.HttpContext?.Request.Headers["X-SuperAdmin"]
      .FirstOrDefault() == "true"; // MVP: Header-based

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
