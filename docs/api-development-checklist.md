# ContentOS API Development Checklist

Quick reference checklist for implementing API features.

## ?? Documentation Index

| Document | Purpose |
|----------|---------|
| [api-implementation-roadmap.md](api-implementation-roadmap.md) | High-level roadmap and architecture |
| [feature-controller-guide.md](feature-controller-guide.md) | Detailed implementation guide with examples |
| [database-setup-guide.md](database-setup-guide.md) | Database setup and verification |
| **This document** | Quick checklist for each feature |

---

## Phase 1: Security Foundation ?

### 1.1 Authentication Context

- [ ] Create `IAuthenticationContext` interface
  - Location: `src/core/TechWayFit.ContentOS.Abstractions/Security/`
  - Properties: CurrentUserId, CurrentUserEmail, Roles, Permissions, IsSuperAdmin

- [ ] Create `ITenantContext` interface
  - Location: `src/core/TechWayFit.ContentOS.Abstractions/Security/`
  - Properties: CurrentTenantId, CurrentTenantKey, IsResolved

- [ ] Implement `HttpAuthenticationContext`
  - Location: `src/infrastructure/identity/TechWayFit.ContentOS.Infrastructure.Identity/`
  - Reads from HttpContext claims

- [ ] Implement `HttpTenantContext`
  - Location: `src/infrastructure/identity/TechWayFit.ContentOS.Infrastructure.Identity/`
  - Reads from `X-Tenant-Id` header

### 1.2 Middleware

- [ ] Create `TenantResolutionMiddleware`
  - Location: `src/delivery/api/TechWayFit.ContentOS.Api/Middleware/`
  - Validates `X-Tenant-Id` header on all requests (except health, swagger, admin)

- [ ] Configure middleware pipeline in `Program.cs`
  - Order: TenantResolution ? Authentication ? Authorization

### 1.3 Authorization

- [ ] Configure JWT authentication in `Program.cs`
  - MVP: Header-based with `X-API-Key` support
  - Future: Full OAuth2/OIDC

- [ ] Define authorization policies
  - `SuperAdmin` - Platform administrator
  - `TenantAdmin` - Tenant administrator
  - `Content.Create`, `Content.Edit`, `Content.Publish`, `Content.Delete`

### 1.4 API Response Types

- [ ] Create `ApiResponse<T>` record
  - Location: `src/core/TechWayFit.ContentOS.Contracts/Common/`
  - Properties: Success, Data, Message, Errors, Timestamp

- [ ] Create `PagedResponse<T>` record
  - Properties: Items, TotalCount, PageNumber, PageSize, HasNext/PreviousPage

---

## Phase 2: Tenant & User Management

### 2.1 Tenant Management (SuperAdmin)

#### Domain Layer
- [ ] Create `Tenant` domain entity
  - Location: `src/features/tenancy/TechWayFit.ContentOS.Tenancy/Domain/`
  - Properties: Id, Key, Name, Status, CreatedAt, UpdatedAt

- [ ] Create `ITenantRepository` interface
  - Location: `src/features/tenancy/TechWayFit.ContentOS.Tenancy/Ports/`

#### Contracts
- [ ] Create `CreateTenantRequest` record
- [ ] Create `UpdateTenantRequest` record
- [ ] Create `TenantResponse` record
- [ ] Create validators for requests

#### Use Cases
- [ ] `CreateTenantUseCase` - Create new tenant
- [ ] `ListTenantsUseCase` - List all tenants (SuperAdmin)
- [ ] `GetTenantUseCase` - Get tenant by ID
- [ ] `UpdateTenantUseCase` - Update tenant
- [ ] `DeleteTenantUseCase` - Soft delete tenant

#### API
- [ ] Create `TenantEndpoints` class
  - Location: `src/delivery/api/TechWayFit.ContentOS.Api/Endpoints/Admin/`
  - Route: `/api/admin/tenants`
  - Policy: `SuperAdmin` required

- [ ] Map endpoints in `Program.cs`

#### Tests
- [ ] Unit tests for use cases
- [ ] Integration tests for endpoints

### 2.2 User Management (Tenant-Scoped)

#### Domain Layer
- [ ] Create `User` domain entity
- [ ] Create `IUserRepository` interface

#### Contracts
- [ ] Create request/response DTOs
- [ ] Create validators

#### Use Cases
- [ ] `CreateUserUseCase`
- [ ] `ListUsersUseCase` (tenant-scoped)
- [ ] `GetUserUseCase`
- [ ] `UpdateUserUseCase`
- [ ] `DeleteUserUseCase`

#### API
- [ ] Create `UserEndpoints`
  - Route: `/api/users`
  - Tenant-scoped (requires `X-Tenant-Id`)

- [ ] Map endpoints

#### Tests
- [ ] Unit tests
- [ ] Integration tests

### 2.3 Role Management

Follow same pattern as User Management:

- [ ] Domain: `Role` entity, `IRoleRepository`
- [ ] Contracts: DTOs and validators
- [ ] Use Cases: CRUD operations
- [ ] API: `RoleEndpoints` at `/api/roles`
- [ ] Tests: Unit and integration tests

### 2.4 Group Management

Follow same pattern as Role Management:

- [ ] Domain: `Group` entity, `IGroupRepository`
- [ ] Contracts: DTOs and validators
- [ ] Use Cases: CRUD operations
- [ ] API: `GroupEndpoints` at `/api/groups`
- [ ] Tests: Unit and integration tests

---

## Phase 3: Core Features

### 3.1 Content Type Management

#### Domain Layer
- [ ] `ContentType` entity
- [ ] `ContentTypeField` entity
- [ ] `IContentTypeRepository` interface
- [ ] `IContentTypeFieldRepository` interface

#### Contracts
- [ ] `CreateContentTypeRequest` with fields
- [ ] `UpdateContentTypeRequest`
- [ ] `ContentTypeResponse` with fields
- [ ] `CreateFieldRequest`
- [ ] `FieldResponse`
- [ ] Validators

#### Use Cases
- [ ] `CreateContentTypeUseCase` - Create with fields
- [ ] `ListContentTypesUseCase` - List by tenant
- [ ] `GetContentTypeUseCase` - Get by ID with fields
- [ ] `UpdateContentTypeUseCase` - Update metadata
- [ ] `DeleteContentTypeUseCase` - Soft delete (check for content items)
- [ ] `AddFieldUseCase` - Add field to content type
- [ ] `UpdateFieldUseCase` - Update field definition
- [ ] `RemoveFieldUseCase` - Remove field (check for data)

#### API
- [ ] `ContentTypeEndpoints` at `/api/content-types`
  - POST `/` - Create
  - GET `/` - List (with pagination)
  - GET `/{id}` - Get by ID
  - PUT `/{id}` - Update
  - DELETE `/{id}` - Delete
  - POST `/{id}/fields` - Add field
  - PUT `/{id}/fields/{fieldId}` - Update field
  - DELETE `/{id}/fields/{fieldId}` - Remove field

#### Tests
- [ ] Unit tests for all use cases
- [ ] Integration tests for all endpoints
- [ ] Test validation scenarios
- [ ] Test tenant isolation

### 3.2 Content Node Management (Tree)

#### Domain Layer
- [ ] `ContentNode` entity
- [ ] `IContentNodeRepository` interface
  - Methods: GetChildren, GetTree, Move, GetByPath

#### Contracts
- [ ] `CreateContentNodeRequest`
- [ ] `UpdateContentNodeRequest`
- [ ] `MoveNodeRequest`
- [ ] `ContentNodeResponse`
- [ ] `ContentNodeTreeResponse`

#### Use Cases
- [ ] `CreateContentNodeUseCase`
- [ ] `ListContentNodesUseCase` - List by parent
- [ ] `GetContentNodeUseCase`
- [ ] `GetContentNodeTreeUseCase` - Get tree structure
- [ ] `UpdateContentNodeUseCase`
- [ ] `MoveContentNodeUseCase` - Move in tree
- [ ] `DeleteContentNodeUseCase` - Check for children

#### API
- [ ] `ContentNodeEndpoints` at `/api/content-nodes`
  - POST `/` - Create
  - GET `/` - List (by parent)
  - GET `/{id}` - Get by ID
  - GET `/{id}/children` - Get children
  - GET `/tree` - Get tree structure
  - PUT `/{id}` - Update
  - POST `/{id}/move` - Move node
  - DELETE `/{id}` - Delete

### 3.3 Route Management

#### Domain Layer
- [ ] `Route` entity
- [ ] `IRouteRepository` interface

#### Contracts
- [ ] `CreateRouteRequest`
- [ ] `RouteResponse`

#### Use Cases
- [ ] `CreateRouteUseCase`
- [ ] `GetRouteByPathUseCase` - Resolve route
- [ ] `ListRoutesUseCase`
- [ ] `DeleteRouteUseCase`

#### API
- [ ] `RouteEndpoints` at `/api/routes`

### 3.4 Content Item Management

#### Domain Layer
- [ ] `ContentItem` entity
- [ ] `IContentItemRepository` interface

#### Contracts
- [ ] `CreateContentItemRequest`
- [ ] `UpdateContentItemRequest`
- [ ] `ContentItemResponse`
- [ ] `ContentItemListResponse`

#### Use Cases
- [ ] `CreateContentItemUseCase`
- [ ] `ListContentItemsUseCase` (with filters)
- [ ] `GetContentItemUseCase`
- [ ] `UpdateContentItemUseCase`
- [ ] `DeleteContentItemUseCase`

#### API
- [ ] `ContentItemEndpoints` at `/api/content`
  - POST `/` - Create
  - GET `/` - List (with filters, pagination)
  - GET `/{id}` - Get by ID
  - PUT `/{id}` - Update
  - DELETE `/{id}` - Delete

### 3.5 Content Version Management

#### Domain Layer
- [ ] `ContentVersion` entity
- [ ] `IContentVersionRepository` interface

#### Contracts
- [ ] `CreateVersionRequest`
- [ ] `ContentVersionResponse`

#### Use Cases
- [ ] `CreateContentVersionUseCase`
- [ ] `GetLatestVersionUseCase`
- [ ] `GetPublishedVersionUseCase`
- [ ] `ListVersionsUseCase`

#### API
- [ ] Add to `ContentItemEndpoints`
  - GET `/content/{id}/versions` - List versions
  - GET `/content/{id}/versions/latest` - Get latest
  - GET `/content/{id}/versions/published` - Get published
  - POST `/content/{id}/versions` - Create version

### 3.6 Content Field Value Management

#### Domain Layer
- [ ] `ContentFieldValue` entity
- [ ] `IContentFieldValueRepository` interface

#### Use Cases
- [ ] `UpdateFieldValuesUseCase` - Bulk update
- [ ] `GetFieldValuesUseCase` - Get by version

#### API
- [ ] Integrated into Content Item endpoints (fields in request/response)

---

## Phase 4: Workflow

### 4.1 Workflow Definition Management

#### Domain Layer
- [ ] `WorkflowDefinition` entity
- [ ] `WorkflowState` entity
- [ ] `WorkflowTransition` entity
- [ ] Repositories for each

#### Contracts
- [ ] `CreateWorkflowRequest` with states and transitions
- [ ] `WorkflowResponse`

#### Use Cases
- [ ] `CreateWorkflowUseCase`
- [ ] `ListWorkflowsUseCase`
- [ ] `GetWorkflowUseCase`
- [ ] `SetDefaultWorkflowUseCase`

#### API
- [ ] `WorkflowEndpoints` at `/api/workflows`

### 4.2 Content Workflow Actions

#### Use Cases
- [ ] `TransitionContentUseCase` - Move between states
- [ ] `SubmitForReviewUseCase` - Draft ? Review
- [ ] `ApproveContentUseCase` - Review ? Published
- [ ] `RejectContentUseCase` - Review ? Draft
- [ ] `PublishContentUseCase` - Any ? Published

#### API
- [ ] Add to `ContentItemEndpoints`
  - POST `/content/{id}/submit` - Submit for review
  - POST `/content/{id}/approve` - Approve
  - POST `/content/{id}/reject` - Reject
  - POST `/content/{id}/publish` - Publish
  - GET `/content/{id}/workflow` - Get workflow state

---

## Phase 5: Advanced Features

### 5.1 Layout & Component Management

- [ ] Layout Definitions
- [ ] Component Definitions
- [ ] Content Layouts

### 5.2 Search

- [ ] Full-text search
- [ ] Faceted search
- [ ] Search index management

### 5.3 Media Management

- [ ] File upload
- [ ] Asset management
- [ ] Image transformations

### 5.4 Preview Tokens

- [ ] Generate preview tokens
- [ ] Validate preview tokens
- [ ] Cleanup expired tokens

### 5.5 Audit Logs

- [ ] Query audit logs
- [ ] Export audit logs

---

## Testing Checklist

For each feature:

### Unit Tests
- [ ] Test successful scenarios
- [ ] Test validation failures
- [ ] Test business rule violations
- [ ] Test authorization failures
- [ ] Test tenant isolation
- [ ] Mock dependencies properly

### Integration Tests
- [ ] Test HTTP endpoints
- [ ] Test request/response mapping
- [ ] Test authentication/authorization
- [ ] Test tenant header validation
- [ ] Test error responses
- [ ] Test pagination

### Performance Tests
- [ ] Test with large datasets
- [ ] Test pagination performance
- [ ] Test query performance

---

## Documentation Checklist

For each feature:

- [ ] OpenAPI/Swagger documentation
  - [ ] Summary and description
  - [ ] Request examples
- [ ] Response examples
  - [ ] Error codes

- [ ] README updates
  - [ ] API endpoints listed
  - [ ] Authentication requirements
  - [ ] Example curl commands

- [ ] Architecture documentation
  - [ ] Update diagrams if needed
  - [ ] Document design decisions

---

## Deployment Checklist

- [ ] Environment configuration
  - [ ] Connection strings
  - [ ] JWT secrets
  - [ ] CORS settings

- [ ] Database migrations
  - [ ] Run migrations
  - [ ] Verify schema

- [ ] Health checks
  - [ ] Database connectivity
  - [ ] External services

- [ ] Monitoring
  - [ ] Logging configured
  - [ ] Metrics collection
- [ ] Error tracking

---

## Quick Reference: File Locations

```
src/
??? core/
?   ??? Abstractions/
?   ?   ??? Security/
?   ?   ?   ??? IAuthenticationContext.cs
?   ?   ?   ??? ITenantContext.cs
?   ?   ??? Repositories/
?   ?       ??? IRepository.cs, IUnitOfWork.cs
?   ??? Contracts/
?       ??? Common/
?  ?   ??? ApiResponse.cs
? ??? {Feature}/
?           ??? {Feature}Request.cs
?           ??? {Feature}Response.cs
?    ??? {Feature}Validators.cs
??? features/
?   ??? {feature}/
?       ??? Domain/
?       ?   ??? {Entity}.cs
?       ??? Ports/
?   ?   ??? I{Entity}Repository.cs
?       ??? UseCases/
?           ??? Create{Entity}UseCase.cs
?        ??? Get{Entity}UseCase.cs
?    ??? ...
??? infrastructure/
?   ??? identity/
?   ?   ??? HttpAuthenticationContext.cs
?   ? ??? HttpTenantContext.cs
?   ??? persistence/
?       ??? Postgres/
?           ??? Repositories/
?       ?   ??? {Entity}Repository.cs
?           ??? Configurations/
?         ??? {Entity}Configuration.cs
??? delivery/
    ??? api/
     ??? Endpoints/
      ?   ??? Admin/
        ?   ?   ??? TenantEndpoints.cs
        ?   ??? {Feature}Endpoints.cs
        ??? Middleware/
        ?   ??? TenantResolutionMiddleware.cs
 ??? Program.cs
```

---

## Next Steps

1. **Start with Phase 1** - Security foundation is critical
2. **Implement Phase 2** - Tenant and user management
3. **Build Phase 3** - Core content features
4. **Add Phase 4** - Workflow capabilities
5. **Extend Phase 5** - Advanced features

**Remember**: Each feature should follow the complete checklist before moving to the next one!
