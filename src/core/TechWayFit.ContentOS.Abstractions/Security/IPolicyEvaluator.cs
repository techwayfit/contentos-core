namespace TechWayFit.ContentOS.Abstractions.Security;

/// <summary>
/// Permission-based policy evaluator for authorization
/// </summary>
public interface IPolicyEvaluator
{
    /// <summary>
    /// Require a specific permission, throw if not authorized
    /// </summary>
    Task RequireAsync(string permission, CancellationToken cancellationToken = default);
}
