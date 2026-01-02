namespace TechWayFit.ContentOS.Content;

/// <summary>
/// Service for managing content items (CRUD operations)
/// </summary>
public interface IContentService
{
    Task<object?> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<object> CreateAsync(object content, CancellationToken cancellationToken = default);
    Task<object> UpdateAsync(string id, object content, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
