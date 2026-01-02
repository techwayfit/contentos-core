using Microsoft.Extensions.Logging;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Contracts.Dtos;

namespace TechWayFit.ContentOS.Media.Application;

/// <summary>
/// Stub implementation that returns fake media metadata.
/// Will be replaced with actual blob storage integration (Azure Blob, S3, etc.)
/// </summary>
public sealed class GetMediaMetadataUseCase : IGetMediaMetadataUseCase
{
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<GetMediaMetadataUseCase> _logger;

    public GetMediaMetadataUseCase(
        ITenantContext tenantContext,
        ILogger<GetMediaMetadataUseCase> logger)
    {
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public Task<Result<MediaMetadataResponse, Error>> ExecuteAsync(
        Guid mediaId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Fetching media metadata for {MediaId} in tenant {TenantId}",
            mediaId,
            _tenantContext.TenantId);

        // TODO: Replace with actual media storage lookup
        // Future implementation will:
        // - Query blob storage for metadata
        // - Validate tenant ownership
        // - Generate signed URLs for secure access
        // - Handle CDN integration

        var response = new MediaMetadataResponse(
            MediaId: mediaId,
            FileName: "sample-image.jpg",
            ContentType: "image/jpeg",
            SizeInBytes: 1024000,
            Url: $"https://cdn.example.com/media/{mediaId}",
            CreatedAt: DateTime.UtcNow.AddDays(-7));

        return Task.FromResult<Result<MediaMetadataResponse, Error>>(new Result<MediaMetadataResponse, Error>.Success(response));
    }
}
