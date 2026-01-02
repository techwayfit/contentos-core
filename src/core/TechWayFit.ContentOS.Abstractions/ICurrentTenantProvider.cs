namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Provides access to current tenant context
/// Port/contract - implementations are runtime-specific
/// </summary>
public interface ICurrentTenantProvider
{
    /// <summary>
    /// Tenant ID for the current request
    /// </summary>
    Guid TenantId { get; }

    /// <summary>
    /// Site ID within the tenant
    /// </summary>
    Guid SiteId { get; }

    /// <summary>
    /// Environment name (e.g., "production", "staging")
    /// </summary>
    string Environment { get; }

    /// <summary>
    /// Default language for the tenant
    /// </summary>
    string DefaultLanguage { get; }

    /// <summary>
    /// Supported languages for the tenant
    /// </summary>
    string[] SupportedLanguages { get; }
}
