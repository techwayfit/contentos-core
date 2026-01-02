namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Provides access to current authenticated user information
/// This is a runtime context provider - implementation lives in Infrastructure/API
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>
    /// User ID if authenticated, otherwise null
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Username if authenticated, otherwise null
    /// </summary>
    string? Username { get; }

    /// <summary>
    /// True if user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// User's assigned roles
    /// </summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>
    /// User's explicit permissions
    /// </summary>
    IReadOnlyList<string> Permissions { get; }
}
