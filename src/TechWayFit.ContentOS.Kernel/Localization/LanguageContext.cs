namespace TechWayFit.ContentOS.Kernel.Localization;

/// <summary>
/// Represents language context for multi-language content operations.
/// This is a concrete class (not behind an interface) as it's a data container, not a behavior contract.
/// </summary>
public class LanguageContext
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
