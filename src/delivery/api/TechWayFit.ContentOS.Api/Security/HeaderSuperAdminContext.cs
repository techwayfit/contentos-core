using TechWayFit.ContentOS.Abstractions.Security;

namespace TechWayFit.ContentOS.Api.Security;

/// <summary>
/// MVP SuperAdmin context - reads X-SuperAdmin header
/// Later: Replace with JWT claim validation
/// </summary>
public sealed class HeaderSuperAdminContext : ISuperAdminContext
{
    public bool IsSuperAdmin { get; }

    public HeaderSuperAdminContext(bool isSuperAdmin)
    {
        IsSuperAdmin = isSuperAdmin;
    }
}
