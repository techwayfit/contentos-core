# ContentOS API Implementation Roadmap

This document outlines the step-by-step approach to building the ContentOS API layer, from security foundations to feature controllers.

## ?? Table of Contents

1. [Overview](#overview)
2. [Phase 1: API Security Foundation](#phase-1-api-security-foundation)
3. [Phase 2: Tenant & User Management](#phase-2-tenant--user-management)
4. [Phase 3: Core Features](#phase-3-core-features)
5. [Phase 4: Advanced Features](#phase-4-advanced-features)
6. [Implementation Patterns](#implementation-patterns)

---

## Overview

### Architecture Principles

? **Clean Architecture**: API ? Use Cases ? Domain ? Infrastructure  
? **CQRS Pattern**: Separate commands and queries  
? **Multi-Tenancy**: Every request is tenant-scoped  
? **Security-First**: Authentication & Authorization from day one  
? **Use Cases**: Controllers call use cases, never repositories directly

### Technology Stack

- **API Framework**: ASP.NET Core Minimal APIs (.NET 10)
- **Authentication**: JWT Bearer tokens (MVP: Header-based, Future: OAuth2/OIDC)
- **Authorization**: Policy-based with custom requirements
- **Validation**: FluentValidation
- **Documentation**: OpenAPI/Swagger
- **Monitoring**: Health checks, logging, metrics

---

## Phase 1: API Security Foundation

### 1.1 Authentication Infrastructure

**Goal**: Establish authentication mechanism for all API requests.

#### Step 1: Create Authentication Abstractions

**File**: `src/core/TechWayFit.ContentOS.Abstractions/Security/IAuthenticationContext.cs`

```csharp
namespace TechWayFit.ContentOS.Abstractions.Security;

/// <summary>
/// Provides access to the current authenticated user context
/// </summary>
public interface IAuthenticationContext
{
    /// <summary>
    /// Current user ID (null if not authenticated)
    /// </summary>
    Guid? CurrentUserId { get; }
    
 /// <summary>
    /// Current user email
    /// </summary>
    string? CurrentUserEmail { get; }
    
    /// <summary>
    /// Current user roles
    /// </summary>
    IReadOnlyList<string> Roles { get; }
    
    /// <summary>
    /// Current user permissions/scopes
    /// </summary>
    IReadOnlyList<string> Permissions { get; }
    
    /// <summary>
    /// Whether user is SuperAdmin
    /// </summary>
    bool IsSuperAdmin { get; }
    
    /// <summary>
    /// Whether user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
}
```

**File**: `src/core/TechWayFit.ContentOS.Abstractions/Security/ITenantContext.cs`

```csharp
namespace TechWayFit.ContentOS.Abstractions.Security;

/// <summary>
/// Provides access to the current tenant context
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Current tenant ID (resolved from request)
    /// </summary>
    Guid CurrentTenantId { get; }

    /// <summary>
    /// Current tenant key
    /// </summary>
    string CurrentTenantKey { get; }
 
    /// <summary>
    /// Whether tenant context is resolved
    /// </summary>
    bool IsResolved { get; }
}
```

#### Step 2: Implement Context Providers

**File**: `src/infrastructure/identity/TechWayFit.ContentOS.Infrastructure.Identity/HttpAuthenticationContext.cs`

```csharp
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TechWayFit.ContentOS.Abstractions.Security;

namespace TechWayFit.ContentOS.Infrastructure.Identity;

public sealed class HttpAuthenticationContext : IAuthenticationContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpAuthenticationContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? CurrentUserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
      .FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
      return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
    }

  public string? CurrentUserEmail => 
        _httpContextAccessor.HttpContext?.User
     .FindFirst(ClaimTypes.Email)?.Value;

    public IReadOnlyList<string> Roles =>
        _httpContextAccessor.HttpContext?.User
     .FindAll(ClaimTypes.Role)
          .Select(c => c.Value)
       .ToList() ?? new List<string>();

    public IReadOnlyList<string> Permissions =>
   _httpContextAccessor.HttpContext?.User
            .FindAll("permission")
         .Select(c => c.Value)
            .ToList() ?? new List<string>();

    public bool IsSuperAdmin =>
        _httpContextAccessor.HttpContext?.Request.Headers["X-SuperAdmin"]
    .FirstOrDefault() == "true"; // MVP: Header-based

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
```

**File**: `src/infrastructure/identity/TechWayFit.ContentOS.Infrastructure.Identity/HttpTenantContext.cs`

```csharp
using Microsoft.AspNetCore.Http;
using TechWayFit.ContentOS.Abstractions.Security;

namespace TechWayFit.ContentOS.Infrastructure.Identity;

public sealed class HttpTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _cachedTenantId;
    private string? _cachedTenantKey;

    public HttpTenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

  public Guid CurrentTenantId
    {
 get
        {
            if (_cachedTenantId.HasValue)
return _cachedTenantId.Value;

            var tenantIdHeader = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"]
                .FirstOrDefault();

      if (Guid.TryParse(tenantIdHeader, out var tenantId))
       {
      _cachedTenantId = tenantId;
     return tenantId;
            }

   throw new InvalidOperationException("Tenant context not resolved. X-Tenant-Id header is required.");
        }
    }

    public string CurrentTenantKey
 {
   get
        {
            if (!string.IsNullOrEmpty(_cachedTenantKey))
                return _cachedTenantKey;

    var tenantKey = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Key"]
                .FirstOrDefault();

      if (!string.IsNullOrEmpty(tenantKey))
            {
       _cachedTenantKey = tenantKey;
    return tenantKey;
            }

          throw new InvalidOperationException("Tenant key not resolved.");
        }
    }

    public bool IsResolved => _cachedTenantId.HasValue;
}
```

#### Step 3: Create Authentication Middleware

**File**: `src/delivery/api/TechWayFit.ContentOS.Api/Middleware/TenantResolutionMiddleware.cs`

```csharp
namespace TechWayFit.ContentOS.Api.Middleware;

/// <summary>
/// Resolves tenant context from request headers
/// </summary>
public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();

        // Skip tenant resolution for:
        // - Health checks
 // - Swagger/OpenAPI
        // - Admin endpoints (SuperAdmin only)
        if (path?.StartsWith("/health") == true ||
            path?.StartsWith("/swagger") == true ||
            path?.StartsWith("/api/admin") == true)
  {
         await _next(context);
            return;
 }

        // Require tenant header for all other endpoints
        var tenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(tenantId))
        {
    context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
 {
error = "Missing required header: X-Tenant-Id"
        });
        return;
        }

  if (!Guid.TryParse(tenantId, out _))
        {
      context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
   {
  error = "Invalid X-Tenant-Id format. Must be a valid GUID."
       });
  return;
        }

        await _next(context);
    }
}
```

#### Step 4: Configure Authentication in Program.cs

**File**: `src/delivery/api/TechWayFit.ContentOS.Api/Program.cs`

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TechWayFit.ContentOS.Api.Middleware;
using TechWayFit.ContentOS.Abstractions.Security;
using TechWayFit.ContentOS.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// SECURITY CONFIGURATION
// ============================================================================

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Register context providers
builder.Services.AddScoped<IAuthenticationContext, HttpAuthenticationContext>();
builder.Services.AddScoped<ITenantContext, HttpTenantContext>();

// Configure JWT authentication (MVP: Simple JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
 {
      options.TokenValidationParameters = new TokenValidationParameters
   {
ValidateIssuer = true,
            ValidateAudience = true,
  ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
  ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(
     Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };

      // Read token from Authorization header OR X-API-Key (for service accounts)
   options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
{
      // Check for API key in header
    var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
      if (!string.IsNullOrEmpty(apiKey))
  {
         // TODO: Validate API key and generate claims
             }
    return Task.CompletedTask;
   }
        };
    });

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
// SuperAdmin policy
    options.AddPolicy("SuperAdmin", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("scope", "platform:superadmin") ||
  context.Request.Headers["X-SuperAdmin"].FirstOrDefault() == "true")); // MVP only

    // Tenant admin policy
  options.AddPolicy("TenantAdmin", policy =>
        policy.RequireClaim("role", "TenantAdmin"));

    // Content management policies
    options.AddPolicy("Content.Create", policy =>
        policy.RequireClaim("permission", "content:create"));
    
    options.AddPolicy("Content.Edit", policy =>
        policy.RequireClaim("permission", "content:edit"));
    
 options.AddPolicy("Content.Publish", policy =>
        policy.RequireClaim("permission", "content:publish"));
    
    options.AddPolicy("Content.Delete", policy =>
        policy.RequireClaim("permission", "content:delete"));
});

var app = builder.Build();

// ============================================================================
// MIDDLEWARE PIPELINE
// ============================================================================

// Security middleware (order matters!)
app.UseMiddleware<TenantResolutionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

### 1.2 Create Base API Response Types

**File**: `src/core/TechWayFit.ContentOS.Contracts/Common/ApiResponse.cs`

```csharp
namespace TechWayFit.ContentOS.Contracts.Common;

public record ApiResponse<T>
{
  public bool Success { get; init; }
    public T? Data { get; init; }
 public string? Message { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public static ApiResponse<T> SuccessResponse(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    public static ApiResponse<T> FailureResponse(string error) => new()
    {
      Success = false,
    Errors = new[] { error }
    };

  public static ApiResponse<T> FailureResponse(IReadOnlyList<string> errors) => new()
    {
    Success = false,
        Errors = errors
    };
}

public record PagedResponse<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
```

---

## Phase 2: Tenant & User Management

### 2.1 Tenant Management (SuperAdmin Only)

#### Step 1: Create Tenant DTOs

**File**: `src/core/TechWayFit.ContentOS.Contracts/Tenancy/CreateTenantRequest.cs`

```csharp
namespace TechWayFit.ContentOS.Contracts.Tenancy;

public record CreateTenantRequest
{
    public required string Key { get; init; }
    public required string Name { get; init; }
}

public record TenantResponse
{
    public Guid Id { get; init; }
    public required string Key { get; init; }
    public required string Name { get; init; }
    public int Status { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
```

#### Step 2: Create Tenant Use Case

**File**: `src/features/tenancy/TechWayFit.ContentOS.Tenancy/UseCases/CreateTenantUseCase.cs`

```csharp
using TechWayFit.ContentOS.Abstractions.Results;
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.Tenancy.Domain;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Tenancy.UseCases;

public sealed class CreateTenantUseCase
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenantUseCase(
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> ExecuteAsync(
        string key,
        string name,
  Guid createdBy,
        CancellationToken cancellationToken = default)
    {
        // Validate key uniqueness
        if (await _tenantRepository.KeyExistsAsync(key, cancellationToken))
  {
     return Result.Failure<Guid>("Tenant key already exists");
        }

  // Create tenant
        var tenant = Tenant.Create(key, name, createdBy);

        // Persist
        await _tenantRepository.AddAsync(tenant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(tenant.Id);
    }
}
```

#### Step 3: Create Tenant Controller

**File**: `src/delivery/api/TechWayFit.ContentOS.Api/Endpoints/Admin/TenantEndpoints.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechWayFit.ContentOS.Contracts.Common;
using TechWayFit.ContentOS.Contracts.Tenancy;
using TechWayFit.ContentOS.Tenancy.UseCases;
using TechWayFit.ContentOS.Tenancy.Ports;

namespace TechWayFit.ContentOS.Api.Endpoints.Admin;

public static class TenantEndpoints
{
    public static IEndpointRouteBuilder MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/tenants")
  .WithTags("Admin - Tenants")
            .RequireAuthorization("SuperAdmin");

        // Create tenant
  group.MapPost("/", CreateTenant)
            .WithName("CreateTenant")
        .WithOpenApi();

        // List tenants
    group.MapGet("/", ListTenants)
   .WithName("ListTenants")
            .WithOpenApi();

        // Get tenant by ID
    group.MapGet("/{id:guid}", GetTenant)
            .WithName("GetTenant")
          .WithOpenApi();

        return app;
  }

    private static async Task<IResult> CreateTenant(
        [FromBody] CreateTenantRequest request,
        [FromServices] CreateTenantUseCase useCase,
        HttpContext context)
    {
   // MVP: Use system user ID
        var createdBy = Guid.Parse("00000000-0000-0000-0000-000000000000");

        var result = await useCase.ExecuteAsync(
            request.Key,
    request.Name,
  createdBy);

     if (!result.IsSuccess)
        {
       return Results.BadRequest(ApiResponse<object>.FailureResponse(result.Error!));
        }

    return Results.Created(
     $"/api/admin/tenants/{result.Value}",
            ApiResponse<Guid>.SuccessResponse(result.Value, "Tenant created successfully"));
    }

    private static async Task<IResult> ListTenants(
        [FromServices] ITenantRepository repository,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
     var tenants = await repository.GetAllAsync();
        
  var response = new PagedResponse<TenantResponse>
        {
   Items = tenants.Select(t => new TenantResponse
   {
       Id = t.Id,
            Key = t.Key,
       Name = t.Name,
   Status = (int)t.Status,
      CreatedAt = t.CreatedAt
            }).ToList(),
            TotalCount = tenants.Count,
            PageNumber = page,
       PageSize = pageSize
        };

   return Results.Ok(ApiResponse<PagedResponse<TenantResponse>>.SuccessResponse(response));
    }

    private static async Task<IResult> GetTenant(
        Guid id,
        [FromServices] ITenantRepository repository)
    {
        var tenant = await repository.GetByIdAsync(id);
        
     if (tenant == null)
     {
         return Results.NotFound(ApiResponse<object>.FailureResponse("Tenant not found"));
        }

        var response = new TenantResponse
        {
            Id = tenant.Id,
            Key = tenant.Key,
    Name = tenant.Name,
            Status = (int)tenant.Status,
            CreatedAt = tenant.CreatedAt
        };

   return Results.Ok(ApiResponse<TenantResponse>.SuccessResponse(response));
    }
}
```

### 2.2 User Management (Tenant-Scoped)

Follow similar pattern for User, Role, and Group management.

---

## Phase 3: Core Features

### 3.1 Content Type Management

**Endpoints**:
- `POST /api/content-types` - Create content type
- `GET /api/content-types` - List content types
- `GET /api/content-types/{id}` - Get content type
- `PUT /api/content-types/{id}` - Update content type
- `DELETE /api/content-types/{id}` - Delete content type

### 3.2 Content Item Management

**Endpoints**:
- `POST /api/content` - Create content item
- `GET /api/content` - List content items
- `GET /api/content/{id}` - Get content item
- `PUT /api/content/{id}` - Update content item
- `POST /api/content/{id}/publish` - Publish content
- `DELETE /api/content/{id}` - Delete content

### 3.3 Workflow Management

**Endpoints**:
- `POST /api/workflows` - Create workflow
- `GET /api/workflows` - List workflows
- `POST /api/content/{id}/transition` - Transition workflow state

---

## Phase 4: Advanced Features

### 4.1 Search
### 4.2 Media Management
### 4.3 AI Features

---

## Implementation Patterns

### Controller Pattern (Minimal API)

```csharp
public static class FeatureEndpoints
{
 public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
   var group = app.MapGroup("/api/feature")
   .WithTags("Feature")
         .RequireAuthorization();

 group.MapPost("/", CreateHandler);
        group.MapGet("/", ListHandler);
        group.MapGet("/{id}", GetHandler);
        group.MapPut("/{id}", UpdateHandler);
        group.MapDelete("/{id}", DeleteHandler);

        return app;
    }

    private static async Task<IResult> CreateHandler(
        [FromBody] CreateRequest request,
      [FromServices] CreateUseCase useCase,
        [FromServices] ITenantContext tenantContext,
        [FromServices] IAuthenticationContext authContext)
    {
  var result = await useCase.ExecuteAsync(
            tenantContext.CurrentTenantId,
            request,
            authContext.CurrentUserId!.Value);

        return result.IsSuccess
     ? Results.Created($"/api/feature/{result.Value}", 
      ApiResponse<Guid>.SuccessResponse(result.Value))
            : Results.BadRequest(ApiResponse<object>.FailureResponse(result.Error!));
    }
}
```

### Use Case Pattern

```csharp
public sealed class CreateFeatureUseCase
{
    private readonly IFeatureRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPolicyEvaluator _policyEvaluator;

    public async Task<Result<Guid>> ExecuteAsync(
        Guid tenantId,
        CreateRequest request,
 Guid userId)
    {
     // 1. Authorization
        await _policyEvaluator.RequireAsync("feature:create");

        // 2. Validation
        if (await _repository.NameExistsAsync(tenantId, request.Name))
  return Result.Failure<Guid>("Name already exists");

    // 3. Domain logic
var entity = Feature.Create(tenantId, request.Name, userId);

        // 4. Persistence
 await _repository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        // 5. Events (optional)
  // await _eventBus.PublishAsync(new FeatureCreated(entity.Id));

        return Result.Success(entity.Id);
    }
}
```

---

## Next Steps

1. ? Review this roadmap
2. ? Implement Phase 1 (Security)
3. ? Implement Phase 2 (Tenants/Users)
4. ? Implement Phase 3 (Core Features)
5. ? Add integration tests
6. ? Add OpenAPI documentation
7. ? Deploy to staging

---

**Ready to start?** Begin with Phase 1, Step 1: Authentication Abstractions.
