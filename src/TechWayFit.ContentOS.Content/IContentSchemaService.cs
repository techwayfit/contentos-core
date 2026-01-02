namespace TechWayFit.ContentOS.Content;

/// <summary>
/// Defines and manages content type schemas
/// </summary>
public interface IContentSchemaService
{
    Task<object?> GetSchemaAsync(string contentType, CancellationToken cancellationToken = default);
    Task<object> DefineSchemaAsync(string contentType, object schema, CancellationToken cancellationToken = default);
}
