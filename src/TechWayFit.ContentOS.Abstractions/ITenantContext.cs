namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Provides tenant isolation context for multi-tenant operations
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Unique identifier of the current tenant (organization)
    /// </summary>
    Guid TenantId { get; }

    /// <summary>
    /// Unique identifier of the current site (brand/division within tenant)
    /// </summary>
    Guid SiteId { get; }

    /// <summary>
    /// Current environment (e.g., "production", "staging", "development")
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
