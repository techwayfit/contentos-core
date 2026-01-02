# ContentOS Project Structure

**Generated:** 2 January 2026  
**Solution:** TechWayFit.ContentOS  
**Framework:** .NET 10.0  

---

## Solution Overview

ContentOS is a multi-tenant, enterprise-grade CMS platform following strict layered architecture principles with an organized folder structure for maximum maintainability and clarity.

### Architecture Layers

**Dependency Flow** (arrows represent "depends on" - top depends on bottom):

```
┌─────────────────────────────────────┐
│ Delivery (API / Hosts)              │
└─────────────────────────────────────┘
              ↓ depends on
┌─────────────────────────────────────┐
│ Infrastructure (implementations)    │
└─────────────────────────────────────┘
              ↓ depends on
┌─────────────────────────────────────┐
│ Abstractions (ports)                │
│ Features (bounded contexts)         │
└─────────────────────────────────────┘
              ↓ depends on
┌─────────────────────────────────────┐
│ Kernel (primitives)                 │
│ Contracts (DTOs/Events)             │
└─────────────────────────────────────┘
```

**Dependency Rules:**
- **Kernel** = Pure primitives, **ZERO dependencies**
- **Abstractions** = Pure interfaces/ports, **ZERO dependencies**  
- **Contracts** = DTOs/events, **ZERO dependencies**
- **Features** = Domain logic, depends on: **Abstractions + Kernel + Contracts**
- **Infrastructure** = Implementations, depends on: **Abstractions + Kernel** (+ Features only for Persistence mapping)
- **Delivery (API)** = Transport layer, depends on: **ALL layers** (composition root)

---

## Folder Structure

```
src/
├── core/                                    # Platform primitives & contracts
│   ├── TechWayFit.ContentOS.Kernel/
│   ├── TechWayFit.ContentOS.Abstractions/
│   └── TechWayFit.ContentOS.Contracts/
│
├── features/                                # Domain features (bounded contexts)
│   ├── content/TechWayFit.ContentOS.Content/
│   ├── workflow/TechWayFit.ContentOS.Workflow/
│   ├── media/TechWayFit.ContentOS.Media/
│   ├── search/TechWayFit.ContentOS.Search/
│   └── tenancy/TechWayFit.ContentOS.Tenancy/
│
├── infrastructure/                          # Provider implementations
│   ├── runtime/TechWayFit.ContentOS.Infrastructure.Runtime/
│   ├── persistence/
│   │   ├── TechWayFit.ContentOS.Infrastructure.Persistence/
│   │   └── TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/
│   ├── events/TechWayFit.ContentOS.Infrastructure.Events/
│   ├── identity/TechWayFit.ContentOS.Infrastructure.Identity/
│   ├── search/TechWayFit.ContentOS.Infrastructure.Search/
│   └── storage/TechWayFit.ContentOS.Infrastructure.Storage/
│
└── delivery/                                # API layer
    └── api/TechWayFit.ContentOS.Api/

tests/                                       # Test projects
docs/                                        # Documentation
```

---

## Core Layer (`src/core/`)

### **TechWayFit.ContentOS.Kernel**
**Location:** `src/core/TechWayFit.ContentOS.Kernel/`  
**Purpose:** Domain truth and platform-agnostic primitives  
**Dependencies:** None (only Microsoft.Extensions.DependencyInjection.Abstractions)

```
TechWayFit.ContentOS.Kernel/
├── Localization/
│   └── LanguageCode.cs               # Immutable language/locale value object
├── Primitives/
│   └── Identifiers.cs                # Platform-wide value objects (TenantId, SiteId, UserId, EntityId)
├── Security/
│   └── AdminPermissions.cs           # Platform permission constants
├── DependencyInjection.cs            # Kernel service registration
├── README.md
├── Result.cs                         # Result<TValue, TError> pattern
└── TechWayFit.ContentOS.Kernel.csproj
```

**Architectural Changes:**
- ✅ Security interfaces (IPolicyEvaluator, IRbacService, ISuperAdminContext) → moved to Abstractions
- ✅ LanguageContext (mutable runtime) → moved to Infrastructure.Runtime
- ✅ Contains ONLY primitives and domain constants (zero dependencies)

---

### **TechWayFit.ContentOS.Abstractions**
**Location:** `src/core/TechWayFit.ContentOS.Abstractions/`  
**Purpose:** Pure behavioral contracts (ports/interfaces)  
**Dependencies:** None

```
TechWayFit.ContentOS.Abstractions/
├── Security/
│   ├── IPolicyEvaluator.cs           # Policy evaluation contract
│   ├── IRbacService.cs               # RBAC service contract
│   └── ISuperAdminContext.cs         # SuperAdmin detection
├── IAiOrchestrationService.cs        # AI orchestration contract
├── IAiProvider.cs                    # AI provider contract
├── IAuthorizationPolicy.cs           # Authorization policy contract
├── ICurrentLocaleProvider.cs         # Current locale provider
├── ICurrentTenantProvider.cs         # Current tenant provider
├── ICurrentUserProvider.cs           # Current user provider
├── IEventBus.cs                      # Event bus contract
├── IEventHandler.cs                  # Event handler contract
├── IMigrationRunner.cs               # Migration runner contract
├── IRepository.cs                    # Generic repository contract
├── ITenantResolver.cs                # Tenant resolution contract
├── IUnitOfWork.cs                    # Unit of work contract
├── PagedResult.cs                    # Paging helper
├── README.md
└── TechWayFit.ContentOS.Abstractions.csproj
```

**Purpose:**  
Defines all contracts that infrastructure implementations must fulfill. Feature projects depend on these interfaces, not on concrete implementations.

---

### **TechWayFit.ContentOS.Contracts**
**Location:** `src/core/TechWayFit.ContentOS.Contracts/`  
**Purpose:** DTOs and versioned integration event contracts  
**Dependencies:** None

```
TechWayFit.ContentOS.Contracts/
├── Dtos/
│   ├── AddLocalizationRequest.cs     # Add content localization DTO
│   ├── ContentDto.cs                 # Content data transfer object
│   ├── ContentResponse.cs            # Content API response
│   ├── CreateContentRequest.cs       # Create content DTO
│   ├── Error.cs                      # Error response DTO
│   ├── MediaMetadataResponse.cs      # Media metadata response
│   ├── TenantDto.cs                  # Tenant data transfer object
│   ├── WorkflowStateResponse.cs      # Workflow state response
│   └── WorkflowTransitionRequest.cs  # Workflow transition DTO
├── Events/
│   ├── ContentCreatedEventV1.cs      # Content created integration event V1
│   ├── ContentLocalizedEventV1.cs    # Content localized integration event V1
│   ├── ContentPublishedEventV1.cs    # Content published integration event V1
│   ├── IntegrationEvent.cs           # Base integration event
│   └── WorkflowTransitionedEventV1.cs # Workflow transitioned integration event V1
├── README.md
└── TechWayFit.ContentOS.Contracts.csproj
```

**Event Versioning:**  
All integration events use V1 suffix for versioning. Base class renamed from `DomainEvent` to `IntegrationEvent` for clarity.

---

## Feature Projects (`src/features/`)

### **TechWayFit.ContentOS.Content**
**Location:** `src/features/content/TechWayFit.ContentOS.Content/`  
**Purpose:** Content management bounded context  
**Dependencies:** Abstractions, Contracts, Kernel

```
TechWayFit.ContentOS.Content/
├── Application/
│   ├── AddLocalizationUseCase.cs     # Add localization use case
│   ├── CreateContentUseCase.cs       # Create content use case
│   ├── IAddLocalizationUseCase.cs    # Add localization interface
│   └── ICreateContentUseCase.cs      # Create content interface
├── Domain/
│   ├── ContentItem.cs                # Content aggregate root
│   ├── ContentItemId.cs              # Content item value object
│   ├── ContentLocalization.cs        # Content localization entity
│   ├── ContentLocalizationId.cs      # Localization ID value object
│   ├── ValueObjects.cs               # Content-specific value objects
│   └── WorkflowStatus.cs             # Workflow status enum
├── Ports/
│   └── IContentRepository.cs         # Content repository contract
├── IContentSchemaService.cs          # Schema service contract
├── IContentService.cs                # Content service contract
├── README.md
└── TechWayFit.ContentOS.Content.csproj
```

**Key Points:**
- ❌ NO Infrastructure dependencies
- ✅ Use-cases inject `ICurrentUserProvider`, `ICurrentTenantProvider`
- ✅ Domain entities stay pure (no persistence concerns)

---

### **TechWayFit.ContentOS.Workflow**
**Location:** `src/features/workflow/TechWayFit.ContentOS.Workflow/`  
**Purpose:** Workflow and state management bounded context  
**Dependencies:** Abstractions, Content, Contracts, Kernel

```
TechWayFit.ContentOS.Workflow/
├── Application/
│   ├── ITransitionWorkflowUseCase.cs # Transition interface
│   └── TransitionWorkflowUseCase.cs  # Transition use case
├── Domain/
│   ├── WorkflowState.cs              # Workflow state aggregate
│   └── WorkflowStateId.cs            # State ID value object
├── Ports/
│   └── IWorkflowRepository.cs        # Workflow repository contract
├── IWorkflowService.cs               # Workflow service contract
├── README.md
├── WorkflowStates.cs                 # Workflow state constants
└── TechWayFit.ContentOS.Workflow.csproj
```

---

### **TechWayFit.ContentOS.Media**
**Location:** `src/features/media/TechWayFit.ContentOS.Media/`  
**Purpose:** Media management bounded context  
**Dependencies:** Abstractions, Contracts, Kernel

```
TechWayFit.ContentOS.Media/
├── Application/
│   ├── GetMediaMetadataUseCase.cs    # Get metadata use case
│   └── IGetMediaMetadataUseCase.cs   # Get metadata interface
├── DependencyInjection.cs            # Media service registration
├── IMediaService.cs                  # Media service contract
├── README.md
└── TechWayFit.ContentOS.Media.csproj
```

---

### **TechWayFit.ContentOS.Search**
**Location:** `src/features/search/TechWayFit.ContentOS.Search/`  
**Purpose:** Search domain models and query contracts  
**Dependencies:** Abstractions, Contracts

```
TechWayFit.ContentOS.Search/
├── Application/
│   └── .gitkeep                      # Search use cases
├── Domain/
│   └── .gitkeep                      # Search domain models
├── Ports/
│   └── .gitkeep                      # Search provider interfaces
├── DependencyInjection.cs            # Search service registration
├── ISearchService.cs                 # Search service contract
├── README.md
└── TechWayFit.ContentOS.Search.csproj
```

**Separation of Concerns:**
- This feature defines WHAT to search (domain models, query contracts)
- Infrastructure.Search defines HOW to index (event handlers, providers)

---

### **TechWayFit.ContentOS.Tenancy**
**Location:** `src/features/tenancy/TechWayFit.ContentOS.Tenancy/`  
**Purpose:** Tenant management bounded context (SuperAdmin only)  
**Dependencies:** Abstractions, Kernel

```
TechWayFit.ContentOS.Tenancy/
├── Application/
│   ├── CreateTenantUseCase.cs        # Create tenant use case
│   ├── GetTenantUseCase.cs           # Get tenant use case
│   ├── ListTenantsUseCase.cs         # List tenants use case
│   └── UpdateTenantUseCase.cs        # Update tenant use case
├── Domain/
│   ├── Tenant.cs                     # Tenant aggregate root
│   └── TenantStatus.cs               # Tenant status enum
├── Ports/
│   └── ITenantRepository.cs          # Tenant repository contract
├── DependencyInjection.cs            # Tenancy service registration
├── README.md
└── TechWayFit.ContentOS.Tenancy.csproj
```

**Note:** Tenant management APIs do NOT require `X-Tenant-Id` header (SuperAdmin scope only).

---

## Infrastructure Layer (`src/infrastructure/`)

### **TechWayFit.ContentOS.Infrastructure.Runtime**
**Location:** `src/infrastructure/runtime/TechWayFit.ContentOS.Infrastructure.Runtime/`  
**Purpose:** Concrete runtime context implementations  
**Dependencies:** Abstractions, Kernel

```
TechWayFit.ContentOS.Infrastructure.Runtime/
├── ApiRequestContext.cs              # API request context
├── CurrentUser.cs                    # Implements ICurrentUserProvider
├── DependencyInjection.cs            # Runtime service registration
├── LanguageContext.cs                # Implements ICurrentLocaleProvider
├── MvpPolicyEvaluator.cs             # Implements IPolicyEvaluator (MVP)
├── RbacService.cs                    # Implements IRbacService
├── README.md
├── SuperAdminContext.cs              # Implements ISuperAdminContext (MVP)
├── TenantContext.cs                  # Implements ICurrentTenantProvider
└── TechWayFit.ContentOS.Infrastructure.Runtime.csproj
```

**Key Changes:**
- All classes implement corresponding Abstractions interfaces
- Feature projects inject `ICurrentUserProvider` (not concrete `CurrentUser`)
- `LanguageContext` moved here from Kernel (mutable runtime state)

---

### **TechWayFit.ContentOS.Infrastructure.Persistence**
**Location:** `src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence/`  
**Purpose:** Provider-agnostic persistence abstractions  
**Dependencies:** Abstractions, Contracts, Kernel

```
TechWayFit.ContentOS.Infrastructure.Persistence/
├── Conventions/
│   └── .gitkeep                      # DB naming conventions
├── Options/
│   └── DatabaseOptions.cs            # Database configuration options
├── README.md
└── TechWayFit.ContentOS.Infrastructure.Persistence.csproj
```

**Note:** NO DbContext, NO EF migrations, NO concrete implementations.

---

### **TechWayFit.ContentOS.Infrastructure.Persistence.Postgres**
**Location:** `src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/`  
**Purpose:** PostgreSQL-specific EF Core implementation  
**Dependencies:** Abstractions, Content, Contracts, Infrastructure.Persistence, Kernel, Media, Search, Tenancy, Workflow

```
TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/
├── Configurations/
│   ├── ContentItemConfiguration.cs    # ContentItemRow EF configuration
│   ├── ContentLocalizationConfiguration.cs
│   ├── TenantConfiguration.cs
│   ├── WorkflowStateConfiguration.cs
│   └── .gitkeep
├── Entities/
│   ├── ContentItemRow.cs              # Content DB row entity
│   ├── ContentLocalizationRow.cs      # Localization DB row entity
│   ├── TenantRow.cs                   # Tenant DB row entity
│   ├── WorkflowStateRow.cs            # Workflow state DB row entity
│   └── .gitkeep
├── Migrations/
│   ├── 20250116200000_InitialMigration.cs
│   ├── 20250116200000_InitialMigration.Designer.cs
│   ├── 20250117133656_AddLocalizations.cs
│   ├── 20250117133656_AddLocalizations.Designer.cs
│   └── ContentDbContextModelSnapshot.cs
├── Repositories/
│   ├── ContentRepository.cs           # IContentRepository implementation
│   ├── TenantRepository.cs            # ITenantRepository implementation
│   ├── WorkflowRepository.cs          # IWorkflowRepository implementation
│   └── .gitkeep
├── ContentDbContext.cs                # EF Core DbContext
├── DependencyInjection.cs             # Persistence service registration
├── PostgresMigrationRunner.cs         # IMigrationRunner implementation
├── README.md
├── UnitOfWork.cs                      # IUnitOfWork implementation
└── TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.csproj
```

**Architecture Rules:**
- ✅ ALL EF code lives here (DbContext, migrations, Row entities, configurations)
- ✅ Domain → DB mapping happens in repositories
- ✅ Feature projects NEVER reference this project
- ✅ All tables include `tenant_id` with global query filters
- ✅ DB entities end with `Row` suffix (e.g., `ContentItemRow`, `TenantRow`)

**Multi-Tenancy Implementation:**
```csharp
// Global query filter in DbContext
modelBuilder.Entity<ContentItemRow>()
    .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);

// Uniqueness scoped by tenant
CREATE UNIQUE INDEX idx_content_slug ON content_items (tenant_id, slug);
```

---

### **TechWayFit.ContentOS.Infrastructure.Events**
**Location:** `src/infrastructure/events/TechWayFit.ContentOS.Infrastructure.Events/`  
**Purpose:** Event bus implementation  
**Dependencies:** Abstractions, Kernel

```
TechWayFit.ContentOS.Infrastructure.Events/
├── DependencyInjection.cs            # Event bus service registration
├── InMemoryEventBus.cs               # IEventBus MVP implementation
├── README.md
└── TechWayFit.ContentOS.Infrastructure.Events.csproj
```

**Note:** MVP uses in-memory event bus. Future: RabbitMQ/Azure Service Bus.

---

### **TechWayFit.ContentOS.Infrastructure.Identity**
**Location:** `src/infrastructure/identity/TechWayFit.ContentOS.Infrastructure.Identity/`  
**Purpose:** Authentication & authorization infrastructure  
**Dependencies:** Abstractions, Kernel

```
TechWayFit.ContentOS.Infrastructure.Identity/
├── DependencyInjection.cs            # Identity service registration
├── README.md
└── TechWayFit.ContentOS.Infrastructure.Identity.csproj
```

**Note:** MVP uses header-based auth (`X-Tenant-Id`, `X-SuperAdmin`). Future: JWT/OAuth2.

---

### **TechWayFit.ContentOS.Infrastructure.Search**
**Location:** `src/infrastructure/search/TechWayFit.ContentOS.Infrastructure.Search/`  
**Purpose:** Search provider implementations and event-driven indexing  
**Dependencies:** Abstractions, Contracts, Kernel

```
TechWayFit.ContentOS.Infrastructure.Search/
├── EventHandlers/
│   └── ContentPublishedIndexer.cs    # Content indexing event handler
├── Models/
│   └── .gitkeep                      # Search index models
├── Providers/
│   └── .gitkeep                      # Search provider implementations
├── DependencyInjection.cs            # Search service registration
├── README.md
└── TechWayFit.ContentOS.Infrastructure.Search.csproj
```

**Architecture Rules (CRITICAL):**
- ❌ NO dependencies on Content or Search feature projects
- ✅ Subscribes to integration events from Contracts (`ContentCreatedEventV1`, `ContentPublishedEventV1`)
- ✅ Uses event payload DTOs to build index documents (event-driven + contract-driven)
- ✅ If extra data needed, uses ports from Abstractions (e.g., `IContentReadApi`), NOT domain assemblies
- ✅ Keeps infrastructure decoupled from domain logic

**Event-Driven Pattern:**
```csharp
public class ContentPublishedIndexer : IEventHandler<ContentPublishedEventV1>
{
    public Task HandleAsync(ContentPublishedEventV1 @event, CancellationToken ct)
    {
        // Build search document from event DTO fields
        var document = new SearchDocument
        {
            ContentId = @event.ContentId,
            TenantId = @event.TenantId,
            Title = @event.Title,
            Slug = @event.Slug,
            // Use event payload, NOT domain entities
        };
        
        return _searchProvider.IndexAsync(document, ct);
    }
}
```

---

### **TechWayFit.ContentOS.Infrastructure.Storage**
**Location:** `src/infrastructure/storage/TechWayFit.ContentOS.Infrastructure.Storage/`  
**Purpose:** Blob storage provider implementations  
**Dependencies:** Abstractions, Kernel

```
TechWayFit.ContentOS.Infrastructure.Storage/
├── DependencyInjection.cs            # Storage service registration
├── README.md
└── TechWayFit.ContentOS.Infrastructure.Storage.csproj
```

**Note:** Future: Azure Blob Storage, S3, local file system.

---

## Delivery Layer (`src/delivery/`)

### **TechWayFit.ContentOS.Api**
**Location:** `src/delivery/api/TechWayFit.ContentOS.Api/`  
**Purpose:** Thin HTTP transport layer (Minimal APIs)  
**Dependencies:** All projects (composition root)

```
TechWayFit.ContentOS.Api/
├── Context/
│   └── TenantResolutionMiddleware.cs  # Tenant resolution from X-Tenant-Id
├── Controllers/
│   ├── ContentController.cs           # Content HTTP endpoints
│   ├── TenantController.cs            # Tenant HTTP endpoints (SuperAdmin only)
│   └── WorkflowController.cs          # Workflow HTTP endpoints
├── appsettings.Development.json       # Development settings
├── appsettings.json                   # Default settings
├── appsettings.local.json             # Local overrides (gitignored)
├── Program.cs                         # Application entry point & DI composition root
├── README.md
├── TechWayFit.ContentOS.Api.csproj
├── TechWayFit.ContentOS.Api.http      # HTTP client test file (content endpoints)
└── TechWayFit.ContentOS.Api.Tenants.http # HTTP client test file (tenant endpoints)
```

**Architecture Rules:**
- ❌ NO EF Core usage
- ❌ NO DbContext
- ❌ NO SQL queries
- ❌ NO Repository implementations
- ✅ Endpoints call use-cases, not repositories directly
- ✅ DTO ↔ command mapping only
- ✅ DI composition root
- ✅ Middleware for cross-cutting concerns

**Example Endpoint:**
```csharp
app.MapPost("/content", async (
    CreateContentRequest request,
    ICreateContentUseCase useCase) =>
{
    var result = await useCase.ExecuteAsync(request);
    return result.IsSuccess 
        ? Results.Ok(result.Value) 
        : Results.BadRequest(result.Error);
})
.RequireAuthorization("content:create");
```

---

## Test Projects (`tests/`)

All test projects mirror source structure with `.Tests` suffix:

```
tests/
├── TechWayFit.ContentOS.Abstractions.Tests/
├── TechWayFit.ContentOS.Api.Tests/
├── TechWayFit.ContentOS.Content.Tests/
├── TechWayFit.ContentOS.Contracts.Tests/
├── TechWayFit.ContentOS.Kernel.Tests/
├── TechWayFit.ContentOS.Media.Tests/
├── TechWayFit.ContentOS.Search.Tests/
└── TechWayFit.ContentOS.Workflow.Tests/
```

**Test Dependencies:**
- xUnit, Moq, FluentAssertions
- Reference corresponding source project
- Use relative paths to new folder structure

---

## Architecture Guidelines

### Dependency Rules (Enforced by NetArchTest)

```
┌─────────────────────────────────────────┐
│ Delivery (API / Hosts)                  │  ← ALL layers (composition root)
└─────────────────────────────────────────┘
              ↓ depends on
┌─────────────────────────────────────────┐
│ Infrastructure (implementations)        │  ← Abstractions + Kernel (+ Features for mapping only)
└─────────────────────────────────────────┘
              ↓ depends on
┌─────────────────────────────────────────┐
│ Abstractions (ports)                    │  ← ZERO dependencies
│ Features (bounded contexts)             │  ← Abstractions + Kernel + Contracts
└─────────────────────────────────────────┘
              ↓ depends on
┌─────────────────────────────────────────┐
│ Kernel (primitives)                     │  ← ZERO dependencies
│ Contracts (DTOs/Events)                 │  ← ZERO dependencies
└─────────────────────────────────────────┘
```

**Critical Rules:**

1. **Feature → Infrastructure:** ❌ FORBIDDEN
   - Features use `ICurrentUserProvider`, NOT `CurrentUser`
   - Features use `IContentRepository`, NOT `ContentRepository`

2. **Kernel → Anything:** ❌ FORBIDDEN
   - Pure domain primitives only
   - No runtime state, services, or implementations

3. **Abstractions → Anything:** ❌ FORBIDDEN
   - Pure interfaces and contracts only

4. **Infrastructure → Features:** ✅ ALLOWED (with restrictions)
   - **Persistence.Postgres** may reference Features for domain mapping
   - **Search/Events/Storage** should use Contracts, NOT Features
   - Event-driven infrastructure uses integration events from Contracts

5. **API → Everything:** ✅ ALLOWED
   - Composition root wires all implementations
   - Must NOT use DbContext or EF directly

---

### Domain Entity vs DB Entity Separation

**Domain Entities** (in Feature Projects):
```csharp
// src/features/content/TechWayFit.ContentOS.Content/Domain/ContentItem.cs
public class ContentItem 
{
    public ContentItemId Id { get; private set; }
    public string Title { get; private set; }
    
    // Rich domain logic, invariants, business rules
    public void UpdateTitle(string newTitle)
    {
        if (string.IsNullOrWhiteSpace(newTitle))
            throw new ArgumentException("Title required");
        Title = newTitle;
    }
}
```

**DB Entities** (in Infrastructure.Persistence.Postgres):
```csharp
// src/infrastructure/persistence/Postgres/Entities/ContentItemRow.cs
public class ContentItemRow 
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Simple DTO-like structure, no business logic
}
```

**Mapping** (in Infrastructure.Persistence.Postgres/Repositories):
```csharp
private ContentItem MapToDomain(ContentItemRow row) 
{
    return new ContentItem(
        new ContentItemId(row.Id),
        row.Title,
        new TenantId(row.TenantId)
    );
}

private ContentItemRow MapToRow(ContentItem entity) 
{
    return new ContentItemRow
    {
        Id = entity.Id.Value,
        TenantId = entity.TenantId.Value,
        Title = entity.Title
    };
}
```

---

### Multi-Tenancy Rules

**MANDATORY for all persisted entities:**
- Every entity must include `TenantId` property
- Every table must have `tenant_id` column
- All uniqueness constraints scoped by tenant: `UNIQUE (tenant_id, slug)`
- DbContext must apply global query filters for `TenantId`

**Tenant Resolution:**
- MVP: `X-Tenant-Id` header
- SuperAdmin routes: tenant-agnostic (no header required)

**Global Query Filter Example:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<ContentItemRow>()
        .HasQueryFilter(e => e.TenantId == _tenantContext.TenantId);
        
    modelBuilder.Entity<ContentItemRow>()
        .HasIndex(e => new { e.TenantId, e.Slug })
        .IsUnique();
}
```

---

### Security & Authorization

**Permissions:**
- Feature permissions: `content:create`, `workflow:transition`
- Platform permissions: `platform:superadmin`

**Policy Enforcement:**
- All checks go through `IPolicyEvaluator`
- Feature code declares required permission
- API enforces via authorization middleware

**MVP Implementation:**
```csharp
// MVP: Header-based
X-SuperAdmin: true

// Future: JWT claims
{
  "scope": ["content:create", "content:read"],
  "role": "editor",
  "tenant_id": "guid"
}
```

---

### Events & Workflow

**Integration Events** (in Contracts):
```csharp
public class ContentPublishedEventV1 : IntegrationEvent
{
    public Guid ContentId { get; init; }
    public Guid TenantId { get; init; }
    public string Title { get; init; }
    public string Slug { get; init; }
    public string LanguageCode { get; init; }
}
```

**Publishing** (in Use-Cases):
```csharp
public class CreateContentUseCase : ICreateContentUseCase
{
    public async Task<Result<ContentResponse>> ExecuteAsync(CreateContentRequest request)
    {
        var contentItem = new ContentItem(...);
        await _contentRepository.AddAsync(contentItem);
        await _unitOfWork.SaveChangesAsync();
        
        // Publish integration event
        await _eventBus.PublishAsync(new ContentCreatedEventV1
        {
            ContentId = contentItem.Id.Value,
            Title = contentItem.Title,
            TenantId = contentItem.TenantId.Value
        });
        
        return Result.Success(response);
    }
}
```

**Event Handling** (in Infrastructure):
```csharp
public class ContentPublishedIndexer : IEventHandler<ContentPublishedEventV1>
{
    public async Task HandleAsync(ContentPublishedEventV1 @event, CancellationToken ct)
    {
        // React to event using DTO data (no domain coupling)
        await _searchProvider.IndexAsync(new SearchDocument
        {
            Id = @event.ContentId,
            Title = @event.Title,
            Slug = @event.Slug
        }, ct);
    }
}
```

---

### Project Reference Examples

**Feature Project (.csproj):**
```xml
<ItemGroup>
  <!-- Core dependencies only -->
  <ProjectReference Include="../../../core/TechWayFit.ContentOS.Abstractions/TechWayFit.ContentOS.Abstractions.csproj" />
  <ProjectReference Include="../../../core/TechWayFit.ContentOS.Kernel/TechWayFit.ContentOS.Kernel.csproj" />
  <ProjectReference Include="../../../core/TechWayFit.ContentOS.Contracts/TechWayFit.ContentOS.Contracts.csproj" />
  
  <!-- ❌ NO Infrastructure references -->
</ItemGroup>
```

**Infrastructure.Search (.csproj):**
```xml
<ItemGroup>
  <!-- ONLY Abstractions + Contracts + Kernel -->
  <ProjectReference Include="../../../core/TechWayFit.ContentOS.Abstractions/TechWayFit.ContentOS.Abstractions.csproj" />
  <ProjectReference Include="../../../core/TechWayFit.ContentOS.Contracts/TechWayFit.ContentOS.Contracts.csproj" />
  <ProjectReference Include="../../../core/TechWayFit.ContentOS.Kernel/TechWayFit.ContentOS.Kernel.csproj" />
  
  <!-- ❌ NO Content or Search feature references -->
</ItemGroup>
```

**Infrastructure.Persistence.Postgres (.csproj):**
```xml
<ItemGroup>
  <!-- Core -->
  <ProjectReference Include="../../../core/TechWayFit.ContentOS.Abstractions/TechWayFit.ContentOS.Abstractions.csproj" />
  <ProjectReference Include="../../../core/TechWayFit.ContentOS.Kernel/TechWayFit.ContentOS.Kernel.csproj" />
  <ProjectReference Include="../../../core/TechWayFit.ContentOS.Contracts/TechWayFit.ContentOS.Contracts.csproj" />
  
  <!-- Sibling infrastructure -->
  <ProjectReference Include="../TechWayFit.ContentOS.Infrastructure.Persistence/TechWayFit.ContentOS.Infrastructure.Persistence.csproj" />
  
  <!-- Features (for domain entity mapping ONLY) -->
  <ProjectReference Include="../../../features/content/TechWayFit.ContentOS.Content/TechWayFit.ContentOS.Content.csproj" />
  <ProjectReference Include="../../../features/tenancy/TechWayFit.ContentOS.Tenancy/TechWayFit.ContentOS.Tenancy.csproj" />
  <ProjectReference Include="../../../features/workflow/TechWayFit.ContentOS.Workflow/TechWayFit.ContentOS.Workflow.csproj" />
</ItemGroup>
```

**API (.csproj):**
```xml
<ItemGroup>
  <!-- Core -->
  <ProjectReference Include="../../core/TechWayFit.ContentOS.Abstractions/TechWayFit.ContentOS.Abstractions.csproj" />
  <ProjectReference Include="../../core/TechWayFit.ContentOS.Kernel/TechWayFit.ContentOS.Kernel.csproj" />
  <ProjectReference Include="../../core/TechWayFit.ContentOS.Contracts/TechWayFit.ContentOS.Contracts.csproj" />
  
  <!-- Features -->
  <ProjectReference Include="../../features/content/TechWayFit.ContentOS.Content/TechWayFit.ContentOS.Content.csproj" />
  <ProjectReference Include="../../features/tenancy/TechWayFit.ContentOS.Tenancy/TechWayFit.ContentOS.Tenancy.csproj" />
  <ProjectReference Include="../../features/workflow/TechWayFit.ContentOS.Workflow/TechWayFit.ContentOS.Workflow.csproj" />
  
  <!-- Infrastructure (for DI wiring) -->
  <ProjectReference Include="../../infrastructure/runtime/TechWayFit.ContentOS.Infrastructure.Runtime/TechWayFit.ContentOS.Infrastructure.Runtime.csproj" />
  <ProjectReference Include="../../infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.csproj" />
  <ProjectReference Include="../../infrastructure/events/TechWayFit.ContentOS.Infrastructure.Events/TechWayFit.ContentOS.Infrastructure.Events.csproj" />
  <ProjectReference Include="../../infrastructure/search/TechWayFit.ContentOS.Infrastructure.Search/TechWayFit.ContentOS.Infrastructure.Search.csproj" />
</ItemGroup>
```

---

## Architecture Testing (NetArchTest)

**Example Tests:**
```csharp
[Fact]
public void FeatureProjects_ShouldNotDependOn_Infrastructure()
{
    var result = Types.InAssembly(typeof(ContentItem).Assembly)
        .ShouldNot()
        .HaveDependencyOn("TechWayFit.ContentOS.Infrastructure")
        .GetResult();
        
    result.IsSuccessful.Should().BeTrue();
}

[Fact]
public void Kernel_ShouldNotDependOn_Anything()
{
    var result = Types.InAssembly(typeof(TenantId).Assembly)
        .ShouldNot()
        .HaveDependencyOnAny(
            "TechWayFit.ContentOS.Abstractions", 
            "TechWayFit.ContentOS.Infrastructure")
        .GetResult();
        
    result.IsSuccessful.Should().BeTrue();
}

[Fact]
public void InfrastructureSearch_ShouldNotDependOn_FeatureProjects()
{
    var result = Types.InAssembly(typeof(ContentPublishedIndexer).Assembly)
        .ShouldNot()
        .HaveDependencyOnAny(
            "TechWayFit.ContentOS.Content",
            "TechWayFit.ContentOS.Search")
        .GetResult();
        
    result.IsSuccessful.Should().BeTrue();
}
```

---

## Build & Run

**Restore dependencies:**
```bash
dotnet restore
```

**Build solution:**
```bash
dotnet build
```

**Run migrations:**
```bash
cd src/delivery/api/TechWayFit.ContentOS.Api
dotnet ef database update \
  --project ../../infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres
```

**Run API:**
```bash
cd src/delivery/api/TechWayFit.ContentOS.Api
dotnet run
```

**Run tests:**
```bash
dotnet test
```

---

## Summary of Architectural Changes

### ✅ Completed Fixes (2 January 2026):

1. **Kernel Security Interfaces → Abstractions**
   - Moved `IPolicyEvaluator`, `IRbacService`, `ISuperAdminContext` to Abstractions
   - Implementations remain in Infrastructure.Runtime

2. **Runtime Context Providers**
   - Created `ICurrentUserProvider`, `ICurrentTenantProvider`, `ICurrentLocaleProvider` in Abstractions
   - Infrastructure.Runtime implements these interfaces
   - Feature projects inject interfaces, NOT concrete implementations

3. **LanguageContext Split**
   - `LanguageCode` (immutable value object) remains in Kernel
   - `LanguageContext` (mutable runtime) moved to Infrastructure.Runtime

4. **Feature → Infrastructure Dependencies**
   - Removed ALL Infrastructure.Runtime dependencies from feature projects
   - Removed Infrastructure.Search dependency from Search feature
   - Feature projects depend ONLY on Abstractions

5. **Infrastructure.Search Boundary**
   - NO dependencies on Content or Search feature projects
   - Event-driven indexing using Contracts integration events
   - Contract-driven approach (uses DTOs from events, not domain entities)

6. **Integration Events**
   - All events versioned with V1 suffix
   - Base class renamed from `DomainEvent` to `IntegrationEvent`
   - Clear distinction between domain events and integration events

7. **Folder Organization**
   - Reorganized `src/` into: `core/`, `features/`, `infrastructure/`, `delivery/`
   - All project references updated to new paths
   - Solution file regenerated
   - Build verified (0 errors)

8. **Architecture Diagrams**
   - Fixed dependency flow arrows (top depends on bottom)
   - Corrected layer positioning (Infrastructure above Features, not below)
   - Consistent arrow meaning throughout documentation

---

## Next Steps

1. **Architecture Tests:** Implement NetArchTest rules to enforce boundaries
2. **JWT Authentication:** Replace header-based MVP with JWT
3. **RBAC Implementation:** Build proper role-based access control
4. **Event Bus:** Replace in-memory bus with RabbitMQ/Azure Service Bus
5. **Search Integration:** Implement Elasticsearch/Azure Search provider
6. **Media Storage:** Implement Azure Blob Storage/S3 provider
7. **API Documentation:** Add Swagger/OpenAPI
8. **Integration Tests:** Add end-to-end API tests with test containers

---

## References

- [Architecture Decision Records (ADRs)](./docs/adr/adr-index.md)
- [Architecture Overview](./docs/architecture.md)
- [Module Boundaries](./docs/module-boundaries.md)
- [Extension Model](./docs/extension-model.md)
- [Architecture Guard](./docs/architecture-guard.md)

---

**Last Updated:** 2 January 2026  
**Architecture Version:** 2.0 (Organized Folder Structure)  
**Build Status:** ✅ Success
