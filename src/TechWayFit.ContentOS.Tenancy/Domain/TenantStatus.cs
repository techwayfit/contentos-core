namespace TechWayFit.ContentOS.Tenancy.Domain;

/// <summary>
/// Tenant status enumeration
/// </summary>
public enum TenantStatus
{
    /// <summary>
    /// Tenant is active and can be used
    /// </summary>
    Active = 0,

    /// <summary>
    /// Tenant is disabled and cannot be used
    /// </summary>
    Disabled = 1
}
