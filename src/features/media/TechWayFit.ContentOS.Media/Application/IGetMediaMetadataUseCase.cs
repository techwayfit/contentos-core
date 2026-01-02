using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Kernel;

namespace TechWayFit.ContentOS.Media.Application;

public interface IGetMediaMetadataUseCase
{
    Task<Result<MediaMetadataResponse, Error>> ExecuteAsync(
        Guid mediaId,
        CancellationToken cancellationToken = default);
}
