using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Contracts.Dtos;

namespace TechWayFit.ContentOS.Media.Application;

public interface IGetMediaMetadataUseCase
{
    Task<Result<MediaMetadataResponse, Error>> ExecuteAsync(
        Guid mediaId,
        CancellationToken cancellationToken = default);
}
