# ContentOS Feature Controller Implementation Guide

Complete guide for implementing feature controllers following ContentOS architectural patterns.

## Table of Contents

1. [Controller Checklist](#controller-checklist)
2. [Step-by-Step Implementation](#step-by-step-implementation)
3. [Feature Templates](#feature-templates)
4. [Testing Guidelines](#testing-guidelines)

---

## Controller Checklist

For each feature controller, you need:

- [ ] **Contracts (DTOs)**
  - [ ] Request DTOs
  - [ ] Response DTOs
  - [ ] Validators

- [ ] **Domain**
  - [ ] Domain entity
  - [ ] Value objects
  - [ ] Domain events

- [ ] **Use Cases**
  - [ ] Create use case
  - [ ] Update use case
  - [ ] Delete use case
  - [ ] Query use cases

- [ ] **Ports (Interfaces)**
  - [ ] Repository interface

- [ ] **Infrastructure**
  - [ ] Repository implementation
  - [ ] Mapper

- [ ] **API**
  - [ ] Endpoint definitions
  - [ ] OpenAPI documentation

- [ ] **Tests**
  - [ ] Unit tests
  - [ ] Integration tests

---

## Step-by-Step Implementation

### Example: Content Type Feature

We'll implement a complete Content Type feature as a reference.

### Step 1: Define Contracts (DTOs)

**File**: `src/core/TechWayFit.ContentOS.Contracts/Content/ContentTypeDto.cs`

```csharp
namespace TechWayFit.ContentOS.Contracts.Content;

/// <summary>
/// Request to create a new content type
/// </summary>
public record CreateContentTypeRequest
{
    public required string TypeKey { get; init; }
    public required string DisplayName { get; init; }
    public string? Description { get; init; }
    public List<CreateFieldRequest> Fields { get; init; } = new();
}

/// <summary>
/// Field definition in content type
/// </summary>
public record CreateFieldRequest
{
    public required string FieldKey { get; init; }
    public required string DisplayName { get; init; }
    public required string DataType { get; init; } // string|richtext|number|bool|datetime|ref|json
    public bool IsRequired { get; init; }
    public bool IsLocalized { get; init; }
    public Dictionary<string, object>? Constraints { get; init; }
    public int SortOrder { get; init; }
}

/// <summary>
/// Request to update content type
/// </summary>
public record UpdateContentTypeRequest
{
    public required string DisplayName { get; init; }
    public string? Description { get; init; }
}

/// <summary>
/// Content type response
/// </summary>
public record ContentTypeResponse
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public required string TypeKey { get; init; }
    public required string DisplayName { get; init; }
  public string? Description { get; init; }
    public int SchemaVersion { get; init; }
    public List<FieldResponse> Fields { get; init; } = new();
    public DateTimeOffset CreatedOn { get; init; }
    public DateTimeOffset UpdatedOn { get; init; }
    public bool IsSystem { get; init; }
}

public record FieldResponse
{
    public Guid Id { get; init; }
    public required string FieldKey { get; init; }
    public required string DisplayName { get; init; }
    public required string DataType { get; init; }
    public bool IsRequired { get; init; }
    public bool IsLocalized { get; init; }
    public Dictionary<string, object> Constraints { get; init; } = new();
  public int SortOrder { get; init; }
}
```

### Step 2: Create Validators

**File**: `src/core/TechWayFit.ContentOS.Contracts/Content/ContentTypeValidators.cs`

```csharp
using FluentValidation;

namespace TechWayFit.ContentOS.Contracts.Content;

public class CreateContentTypeRequestValidator : AbstractValidator<CreateContentTypeRequest>
{
  public CreateContentTypeRequestValidator()
    {
        RuleFor(x => x.TypeKey)
            .NotEmpty()
    .MaximumLength(200)
            .Matches(@"^[a-z][a-z0-9]*(\.[a-z][a-z0-9]*)*$")
            .WithMessage("TypeKey must be lowercase, dot-separated (e.g., page.article)");

        RuleFor(x => x.DisplayName)
  .NotEmpty()
         .MaximumLength(200);

        RuleFor(x => x.Fields)
         .NotEmpty()
            .WithMessage("At least one field is required");

        RuleForEach(x => x.Fields)
            .SetValidator(new CreateFieldRequestValidator());
    }
}

public class CreateFieldRequestValidator : AbstractValidator<CreateFieldRequest>
{
    private static readonly string[] ValidDataTypes = 
    {
        "string", "richtext", "number", "bool", "datetime", "ref", "json"
    };

    public CreateFieldRequestValidator()
    {
        RuleFor(x => x.FieldKey)
  .NotEmpty()
    .MaximumLength(200)
            .Matches(@"^[a-z][a-z0-9_]*$")
            .WithMessage("FieldKey must be lowercase, snake_case");

        RuleFor(x => x.DisplayName)
      .NotEmpty()
  .MaximumLength(200);

        RuleFor(x => x.DataType)
         .NotEmpty()
            .Must(dt => ValidDataTypes.Contains(dt.ToLowerInvariant()))
            .WithMessage($"DataType must be one of: {string.Join(", ", ValidDataTypes)}");
    }
}
```

### Step 3: Create Use Cases

**File**: `src/features/content/TechWayFit.ContentOS.Content/UseCases/CreateContentTypeUseCase.cs`

```csharp
using TechWayFit.ContentOS.Abstractions.Results;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Content.Domain;
using TechWayFit.ContentOS.Content.Ports;
using TechWayFit.ContentOS.Contracts.Content;

namespace TechWayFit.ContentOS.Content.UseCases;

public sealed class CreateContentTypeUseCase
{
    private readonly IContentTypeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateContentTypeUseCase(
        IContentTypeRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> ExecuteAsync(
  Guid tenantId,
 CreateContentTypeRequest request,
    Guid createdBy,
        CancellationToken cancellationToken = default)
    {
        // Validate uniqueness
        if (await _repository.TypeKeyExistsAsync(tenantId, request.TypeKey, cancellationToken))
        {
     return Result.Failure<Guid>($"Content type with key '{request.TypeKey}' already exists");
        }

        // Create domain entity
    var contentType = ContentType.Create(
            tenantId,
            request.TypeKey,
          request.DisplayName,
            createdBy);

     // Add fields
      foreach (var fieldRequest in request.Fields)
        {
  var field = ContentTypeField.Create(
      tenantId,
          contentType.Id,
        fieldRequest.FieldKey,
                fieldRequest.DisplayName,
       fieldRequest.DataType,
    fieldRequest.IsRequired,
            fieldRequest.IsLocalized,
       fieldRequest.Constraints ?? new Dictionary<string, object>(),
   fieldRequest.SortOrder,
        createdBy);

            contentType.AddField(field);
        }

        // Persist
     await _repository.AddAsync(contentType, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

     return Result.Success(contentType.Id);
    }
}
```

**File**: `src/features/content/TechWayFit.ContentOS.Content/UseCases/GetContentTypeUseCase.cs`

```csharp
public sealed class GetContentTypeUseCase
{
    private readonly IContentTypeRepository _repository;

    public GetContentTypeUseCase(IContentTypeRepository repository)
    {
   _repository = repository;
    }

public async Task<Result<ContentType>> ExecuteAsync(
        Guid tenantId,
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var contentType = await _repository.GetByIdAsync(id, cancellationToken);

        if (contentType == null || contentType.TenantId != tenantId)
        {
            return Result.Failure<ContentType>("Content type not found");
    }

    return Result.Success(contentType);
    }
}
```

**File**: `src/features/content/TechWayFit.ContentOS.Content/UseCases/ListContentTypesUseCase.cs`

```csharp
public sealed class ListContentTypesUseCase
{
    private readonly IContentTypeRepository _repository;

    public ListContentTypesUseCase(IContentTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<ContentType>>> ExecuteAsync(
    Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var contentTypes = await _repository.GetByTenantIdAsync(tenantId, cancellationToken);
      return Result.Success(contentTypes);
    }
}
```

### Step 4: Create API Endpoints

**File**: `src/delivery/api/TechWayFit.ContentOS.Api/Endpoints/ContentTypeEndpoints.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Abstractions.Security;
using TechWayFit.ContentOS.Content.UseCases;
using TechWayFit.ContentOS.Contracts.Common;
using TechWayFit.ContentOS.Contracts.Content;
using FluentValidation;

namespace TechWayFit.ContentOS.Api.Endpoints;

public static class ContentTypeEndpoints
{
    public static IEndpointRouteBuilder MapContentTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/content-types")
        .WithTags("Content Types")
  .RequireAuthorization();

        group.MapPost("/", CreateContentType)
 .WithName("CreateContentType")
  .WithOpenApi(op =>
            {
      op.Summary = "Create a new content type";
       op.Description = "Creates a new content type with fields for the current tenant";
                return op;
          })
    .Produces<ApiResponse<Guid>>(StatusCodes.Status201Created)
            .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest);

        group.MapGet("/", ListContentTypes)
            .WithName("ListContentTypes")
            .WithOpenApi()
 .Produces<ApiResponse<List<ContentTypeResponse>>>();

        group.MapGet("/{id:guid}", GetContentType)
     .WithName("GetContentType")
            .WithOpenApi()
            .Produces<ApiResponse<ContentTypeResponse>>()
    .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateContentType)
            .WithName("UpdateContentType")
      .WithOpenApi()
            .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
       .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteContentType)
         .WithName("DeleteContentType")
            .WithOpenApi()
  .Produces(StatusCodes.Status204NoContent)
       .Produces(StatusCodes.Status404NotFound);

   return app;
    }

    private static async Task<IResult> CreateContentType(
        [FromBody] CreateContentTypeRequest request,
      [FromServices] CreateContentTypeUseCase useCase,
        [FromServices] ITenantContext tenantContext,
   [FromServices] IAuthenticationContext authContext,
        [FromServices] IValidator<CreateContentTypeRequest> validator)
    {
        // Validate request
        var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
          var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
     return Results.BadRequest(ApiResponse<object>.FailureResponse(errors));
   }

        // Execute use case
        var result = await useCase.ExecuteAsync(
            tenantContext.CurrentTenantId,
       request,
         authContext.CurrentUserId!.Value);

        if (!result.IsSuccess)
        {
          return Results.BadRequest(ApiResponse<object>.FailureResponse(result.Error!));
     }

      return Results.Created(
  $"/api/content-types/{result.Value}",
            ApiResponse<Guid>.SuccessResponse(result.Value, "Content type created successfully"));
    }

    private static async Task<IResult> ListContentTypes(
        [FromServices] ListContentTypesUseCase useCase,
   [FromServices] ITenantContext tenantContext)
    {
        var result = await useCase.ExecuteAsync(tenantContext.CurrentTenantId);

        if (!result.IsSuccess)
        {
   return Results.BadRequest(ApiResponse<object>.FailureResponse(result.Error!));
        }

        var response = result.Value!.Select(ct => new ContentTypeResponse
        {
   Id = ct.Id,
        TenantId = ct.TenantId,
     TypeKey = ct.TypeKey,
   DisplayName = ct.DisplayName,
  SchemaVersion = ct.SchemaVersion,
            CreatedOn = ct.CreatedOn,
  UpdatedOn = ct.UpdatedOn,
          IsSystem = ct.IsSystem,
    Fields = ct.Fields.Select(f => new FieldResponse
      {
        Id = f.Id,
             FieldKey = f.FieldKey,
                DisplayName = f.DisplayName,
      DataType = f.DataType,
IsRequired = f.IsRequired,
      IsLocalized = f.IsLocalized,
 Constraints = f.Constraints,
                SortOrder = f.SortOrder
            }).ToList()
        }).ToList();

   return Results.Ok(ApiResponse<List<ContentTypeResponse>>.SuccessResponse(response));
}

    private static async Task<IResult> GetContentType(
        Guid id,
   [FromServices] GetContentTypeUseCase useCase,
      [FromServices] ITenantContext tenantContext)
    {
        var result = await useCase.ExecuteAsync(tenantContext.CurrentTenantId, id);

        if (!result.IsSuccess)
        {
  return Results.NotFound(ApiResponse<object>.FailureResponse(result.Error!));
        }

        var ct = result.Value!;
    var response = new ContentTypeResponse
 {
            Id = ct.Id,
       TenantId = ct.TenantId,
     TypeKey = ct.TypeKey,
 DisplayName = ct.DisplayName,
            SchemaVersion = ct.SchemaVersion,
          CreatedOn = ct.CreatedOn,
    UpdatedOn = ct.UpdatedOn,
 IsSystem = ct.IsSystem,
   Fields = ct.Fields.Select(f => new FieldResponse
       {
    Id = f.Id,
       FieldKey = f.FieldKey,
                DisplayName = f.DisplayName,
        DataType = f.DataType,
    IsRequired = f.IsRequired,
       IsLocalized = f.IsLocalized,
      Constraints = f.Constraints,
     SortOrder = f.SortOrder
    }).ToList()
        };

        return Results.Ok(ApiResponse<ContentTypeResponse>.SuccessResponse(response));
    }

    private static async Task<IResult> UpdateContentType(
        Guid id,
[FromBody] UpdateContentTypeRequest request,
        [FromServices] UpdateContentTypeUseCase useCase,
        [FromServices] ITenantContext tenantContext,
        [FromServices] IAuthenticationContext authContext)
  {
        var result = await useCase.ExecuteAsync(
    tenantContext.CurrentTenantId,
            id,
  request,
        authContext.CurrentUserId!.Value);

        if (!result.IsSuccess)
   {
            return Results.NotFound(ApiResponse<object>.FailureResponse(result.Error!));
 }

        return Results.Ok(ApiResponse<object>.SuccessResponse(null, "Content type updated successfully"));
    }

    private static async Task<IResult> DeleteContentType(
    Guid id,
        [FromServices] DeleteContentTypeUseCase useCase,
        [FromServices] ITenantContext tenantContext,
        [FromServices] IAuthenticationContext authContext)
    {
        var result = await useCase.ExecuteAsync(
       tenantContext.CurrentTenantId,
   id,
            authContext.CurrentUserId!.Value);

        if (!result.IsSuccess)
      {
      return Results.NotFound(ApiResponse<object>.FailureResponse(result.Error!));
        }

return Results.NoContent();
    }
}
```

### Step 5: Register Endpoints in Program.cs

**File**: `src/delivery/api/TechWayFit.ContentOS.Api/Program.cs`

```csharp
using TechWayFit.ContentOS.Api.Endpoints;

// ... existing code ...

app.MapContentTypeEndpoints();
app.MapContentItemEndpoints();
app.MapWorkflowEndpoints();
// ... other endpoints

app.Run();
```

### Step 6: Register Use Cases in DI

**File**: `src/delivery/api/TechWayFit.ContentOS.Api/Program.cs`

```csharp
using TechWayFit.ContentOS.Content.UseCases;
using FluentValidation;

// ... existing code ...

// Register validators
builder.Services.AddValidatorsFromAssemblyContaining<CreateContentTypeRequest>();

// Register use cases
builder.Services.AddScoped<CreateContentTypeUseCase>();
builder.Services.AddScoped<GetContentTypeUseCase>();
builder.Services.AddScoped<ListContentTypesUseCase>();
builder.Services.AddScoped<UpdateContentTypeUseCase>();
builder.Services.AddScoped<DeleteContentTypeUseCase>();
```

---

## Feature Templates

### Template 1: CRUD Feature

Use this for: Content Types, Roles, Groups, Tags, etc.

**Endpoints**:
- `POST /api/{feature}` - Create
- `GET /api/{feature}` - List (with pagination)
- `GET /api/{feature}/{id}` - Get by ID
- `PUT /api/{feature}/{id}` - Update
- `DELETE /api/{feature}/{id}` - Delete (soft delete)

### Template 2: Hierarchical Feature

Use this for: Content Nodes, Categories

**Additional Endpoints**:
- `GET /api/{feature}/{id}/children` - Get children
- `POST /api/{feature}/{id}/move` - Move node
- `GET /api/{feature}/tree` - Get tree structure

### Template 3: Workflow Feature

Use this for: Content Items with workflow

**Additional Endpoints**:
- `POST /api/{feature}/{id}/submit` - Submit for review
- `POST /api/{feature}/{id}/approve` - Approve
- `POST /api/{feature}/{id}/reject` - Reject
- `POST /api/{feature}/{id}/publish` - Publish
- `GET /api/{feature}/{id}/history` - Get history

---

## Testing Guidelines

### Unit Tests

**File**: `tests/TechWayFit.ContentOS.Content.Tests/UseCases/CreateContentTypeUseCaseTests.cs`

```csharp
using Xunit;
using Moq;
using FluentAssertions;

namespace TechWayFit.ContentOS.Content.Tests.UseCases;

public class CreateContentTypeUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsSuccess()
    {
   // Arrange
  var mockRepo = new Mock<IContentTypeRepository>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();
   
        mockRepo.Setup(r => r.TypeKeyExistsAsync(It.IsAny<Guid>(), It.IsAny<string>(), default))
          .ReturnsAsync(false);

        var useCase = new CreateContentTypeUseCase(mockRepo.Object, mockUnitOfWork.Object);
     
        var request = new CreateContentTypeRequest
        {
  TypeKey = "page.article",
       DisplayName = "Article",
         Fields = new List<CreateFieldRequest>
 {
                new() { FieldKey = "title", DisplayName = "Title", DataType = "string", IsRequired = true }
            }
        };

        // Act
        var result = await useCase.ExecuteAsync(
            Guid.NewGuid(),
          request,
         Guid.NewGuid());

        // Assert
      result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
      
        mockRepo.Verify(r => r.AddAsync(It.IsAny<ContentType>(), default), Times.Once);
   mockUnitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateTypeKey_ReturnsFailure()
    {
        // Arrange
        var mockRepo = new Mock<IContentTypeRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
      
        mockRepo.Setup(r => r.TypeKeyExistsAsync(It.IsAny<Guid>(), It.IsAny<string>(), default))
            .ReturnsAsync(true);

        var useCase = new CreateContentTypeUseCase(mockRepo.Object, mockUnitOfWork.Object);
        
        var request = new CreateContentTypeRequest
    {
            TypeKey = "page.article",
       DisplayName = "Article",
      Fields = new List<CreateFieldRequest> { /* ... */ }
        };

        // Act
        var result = await useCase.ExecuteAsync(Guid.NewGuid(), request, Guid.NewGuid());

   // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already exists");
    }
}
```

### Integration Tests

**File**: `tests/TechWayFit.ContentOS.Api.Tests/Endpoints/ContentTypeEndpointsTests.cs`

```csharp
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace TechWayFit.ContentOS.Api.Tests.Endpoints;

public class ContentTypeEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly Guid _tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public ContentTypeEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateContentType_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateContentTypeRequest
        {
            TypeKey = "test.article",
   DisplayName = "Test Article",
            Fields = new List<CreateFieldRequest>
    {
    new() { FieldKey = "title", DisplayName = "Title", DataType = "string", IsRequired = true }
}
   };

      // Act
        var response = await _client.PostAsJsonAsync("/api/content-types", request, options =>
    {
 options.DefaultRequestHeaders.Add("X-Tenant-Id", _tenantId.ToString());
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<Guid>>();
        result.Should().NotBeNull();
  result!.Success.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
    }
}
```

---

## Feature Implementation Order

1. **Phase 1: Foundation (Week 1)**
   - ? Security & Authentication
   - ? Tenant Management
   - ? User Management
   - ? Role Management

2. **Phase 2: Content Core (Week 2)**
   - Content Types
   - Content Items
   - Content Versions
   - Content Nodes (Tree)
   - Routes

3. **Phase 3: Workflow (Week 3)**
   - Workflow Definitions
   - Workflow States
   - Workflow Transitions
   - Content State Management

4. **Phase 4: Advanced (Week 4)**
   - Search
   - Media Management
   - Preview Tokens
   - Audit Logs

---

## Best Practices

### ? DO

- Use Result<T> pattern for use case returns
- Validate at API layer with FluentValidation
- Always check tenant scope
- Use cancellation tokens
- Log important operations
- Return proper HTTP status codes
- Use OpenAPI documentation
- Write unit tests for use cases
- Write integration tests for endpoints

### ? DON'T

- Don't call repositories directly from controllers
- Don't expose domain entities in API responses
- Don't skip validation
- Don't ignore cancellation tokens
- Don't return internal exception details
- Don't skip authorization checks
- Don't bypass tenant filtering

---

**Ready to implement?** Start with Content Type feature following this guide!
