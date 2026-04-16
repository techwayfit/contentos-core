using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TechWayFit.ContentOS.Content.Application;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Contracts.Dtos;
using TechWayFit.ContentOS.Infrastructure.Runtime;
using TechWayFit.ContentOS.Api.Tenancy;

namespace TechWayFit.ContentOS.Api.Controllers;

/// <summary>
/// Content management operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Manage content items, localizations, and publishing workflow")]
public class ContentController : ControllerBase
{
    private readonly ICreateContentUseCase _createContent;
    private readonly IAddLocalizationUseCase _addLocalization;
  private readonly TenantContext _tenantContext;
    private readonly ILogger<ContentController> _logger;

    public ContentController(
        ICreateContentUseCase createContent,
        IAddLocalizationUseCase addLocalization,
        TenantContext tenantContext,
        ILogger<ContentController> logger)
    {
        _createContent = createContent;
        _addLocalization = addLocalization;
   _tenantContext = tenantContext;
        _logger = logger;
    }

    /// <summary>
    /// Create new content item
    /// </summary>
    /// <param name="request">Content creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created content item with initial localization</returns>
    /// <remarks>
    /// Creates a new content item with the default language localization.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/content
    ///     {
    ///       "contentType": "article",
    ///       "languageCode": "en-US",
    ///       "title": "Getting Started with ContentOS",
    ///       "slug": "getting-started",
    ///       "fields": {
    ///  "body": "Welcome to ContentOS...",
    /// "author": "John Doe",
    ///         "publishDate": "2024-01-15"
    ///       }
    ///     }
    /// 
    /// **Headers Required:**
    /// - `X-Tenant-Id`: Tenant UUID
    /// - `X-Site-Id`: Site UUID
    /// 
    /// **Permissions Required:**
    /// - `content:create`
    /// </remarks>
    /// <response code="201">Content item created successfully</response>
    /// <response code="400">Invalid request or validation error</response>
    /// <response code="401">Missing or invalid tenant/site headers</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Create new content item",
  Description = "Creates a new content item with default language localization",
        OperationId = "CreateContent",
        Tags = new[] { "Content" }
    )]
  [SwaggerResponse(201, "Content created successfully", typeof(ContentResponse))]
    [SwaggerResponse(400, "Invalid request", typeof(Error))]
  [SwaggerResponse(401, "Unauthorized")]
    [SwaggerResponse(403, "Forbidden")]
    [ProducesResponseType(typeof(ContentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContentResponse>> CreateContent(
        [FromBody, SwaggerRequestBody("Content creation request", Required = true)] CreateContentRequest request,
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

    /// <summary>
    /// Add language localization to existing content
    /// </summary>
    /// <param name="id">Content item ID</param>
    /// <param name="request">Localization details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated content item with new localization</returns>
    /// <remarks>
    /// Adds a new language variant to an existing content item.
    /// Each content item can have multiple localizations.
    /// 
    /// Sample request:
    /// 
    ///     POST /api/content/550e8400-e29b-41d4-a716-446655440000/localizations
    ///  {
    ///       "languageCode": "es-ES",
 ///       "title": "Primeros pasos con ContentOS",
    ///       "slug": "primeros-pasos",
    ///       "fields": {
    ///         "body": "Bienvenido a ContentOS...",
    ///"author": "Juan Pérez"
    ///       }
    ///     }
    /// 
 /// **Notes:**
    /// - Language code must be valid ISO 639-1 or BCP 47 format
 /// - Slug must be unique within the site and language
    /// - Field values are localized independently
    /// </remarks>
    /// <response code="200">Localization added successfully</response>
    /// <response code="400">Invalid request or duplicate language</response>
    /// <response code="404">Content item not found</response>
    [HttpPost("{id}/localizations")]
    [SwaggerOperation(
        Summary = "Add language localization",
        Description = "Adds a new language variant to existing content",
        OperationId = "AddLocalization",
    Tags = new[] { "Content", "Localization" }
    )]
    [SwaggerResponse(200, "Localization added", typeof(ContentResponse))]
    [SwaggerResponse(400, "Invalid request", typeof(Error))]
    [SwaggerResponse(404, "Content not found", typeof(Error))]
    [ProducesResponseType(typeof(ContentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentResponse>> AddLocalization(
        [FromRoute, SwaggerParameter("Content item identifier", Required = true)] Guid id,
        [FromBody, SwaggerRequestBody("Localization details", Required = true)] AddLocalizationRequest request,
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

    /// <summary>
    /// Get content item by ID
    /// </summary>
    /// <param name="id">Content item ID</param>
    /// <returns>Content item with all localizations</returns>
    /// <remarks>
    /// Retrieves a content item with all its localizations.
    /// 
    /// **Query Parameters** (planned):
    /// - `language`: Filter by specific language code
    /// - `include`: Include related data (versions, workflow, layout)
    /// 
    /// **Note:** This endpoint is not yet implemented.
    /// </remarks>
    /// <response code="200">Content item retrieved</response>
    /// <response code="404">Content not found</response>
    [HttpGet("{id}")]
    [SwaggerOperation(
  Summary = "Get content by ID",
        Description = "Retrieves a content item with all localizations (not yet implemented)",
        OperationId = "GetContent",
        Tags = new[] { "Content" }
    )]
    [SwaggerResponse(200, "Content found", typeof(ContentResponse))]
    [SwaggerResponse(404, "Content not found")]
    [ProducesResponseType(typeof(ContentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
 public ActionResult<ContentResponse> GetContent(
        [FromRoute, SwaggerParameter("Content item identifier", Required = true)] Guid id)
    {
 // TODO: Implement GetContentUseCase
      _logger.LogWarning("GetContent not yet implemented for {ContentId}", id);
        return NotFound();
    }
}
