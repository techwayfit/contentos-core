using Microsoft.Extensions.Logging;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Contracts.Events;

namespace TechWayFit.ContentOS.Infrastructure.Search.EventHandlers;

/// <summary>
/// Event handler that indexes published content for search.
/// Currently maintains in-memory index; will be replaced with Elasticsearch/etc.
/// This is infrastructure concern - implementation belongs in Infrastructure.Search.
/// </summary>
public sealed class ContentPublishedIndexer : IEventHandler<ContentPublishedEventV1>
{
    private readonly ILogger<ContentPublishedIndexer> _logger;

    public ContentPublishedIndexer(ILogger<ContentPublishedIndexer> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ContentPublishedEventV1 @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Indexing published content: {ContentId} for tenant {TenantId}, language {LanguageCode}",
            @event.ContentId,
            @event.TenantId,
            @event.LanguageCode);

        // TODO: Replace with actual search indexing (Elasticsearch, Azure Search, etc.)
        // For now, this is a stub that logs the event
        // Future implementation will:
        // - Extract searchable fields from ContentFields
        // - Build search document with title, slug, fields
        // - Index to search backend
        // - Handle tenant isolation in search queries

        return Task.CompletedTask;
    }
}
