namespace TechWayFit.ContentOS.Search;

/// <summary>
/// Keyword-based search service for content discovery
/// </summary>
public interface ISearchService
{
    Task IndexAsync(string documentId, object document, CancellationToken cancellationToken = default);
    Task<object> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task DeleteFromIndexAsync(string documentId, CancellationToken cancellationToken = default);
}
