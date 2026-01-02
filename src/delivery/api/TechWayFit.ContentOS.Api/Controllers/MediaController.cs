using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Media.Application;

namespace TechWayFit.ContentOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IGetMediaMetadataUseCase _getMediaMetadata;
    private readonly ILogger<MediaController> _logger;

    public MediaController(
        IGetMediaMetadataUseCase getMediaMetadata,
        ILogger<MediaController> logger)
    {
        _getMediaMetadata = getMediaMetadata;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MediaMetadataResponse>> GetMediaMetadata(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching media metadata for {MediaId}", id);

        var result = await _getMediaMetadata.ExecuteAsync(id, cancellationToken);

        return result.Match<ActionResult<MediaMetadataResponse>>(
            success => Ok(success),
            error => NotFound(error));
    }
}
