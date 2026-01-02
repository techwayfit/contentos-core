using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Abstractions;
using TechWayFit.ContentOS.Content.Application;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;

namespace TechWayFit.ContentOS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
    private readonly ICreateContentUseCase _createContent;
    private readonly IAddLocalizationUseCase _addLocalization;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<ContentController> _logger;

    public ContentController(
        ICreateContentUseCase createContent,
        IAddLocalizationUseCase addLocalization,
        ITenantContext tenantContext,
        ILogger<ContentController> logger)
    {
        _createContent = createContent;
        _addLocalization = addLocalization;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ContentResponse>> CreateContent(
        [FromBody] CreateContentRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating content for tenant {TenantId}, site {SiteId}",
            _tenantContext.TenantId,
            _tenantContext.SiteId);

        var command = new CreateContentCommand(
            request.ContentType,
            request.LanguageCode,
            request.Title,
            request.Slug,
            request.Fields);

        var result = await _createContent.ExecuteAsync(command, cancellationToken);

        return result.Match<ActionResult<ContentResponse>>(
            success => CreatedAtAction(
                nameof(GetContent),
                new { id = success.Id },
                success),
            error => BadRequest(error));
    }

    [HttpPost("{id}/localizations")]
    public async Task<ActionResult<ContentResponse>> AddLocalization(
        Guid id,
        [FromBody] AddLocalizationRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Adding localization to content {ContentId} for language {LanguageCode}",
            id,
            request.LanguageCode);

        var command = new AddLocalizationCommand(
            new ContentItemId(id),
            request.LanguageCode,
            request.Title,
            request.Slug,
            request.Fields);

        var result = await _addLocalization.ExecuteAsync(command, cancellationToken);

        return result.Match<ActionResult<ContentResponse>>(
            success => Ok(success),
            error => error.Code == "ContentNotFound" 
                ? NotFound(error) 
                : BadRequest(error));
    }

    [HttpGet("{id}")]
    public ActionResult<ContentResponse> GetContent(Guid id)
    {
        // TODO: Implement GetContentUseCase
        _logger.LogWarning("GetContent not yet implemented for {ContentId}", id);
        return NotFound();
    }
}
