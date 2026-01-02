namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Provides access to the current authenticated user context
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Unique identifier of the current user
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Username or email of the current user
    /// </summary>
    string? Username { get; }

    /// <summary>
    /// Indicates whether a user is currently authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Roles assigned to the current user
    /// </summary>
    IReadOnlyList<string> Roles { get; }

    /// <summary>
    /// Permissions granted to the current user
    /// </summary>
    IReadOnlyList<string> Permissions { get; }
}
