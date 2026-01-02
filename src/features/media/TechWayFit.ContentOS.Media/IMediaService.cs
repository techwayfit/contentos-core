namespace TechWayFit.ContentOS.Media;

/// <summary>
/// Digital Asset Management service for media files
/// </summary>
public interface IMediaService
{
    Task<object> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
    Task<Stream?> DownloadAsync(string assetId, CancellationToken cancellationToken = default);
    Task DeleteAsync(string assetId, CancellationToken cancellationToken = default);
    Task<object?> GetMetadataAsync(string assetId, CancellationToken cancellationToken = default);
}
