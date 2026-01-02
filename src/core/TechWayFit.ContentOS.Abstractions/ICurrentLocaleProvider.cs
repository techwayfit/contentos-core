namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Provides access to current request's locale/language context
/// This is a runtime context provider - implementation lives in Infrastructure
/// </summary>
public interface ICurrentLocaleProvider
{
    /// <summary>
    /// Gets the current language for this request
    /// </summary>
    string CurrentLanguage { get; }

    /// <summary>
    /// Gets the default/fallback language for the tenant
    /// </summary>
    string DefaultLanguage { get; }

    /// <summary>
    /// Gets all accepted languages for content negotiation (from Accept-Language header)
    /// </summary>
    IReadOnlyList<string> AcceptedLanguages { get; }
}
