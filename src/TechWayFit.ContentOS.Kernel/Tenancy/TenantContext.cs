namespace TechWayFit.ContentOS.Kernel.Tenancy;

/// <summary>
/// Represents tenant isolation context for multi-tenant operations.
/// This is a concrete class (not behind an interface) as it's a data container, not a behavior contract.
/// </summary>
public class TenantContext
{
    public Guid TenantId { get; private set; }
    public Guid SiteId { get; private set; }
    public string Environment { get; private set; } = string.Empty;
    public string DefaultLanguage { get; private set; } = "en-US";
    public string[] SupportedLanguages { get; private set; } = Array.Empty<string>();

    /// <summary>
    /// Sets the tenant context (called by tenant resolver)
    /// </summary>
    public void SetContext(
        Guid tenantId,
        Guid siteId,
        string environment,
        string defaultLanguage,
        string[] supportedLanguages)
    {
        TenantId = tenantId;
        SiteId = siteId;
        Environment = environment;
        DefaultLanguage = defaultLanguage;
        SupportedLanguages = supportedLanguages;
    }
}
