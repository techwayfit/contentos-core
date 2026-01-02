namespace TechWayFit.ContentOS.Kernel.Security;

/// <summary>
/// Platform-level admin permissions (SuperAdmin scope)
/// </summary>
public static class AdminPermissions
{
    /// <summary>
    /// Platform superadmin - full system access
    /// </summary>
    public const string SuperAdmin = "platform:superadmin";

    /// <summary>
    /// Tenant management - create/update/delete tenants
    /// </summary>
    public const string TenantManage = "tenant:manage";
}
