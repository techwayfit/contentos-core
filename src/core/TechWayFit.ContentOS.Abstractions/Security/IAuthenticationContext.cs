namespace TechWayFit.ContentOS.Abstractions.Security;

/// <summary>
/// Provides access to the current authenticated user context
/// This is a runtime context provider - implementation lives in Infrastructure.Identity
/// </summary>
public interface IAuthenticationContext
{
    /// <summary>
    /// Current user ID (null if not authenticated)
    /// </summary>
    Guid? CurrentUserId { get; }
    
    /// <summary>
    /// Current user email
    /// </summary>
    string? CurrentUserEmail { get; }
    
    /// <summary>
    /// Current user roles
  /// </summary>
    IReadOnlyList<string> Roles { get; }
    
    /// <summary>
    /// Current user permissions/scopes
    /// </summary>
    IReadOnlyList<string> Permissions { get; }
    
    /// <summary>
    /// Whether user is SuperAdmin
    /// </summary>
    bool IsSuperAdmin { get; }
    
    /// <summary>
    /// Whether user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
}
