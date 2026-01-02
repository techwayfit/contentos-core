using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Infrastructure.Runtime;

/// <summary>
/// Mutable runtime context for language/locale settings.
/// Typically set by middleware based on Accept-Language header.
/// This is NOT a domain concept - it's infrastructure/runtime state.
/// Implements ICurrentLocaleProvider to allow feature projects to depend only on abstractions.
/// </summary>
public class LanguageContext : ICurrentLocaleProvider
{
    public string CurrentLanguage { get; private set; } = "en-US";
    public string DefaultLanguage { get; private set; } = "en-US";
    public IReadOnlyList<string> AcceptedLanguages { get; private set; } = new[] { "en-US" };

    /// <summary>
    /// Sets the language context (called from middleware based on Accept-Language header)
    /// </summary>
    public void SetLanguage(
        string currentLanguage,
        string defaultLanguage,
        IReadOnlyList<string>? acceptedLanguages = null)
    {
        CurrentLanguage = currentLanguage;
        DefaultLanguage = defaultLanguage;
        AcceptedLanguages = acceptedLanguages ?? new[] { currentLanguage };
    }
}
