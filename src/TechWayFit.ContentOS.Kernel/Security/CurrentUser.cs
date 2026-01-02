using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Kernel.Security;

/// <summary>
/// Implementation of current user context
/// </summary>
public class CurrentUser : ICurrentUser
{
    public Guid? UserId { get; private set; }
    public string? Username { get; private set; }
    public bool IsAuthenticated => UserId.HasValue;
    public IReadOnlyList<string> Roles { get; private set; } = Array.Empty<string>();
    public IReadOnlyList<string> Permissions { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Sets the current user context (called from middleware/auth handler)
    /// </summary>
    public void SetUser(
        Guid? userId,
        string? username,
        IReadOnlyList<string>? roles = null,
        IReadOnlyList<string>? permissions = null)
    {
        UserId = userId;
        Username = username;
        Roles = roles ?? Array.Empty<string>();
        Permissions = permissions ?? Array.Empty<string>();
    }
}
