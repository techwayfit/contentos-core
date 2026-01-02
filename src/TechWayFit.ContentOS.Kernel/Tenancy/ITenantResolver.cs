using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Kernel.Tenancy;

/// <summary>
/// Manages tenant context resolution and isolation
/// </summary>
public interface ITenantResolver
{
    Task<ITenantContext> ResolveAsync(CancellationToken cancellationToken = default);
}
