using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Abstractions.Security;
using TechWayFit.ContentOS.Contracts.Common;
using TechWayFit.ContentOS.Contracts.Dtos.ContentItems;
using TechWayFit.ContentOS.Content.Application.ContentItems;
using TechWayFit.ContentOS.Content.Application.ContentVersions;
using TechWayFit.ContentOS.Content.Application.ContentFieldValues;
using TechWayFit.ContentOS.Contracts.Dtos.ContentVersions;

namespace TechWayFit.ContentOS.Api.Controllers;

/// <summary>
/// Content Item management endpoints (tenant-scoped)
/// </summary>
[Authorize]
[ApiController]
[Route("api/content")]
public class ContentItemsController : ControllerBase
{
    private readonly CreateContentItemUseCase _createItem;
    private readonly UpdateContentItemUseCase _updateItem;
    private readonly GetContentItemUseCase _getItem;
    private readonly ListContentItemsUseCase _listItems;
    private readonly DeleteContentItemUseCase _deleteItem;
    private readonly CreateContentVersionUseCase _createVersion;
    private readonly ListContentVersionsUseCase _listVersions;
    private readonly GetLatestVersionUseCase _getLatestVersion;
    private readonly GetPublishedVersionUseCase _getPublishedVersion;
    private readonly PublishContentVersionUseCase _publishVersion;
    private readonly UpdateFieldValuesUseCase _updateFieldValues;
    private readonly GetFieldValuesUseCase _getFieldValues;
 private readonly ITenantContext _tenantContext;

    public ContentItemsController(
    CreateContentItemUseCase createItem,
 UpdateContentItemUseCase updateItem,
    GetContentItemUseCase getItem,
        ListContentItemsUseCase listItems,
     DeleteContentItemUseCase deleteItem,
  CreateContentVersionUseCase createVersion,
        ListContentVersionsUseCase listVersions,
        GetLatestVersionUseCase getLatestVersion,
GetPublishedVersionUseCase getPublishedVersion,
      PublishContentVersionUseCase publishVersion,
        UpdateFieldValuesUseCase updateFieldValues,
GetFieldValuesUseCase getFieldValues,
        ITenantContext tenantContext)
    {
   _createItem = createItem;
      _updateItem = updateItem;
        _getItem = getItem;
_listItems = listItems;
 _deleteItem = deleteItem;
        _createVersion = createVersion;
     _listVersions = listVersions;
        _getLatestVersion = getLatestVersion;
 _getPublishedVersion = getPublishedVersion;
    _publishVersion = publishVersion;
        _updateFieldValues = updateFieldValues;
      _getFieldValues = getFieldValues;
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Create a new content item
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateContentItem(
[FromBody] CreateContentItemRequest request,
 CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var result = await _createItem.ExecuteAsync(
      tenantId,
       request.SiteId,
       request.ContentTypeId,
       cancellationToken);

        return result.Match<IActionResult>(
     success => Created($"/api/content/{success}",
       ApiResponse<Guid>.SuccessResponse(success, "Content item created successfully")),
       error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
    /// Update a content item
    /// </summary>
 [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateContentItem(
  Guid id,
    [FromBody] UpdateContentItemRequest request,
   CancellationToken cancellationToken)
    {
   var tenantId = _tenantContext.CurrentTenantId;

        var result = await _updateItem.ExecuteAsync(
   tenantId,
    id,
            request.Status,
       cancellationToken);

        return result.Match<IActionResult>(
       success => NoContent(),
  error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
    /// Get a content item by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetContentItem(
  Guid id,
    CancellationToken cancellationToken)
    {
    var tenantId = _tenantContext.CurrentTenantId;

        var (item, latestVersion) = await _getItem.ExecuteWithVersionAsync(tenantId, id, cancellationToken);

        if (item == null)
  {
 return NotFound(ApiResponse<object>.FailureResponse("Content item not found"));
 }

   var response = new ContentItemResponse
      {
Id = item.Id,
      SiteId = item.SiteId,
        ContentTypeId = item.ContentTypeId,
  Status = item.Status,
 CreatedAt = item.Audit.CreatedOn,
            UpdatedAt = item.Audit.UpdatedOn
    };

        return Ok(ApiResponse<ContentItemResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// List content items with optional filters
    /// </summary>
    [HttpGet]
 public async Task<IActionResult> ListContentItems(
        [FromQuery] Guid? siteId,
        [FromQuery] Guid? contentTypeId,
     [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
var tenantId = _tenantContext.CurrentTenantId;

        var items = await _listItems.ExecuteAsync(tenantId, siteId, contentTypeId, status, cancellationToken);

        var response = items.Select(i => new ContentItemResponse
 {
     Id = i.Id,
       SiteId = i.SiteId,
  ContentTypeId = i.ContentTypeId,
   Status = i.Status,
   CreatedAt = i.Audit.CreatedOn,
 UpdatedAt = i.Audit.UpdatedOn
 }).ToList();

    return Ok(ApiResponse<IReadOnlyList<ContentItemResponse>>.SuccessResponse(response));
    }

    /// <summary>
    /// Delete a content item
    /// </summary>
  [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteContentItem(
   Guid id,
     CancellationToken cancellationToken)
 {
        var tenantId = _tenantContext.CurrentTenantId;

        var result = await _deleteItem.ExecuteAsync(tenantId, id, cancellationToken);

 return result.Match<IActionResult>(
      success => NoContent(),
error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
    /// Create a new version of a content item
    /// </summary>
  [HttpPost("{id:guid}/versions")]
    public async Task<IActionResult> CreateVersion(
        Guid id,
        [FromBody] CreateVersionRequest request,
     CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var result = await _createVersion.ExecuteAsync(
   tenantId,
        id,
     request.CopyFromLatest,
   cancellationToken);

  return result.Match<IActionResult>(
    success => Created($"/api/content/{id}/versions/{success}",
   ApiResponse<Guid>.SuccessResponse(success, "Version created successfully")),
            error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
    /// List all versions for a content item
    /// </summary>
    [HttpGet("{id:guid}/versions")]
    public async Task<IActionResult> ListVersions(
        Guid id,
        CancellationToken cancellationToken)
    {
 var tenantId = _tenantContext.CurrentTenantId;

 var versions = await _listVersions.ExecuteAsync(tenantId, id, cancellationToken);

      var response = versions.Select(v => new ContentVersionResponse
        {
      Id = v.Id,
      ContentItemId = v.ContentItemId,
    VersionNumber = v.VersionNumber,
            Lifecycle = v.Lifecycle,
         WorkflowStateId = v.WorkflowStateId,
  PublishedAt = v.PublishedAt,
     CreatedAt = v.Audit.CreatedOn,
            UpdatedAt = v.Audit.UpdatedOn
        }).ToList();

  return Ok(ApiResponse<IReadOnlyList<ContentVersionResponse>>.SuccessResponse(response));
    }

    /// <summary>
    /// Get the latest version
    /// </summary>
    [HttpGet("{id:guid}/versions/latest")]
    public async Task<IActionResult> GetLatestVersion(
   Guid id,
        CancellationToken cancellationToken)
    {
   var tenantId = _tenantContext.CurrentTenantId;

  var version = await _getLatestVersion.ExecuteAsync(tenantId, id, cancellationToken);

        if (version == null)
 {
            return NotFound(ApiResponse<object>.FailureResponse("Latest version not found"));
  }

        var response = new ContentVersionResponse
        {
    Id = version.Id,
            ContentItemId = version.ContentItemId,
   VersionNumber = version.VersionNumber,
         Lifecycle = version.Lifecycle,
     WorkflowStateId = version.WorkflowStateId,
   PublishedAt = version.PublishedAt,
   CreatedAt = version.Audit.CreatedOn,
          UpdatedAt = version.Audit.UpdatedOn
    };

    return Ok(ApiResponse<ContentVersionResponse>.SuccessResponse(response));
    }

    /// <summary>
 /// Get the published version
    /// </summary>
    [HttpGet("{id:guid}/versions/published")]
    public async Task<IActionResult> GetPublishedVersion(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

     var version = await _getPublishedVersion.ExecuteAsync(tenantId, id, cancellationToken);

     if (version == null)
 {
     return NotFound(ApiResponse<object>.FailureResponse("Published version not found"));
        }

        var response = new ContentVersionResponse
        {
            Id = version.Id,
       ContentItemId = version.ContentItemId,
   VersionNumber = version.VersionNumber,
Lifecycle = version.Lifecycle,
          WorkflowStateId = version.WorkflowStateId,
            PublishedAt = version.PublishedAt,
  CreatedAt = version.Audit.CreatedOn,
  UpdatedAt = version.Audit.UpdatedOn
   };

        return Ok(ApiResponse<ContentVersionResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// Publish a version
    /// </summary>
    [HttpPost("{id:guid}/versions/{versionId:guid}/publish")]
    public async Task<IActionResult> PublishVersion(
  Guid id,
        Guid versionId,
 CancellationToken cancellationToken)
 {
        var tenantId = _tenantContext.CurrentTenantId;

     var result = await _publishVersion.ExecuteAsync(tenantId, versionId, cancellationToken);

return result.Match<IActionResult>(
     success => NoContent(),
       error => BadRequest(ApiResponse<object>.FailureResponse(error)));
 }

    /// <summary>
    /// Update field values for a version
 /// </summary>
    [HttpPut("{id:guid}/versions/{versionId:guid}/fields")]
    public async Task<IActionResult> UpdateFieldValues(
  Guid id,
      Guid versionId,
        [FromBody] UpdateFieldValuesRequest request,
        CancellationToken cancellationToken)
    {
    var tenantId = _tenantContext.CurrentTenantId;

var result = await _updateFieldValues.ExecuteAsync(
   tenantId,
  versionId,
     request.FieldValues,
 request.Locale,
     cancellationToken);

 return result.Match<IActionResult>(
  success => NoContent(),
      error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

/// <summary>
    /// Get field values for a version
    /// </summary>
    [HttpGet("{id:guid}/versions/{versionId:guid}/fields")]
    public async Task<IActionResult> GetFieldValues(
        Guid id,
   Guid versionId,
        [FromQuery] string? locale,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var fieldValues = await _getFieldValues.ExecuteAsync(tenantId, versionId, locale, cancellationToken);

        var response = fieldValues.Select(fv => new ContentFieldValueResponse
        {
Id = fv.Id,
      FieldKey = fv.FieldKey,
  Locale = fv.Locale,
  ValueJson = fv.ValueJson
    }).ToList();

        return Ok(ApiResponse<IReadOnlyList<ContentFieldValueResponse>>.SuccessResponse(response));
    }
}
