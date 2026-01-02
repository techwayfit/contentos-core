namespace TechWayFit.ContentOS.Abstractions;

/// <summary>
/// Provides language context for multi-language content operations
/// </summary>
public interface ILanguageContext
{
    /// <summary>
    /// Current language code for the request (e.g., "en-US", "fr-FR")
    /// </summary>
    string CurrentLanguage { get; }

    /// <summary>
    /// Default/fallback language for the tenant
    /// </summary>
    string DefaultLanguage { get; }

    /// <summary>
    /// Ordered list of accepted languages based on Accept-Language header
    /// </summary>
    IReadOnlyList<string> AcceptedLanguages { get; }
}
