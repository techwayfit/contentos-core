namespace TechWayFit.ContentOS.Kernel.Security;

/// <summary>
/// Context for SuperAdmin scope enforcement
/// MVP: Reads X-SuperAdmin header
/// Later: JWT claim validation
/// </summary>
public interface ISuperAdminContext
{
    /// <summary>
    /// True if current request has SuperAdmin privileges
    /// </summary>
    bool IsSuperAdmin { get; }
}
