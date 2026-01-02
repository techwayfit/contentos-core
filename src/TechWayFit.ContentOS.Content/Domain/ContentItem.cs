namespace TechWayFit.ContentOS.Content.Domain;

/// <summary>
/// Content item aggregate root
/// Represents language-agnostic content shell with localizations
/// </summary>
public sealed class ContentItem
{
    public ContentItemId Id { get; private set; }
    public TenantId TenantId { get; private set; }
    public SiteId SiteId { get; private set; }
    public ContentType ContentType { get; private set; }
    public LanguageCode DefaultLanguage { get; private set; }
    public WorkflowStatus Status { get; private set; }

    private readonly List<ContentLocalization> _localizations = new();
    public IReadOnlyList<ContentLocalization> Localizations => _localizations.AsReadOnly();

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public UserId? CreatedBy { get; private set; }
    public UserId? UpdatedBy { get; private set; }

    // Private constructor for EF Core
    private ContentItem()
    {
        Id = null!;
        TenantId = null!;
        SiteId = null!;
        ContentType = null!;
        DefaultLanguage = null!;
    }

    /// <summary>
    /// Factory method to create a new content item
    /// </summary>
    public static ContentItem Create(
        TenantId tenantId,
        SiteId siteId,
        ContentType contentType,
        LanguageCode defaultLanguage,
        UserId createdBy)
    {
        var item = new ContentItem
        {
            Id = ContentItemId.New(),
            TenantId = tenantId,
            SiteId = siteId,
            ContentType = contentType,
            DefaultLanguage = defaultLanguage,
            Status = WorkflowStatus.Draft,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = createdBy
        };

        return item;
    }

    /// <summary>
    /// Add a new localization to the content
    /// </summary>
    public void AddLocalization(ContentLocalization localization)
    {
        if (localization == null)
            throw new ArgumentNullException(nameof(localization));

        if (_localizations.Any(l => l.LanguageCode.Value == localization.LanguageCode.Value))
            throw new InvalidOperationException($"Localization for language '{localization.LanguageCode.Value}' already exists");

        _localizations.Add(localization);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Get localization for a specific language with fallback to default
    /// </summary>
    public ContentLocalization? GetLocalization(LanguageCode languageCode)
    {
        // Try exact match first
        var localization = _localizations.FirstOrDefault(l => l.LanguageCode.Value == languageCode.Value);

        // Fallback to default language if not found
        if (localization == null && languageCode.Value != DefaultLanguage.Value)
        {
            localization = _localizations.FirstOrDefault(l => l.LanguageCode.Value == DefaultLanguage.Value);
        }

        return localization;
    }

    /// <summary>
    /// Get all available language codes
    /// </summary>
    public IReadOnlyList<string> GetAvailableLanguages()
    {
        return _localizations.Select(l => l.LanguageCode.Value).ToList();
    }

    /// <summary>
    /// Change workflow status
    /// </summary>
    public void ChangeStatus(WorkflowStatus newStatus, UserId userId)
    {
        Status = newStatus;
        UpdatedBy = userId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Rehydrate from persistence (for mappers)
    /// </summary>
    public static ContentItem Rehydrate(
        ContentItemId id,
        TenantId tenantId,
        SiteId siteId,
        ContentType contentType,
        LanguageCode defaultLanguage,
        WorkflowStatus status,
        List<ContentLocalization> localizations,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        UserId? createdBy,
        UserId? updatedBy)
    {
        var item = new ContentItem
        {
            Id = id,
            TenantId = tenantId,
            SiteId = siteId,
            ContentType = contentType,
            DefaultLanguage = defaultLanguage,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            CreatedBy = createdBy,
            UpdatedBy = updatedBy
        };

        foreach (var localization in localizations)
        {
            item._localizations.Add(localization);
        }

        return item;
    }
}
