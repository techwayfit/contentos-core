namespace TechWayFit.ContentOS.Kernel.Localization;

/// <summary>
/// Immutable value object representing a language/locale code (e.g., "en-US", "fr-FR")
/// This is a domain primitive - represents business meaning, not runtime state.
/// </summary>
public record LanguageCode
{
    public string Value { get; }

    public LanguageCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Language code cannot be empty", nameof(value));

        // Basic validation: should be in format like "en-US" or "en"
        if (!IsValidFormat(value))
            throw new ArgumentException($"Invalid language code format: {value}", nameof(value));

        Value = value;
    }

    private static bool IsValidFormat(string value)
    {
        // Simple validation: 2-letter or 2-letter + dash + 2-letter
        var parts = value.Split('-');
        return parts.Length is 1 or 2 && parts.All(p => p.Length >= 2);
    }

    public static implicit operator string(LanguageCode languageCode) => languageCode.Value;
    public static explicit operator LanguageCode(string value) => new(value);

    public override string ToString() => Value;
}
