namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Authorization policy abstraction for RBAC enforcement
/// </summary>
public interface IAuthorizationPolicy
{
    Task<bool> AuthorizeAsync(string userId, string resource, string action, CancellationToken cancellationToken = default);
}
