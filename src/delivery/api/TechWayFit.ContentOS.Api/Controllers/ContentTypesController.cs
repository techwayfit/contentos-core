using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Abstractions.Security;
using TechWayFit.ContentOS.Contracts.Common;
using TechWayFit.ContentOS.Contracts.Dtos.ContentTypes;
using TechWayFit.ContentOS.Content.Application.ContentTypes;
using TechWayFit.ContentOS.Content.Application.ContentTypeFields;

namespace TechWayFit.ContentOS.Api.Controllers;

/// <summary>
/// Content Type management endpoints (tenant-scoped)
/// </summary>
[Authorize]
[ApiController]
[Route("api/content-types")]
public class ContentTypesController : ControllerBase
{
    private readonly CreateContentTypeUseCase _createContentType;
    private readonly UpdateContentTypeUseCase _updateContentType;
    private readonly GetContentTypeUseCase _getContentType;
    private readonly ListContentTypesUseCase _listContentTypes;
    private readonly DeleteContentTypeUseCase _deleteContentType;
    private readonly AddFieldToContentTypeUseCase _addField;
    private readonly UpdateContentTypeFieldUseCase _updateField;
    private readonly RemoveFieldFromContentTypeUseCase _removeField;
    private readonly ListContentTypeFieldsUseCase _listFields;
    private readonly ITenantContext _tenantContext;

    public ContentTypesController(
     CreateContentTypeUseCase createContentType,
        UpdateContentTypeUseCase updateContentType,
        GetContentTypeUseCase getContentType,
        ListContentTypesUseCase listContentTypes,
        DeleteContentTypeUseCase deleteContentType,
        AddFieldToContentTypeUseCase addField,
        UpdateContentTypeFieldUseCase updateField,
        RemoveFieldFromContentTypeUseCase removeField,
        ListContentTypeFieldsUseCase listFields,
        ITenantContext tenantContext)
    {
        _createContentType = createContentType;
        _updateContentType = updateContentType;
        _getContentType = getContentType;
        _listContentTypes = listContentTypes;
        _deleteContentType = deleteContentType;
        _addField = addField;
        _updateField = updateField;
        _removeField = removeField;
        _listFields = listFields;
        _tenantContext = tenantContext;
    }

    /// <summary>
    /// Create a new content type
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateContentType(
        [FromBody] CreateContentTypeRequest request,
        CancellationToken cancellationToken)
  {
        var tenantId = _tenantContext.CurrentTenantId;

        var result = await _createContentType.ExecuteAsync(
    tenantId,
   request.TypeKey,
request.DisplayName,
            cancellationToken);

return result.Match<IActionResult>(
          success => Created($"/api/content-types/{success}", 
  ApiResponse<Guid>.SuccessResponse(success, "Content type created successfully")),
            error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
    /// Update an existing content type
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateContentType(
        Guid id,
        [FromBody] UpdateContentTypeRequest request,
  CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

  var result = await _updateContentType.ExecuteAsync(
  tenantId,
id,
    request.DisplayName,
            cancellationToken);

      return result.Match<IActionResult>(
            success => NoContent(),
  error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
    /// Get a content type by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetContentType(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var contentType = await _getContentType.ExecuteAsync(tenantId, id, cancellationToken);

      if (contentType == null)
        {
            return NotFound(ApiResponse<object>.FailureResponse("Content type not found"));
        }

        var response = new ContentTypeResponse
{
  Id = contentType.Id,
       TypeKey = contentType.TypeKey,
 DisplayName = contentType.DisplayName,
     SchemaVersion = contentType.SchemaVersion,
    CreatedAt = contentType.Audit.CreatedOn,
       UpdatedAt = contentType.Audit.UpdatedOn
   };

        return Ok(ApiResponse<ContentTypeResponse>.SuccessResponse(response));
    }

    /// <summary>
    /// List all content types for the current tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListContentTypes(CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var contentTypes = await _listContentTypes.ExecuteAsync(tenantId, cancellationToken);

        var response = contentTypes.Select(ct => new ContentTypeResponse
        {
     Id = ct.Id,
            TypeKey = ct.TypeKey,
            DisplayName = ct.DisplayName,
            SchemaVersion = ct.SchemaVersion,
         CreatedAt = ct.Audit.CreatedOn,
      UpdatedAt = ct.Audit.UpdatedOn
        }).ToList();

 return Ok(ApiResponse<IReadOnlyList<ContentTypeResponse>>.SuccessResponse(response));
    }

    /// <summary>
    /// Delete a content type
    /// </summary>
  [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteContentType(
    Guid id,
        CancellationToken cancellationToken)
    {
      var tenantId = _tenantContext.CurrentTenantId;

        var result = await _deleteContentType.ExecuteAsync(tenantId, id, cancellationToken);

    return result.Match<IActionResult>(
       success => NoContent(),
     error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
    /// Add a field to a content type
    /// </summary>
    [HttpPost("{contentTypeId:guid}/fields")]
    public async Task<IActionResult> AddField(
        Guid contentTypeId,
        [FromBody] AddFieldRequest request,
        CancellationToken cancellationToken)
    {
    var tenantId = _tenantContext.CurrentTenantId;

        var result = await _addField.ExecuteAsync(
tenantId,
contentTypeId,
       request.FieldKey,
          request.DataType,
            request.IsRequired,
          request.IsLocalized,
            request.ConstraintsJson,
         cancellationToken);

      return result.Match<IActionResult>(
            success => Created($"/api/content-types/{contentTypeId}/fields/{success}",
         ApiResponse<Guid>.SuccessResponse(success, "Field added successfully")),
            error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
    /// Update a content type field
    /// </summary>
    [HttpPut("{contentTypeId:guid}/fields/{fieldId:guid}")]
    public async Task<IActionResult> UpdateField(
Guid contentTypeId,
   Guid fieldId,
   [FromBody] UpdateFieldRequest request,
        CancellationToken cancellationToken)
    {
 var tenantId = _tenantContext.CurrentTenantId;

        var result = await _updateField.ExecuteAsync(
      tenantId,
      fieldId,
   request.IsRequired,
            request.IsLocalized,
            request.ConstraintsJson,
 cancellationToken);

  return result.Match<IActionResult>(
    success => NoContent(),
         error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
    /// Remove a field from a content type
 /// </summary>
    [HttpDelete("{contentTypeId:guid}/fields/{fieldId:guid}")]
    public async Task<IActionResult> RemoveField(
     Guid contentTypeId,
  Guid fieldId,
        CancellationToken cancellationToken)
 {
        var tenantId = _tenantContext.CurrentTenantId;

var result = await _removeField.ExecuteAsync(tenantId, fieldId, cancellationToken);

     return result.Match<IActionResult>(
  success => NoContent(),
    error => BadRequest(ApiResponse<object>.FailureResponse(error)));
    }

    /// <summary>
    /// List all fields for a content type
    /// </summary>
    [HttpGet("{contentTypeId:guid}/fields")]
    public async Task<IActionResult> ListFields(
   Guid contentTypeId,
     CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.CurrentTenantId;

        var fields = await _listFields.ExecuteAsync(tenantId, contentTypeId, cancellationToken);

 var response = fields.Select(f => new ContentTypeFieldResponse
        {
  Id = f.Id,
       ContentTypeId = f.ContentTypeId,
FieldKey = f.FieldKey,
            DataType = f.DataType,
 IsRequired = f.IsRequired,
   IsLocalized = f.IsLocalized,
ConstraintsJson = f.ConstraintsJson,
       SortOrder = f.SortOrder,
         CreatedAt = f.Audit.CreatedOn,
            UpdatedAt = f.Audit.UpdatedOn
        }).ToList();

        return Ok(ApiResponse<IReadOnlyList<ContentTypeFieldResponse>>.SuccessResponse(response));
    }
}
