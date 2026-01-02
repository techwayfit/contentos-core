namespace TechWayFit.ContentOS.Abstractions.Security;

/// <summary>
/// Role-based access control (RBAC) authorization service
/// </summary>
public interface IRbacService
{
    Task<bool> HasPermissionAsync(string userId, string resource, string action, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
    Task RevokeRoleAsync(string userId, string role, CancellationToken cancellationToken = default);
}
