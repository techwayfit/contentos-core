using TechWayFit.ContentOS.Abstractions;

namespace TechWayFit.ContentOS.Kernel.Localization;

/// <summary>
/// Implementation of language context
/// </summary>
public class LanguageContext : ILanguageContext
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
