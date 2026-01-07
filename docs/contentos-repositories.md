# ContentOS Repository Architecture

## Overview

This document defines the **architectural decisions, patterns, and implementation status** of the repository layer in ContentOS. The repository pattern provides clean separation between domain logic and data persistence, enabling infrastructure independence while maintaining a consistent API across all features.

---

## Architectural Decisions

### 1. Pure POCO Pattern for Domain Entities

**Decision:** All domain entities use **pure POCOs** (Plain Old CLR Objects) with public property setters.

**Rationale:**
- **Simplicity:** No reflection required for mapping between database entities and domain entities
- **Framework compatibility:** Works seamlessly with EF Core, serializers, test frameworks, and dependency injection
- **Developer experience:** Intuitive object initialization, easier to reason about
- **Validation location:** Business rules and validation belong in the **use-case layer**, not domain entities
- **CMS context:** ContentOS is a content management system, not a complex business domain requiring strict DDD aggregate boundaries

**Rejected Alternative:** Rich domain models with private setters, factory methods, and encapsulated invariants
- **Why rejected:** Added complexity through reflection-based mapping, reduced developer productivity, no significant benefit for CMS scenarios

**Example:**
```csharp
// ✅ CORRECT: Pure POCO
public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public TenantStatus Status { get; set; }
    public bool IsActive { get; set; }
    // ... other properties
}

// ❌ INCORRECT: DDD-style entity with private setters
public class Tenant
{
    private Tenant() { }
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    
    public static Result<Tenant> Create(string name) 
    {
        // Factory method with validation
    }
}
```

---

### 2. Three-Layer Persistence Architecture

**Layer Structure:**

```
┌─────────────────────────────────────────────────────────┐
│ Feature Projects (Tenancy, Content, Workflow)           │
│ - Domain entities (POCOs)                               │
│ - Repository interfaces (Ports)                         │
│ - Use-cases (validation + orchestration)                │
└─────────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│ Infrastructure.Persistence (Provider-Agnostic)          │
│ - DB entities (Row classes)                             │
│ - EfCoreRepository<T> base class                        │
│ - Repository implementations (simple mapping)           │
└─────────────────────────────────────────────────────────┘
                         ↓
┌─────────────────────────────────────────────────────────┐
│ Infrastructure.Persistence.Postgres (Provider-Specific) │
│ - ContentOsDbContext                                    │
│ - EF Core configurations                                │
│ - Provider-specific overrides (ILike, FTS, etc.)        │
│ - Migrations                                            │
└─────────────────────────────────────────────────────────┘
```

**Decision:** Separate provider-agnostic abstractions (Persistence) from provider-specific implementations (Persistence.Postgres).

**Rationale:**
- **Extensibility:** Can add MySQL, SQLite, or NoSQL providers without changing feature code
- **Testability:** Can mock repositories or use in-memory providers for testing
- **Clean boundaries:** Feature projects never reference EF Core, Npgsql, or database-specific concerns
- **Migration path:** Enables future polyglot persistence (content in Postgres, media in S3, search in Elasticsearch)

---

### 3. Repository Mapping Pattern

**Pattern:** Simple object initializer mapping (no reflection, no AutoMapper)

**Implementation:**
```csharp
protected override Tenant MapToDomain(TenantRow row)
{
    return new Tenant
    {
        Id = row.Id,
        Name = row.Name,
        Status = row.Status,
        // ... all properties explicitly mapped
    };
}

protected override TenantRow MapToRow(Tenant entity)
{
    return new TenantRow
    {
        Id = entity.Id,
        Name = entity.Name,
        Status = entity.Status,
        // ... all properties explicitly mapped
    };
}
```

**Rationale:**
- **Explicit:** Clear visibility of what's being mapped
- **Type-safe:** Compiler catches mapping errors
- **Performant:** No reflection overhead
- **Debuggable:** Easy to step through and inspect

**Rejected Alternative:** Reflection-based mapping or AutoMapper
- **Why rejected:** Added complexity, hidden magic, performance overhead, harder to debug

---

### 4. Multi-Tenancy Enforcement

**Decision:** All entities (except `Tenant`) must include `TenantId` property and column.

**Enforcement:**
- **Database level:** Global query filters in `DbContext.OnModelCreating()`
- **Repository level:** All queries automatically filtered by current tenant
- **API level:** Tenant resolution via `X-Tenant-Id` header (MVP) or JWT claims (future)

**Example:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Global query filter for multi-tenancy
    modelBuilder.Entity<ContentItemRow>()
        .HasQueryFilter(e => e.TenantId == _currentTenant.TenantId);
}
```

**Constraints:**
- All uniqueness constraints must be scoped by `TenantId`:
  ```sql
  UNIQUE (tenant_id, slug) WHERE deleted_on IS NULL
  ```

---

### 5. Soft Delete Pattern

**Decision:** All entities use soft deletes with audit trail.

**Implementation:**
- `IsDeleted` (bool) - Flag for soft delete status
- `DeletedOn` (DateTime?) - Timestamp of deletion
- `DeletedBy` (Guid?) - User who deleted the entity

**Database Constraints:**
- Uniqueness constraints include `WHERE deleted_on IS NULL`
- Foreign keys use `ON DELETE RESTRICT` or `ON DELETE SET NULL` (never `CASCADE` unless intentional)

**Repository Behavior:**
- Default queries exclude soft-deleted entities via global query filters
- Special methods like `GetDeletedAsync()` can retrieve deleted entities for audit/recovery

---

### 6. Unit of Work Pattern

**Decision:** Repositories **do not call `SaveChanges()`** - use-cases are responsible.

**Rationale:**
- **Transaction control:** Use-cases control transaction boundaries
- **Batch operations:** Multiple repository operations can be committed together
- **Consistency:** All changes are atomic within a use-case

**Example:**
```csharp
public class CreateContentItemUseCase
{
    private readonly IContentItemRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    
    public async Task<Result<Guid>> ExecuteAsync(CreateContentItemCommand cmd)
    {
        var item = new ContentItem { ... };
        await _repo.AddAsync(item);
        await _unitOfWork.SaveChangesAsync(); // ✅ Use-case commits
        return Result.Success(item.Id);
    }
}
```

---

### 7. Folder Organization

**Decision:** Domain entities and repository interfaces are organized by **database module grouping**.

**Structure:**
```
TechWayFit.ContentOS.Tenancy/
├── Domain/
│   ├── Core/
│   │   ├── Site.cs
│   │   └── Tenant.cs
│   └── Identity/
│       ├── User.cs
│       ├── Role.cs
│       └── Group.cs
└── Ports/
    ├── Core/
    │   ├── ISiteRepository.cs
    │   └── ITenantRepository.cs
    └── Identity/
        ├── IUserRepository.cs
        ├── IRoleRepository.cs
        └── IGroupRepository.cs
```

**Rationale:**
- **Scalability:** Clear organization as project grows to 38+ entities
- **Navigation:** Easy to locate related entities (all Identity entities in one folder)
- **Mirrors database:** Matches the database schema organization from db-design.md

---

## Repository Implementation Status

The following table maps all database entities to their corresponding domain entities, features, repository interfaces, and implementation status.

### Core Module - Tenancy

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `TENANT` | `Tenant` | Tenancy | `ITenantRepository` | `TenantRepository` → `PostgresTenantRepository` | ✅ Implemented | `GetByIdAsync`, `GetByKeyAsync`, `GetAllAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Top-level multi-tenant isolation boundary | `UNIQUE (name)` WHERE deleted |
| `SITE` | `Site` | Tenancy | `ISiteRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByHostNameAsync`, `GetByTenantAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Multi-site support within tenant (hostnames, locales) | `UNIQUE (tenant_id, host_name)` WHERE deleted |

### Identity Module

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `USER` | `User` | Tenancy | `IUserRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByEmailAsync`, `GetByTenantAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Core identity record (federated auth planned) | `UNIQUE (tenant_id, email)` WHERE deleted |
| `ROLE` | `Role` | Tenancy | `IRoleRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByNameAsync`, `GetByTenantAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Role-based access control (RBAC) | `UNIQUE (tenant_id, name)` WHERE deleted |
| `GROUP` | `Group` | Tenancy | `IGroupRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByNameAsync`, `GetByTenantAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Directory-style user grouping | `UNIQUE (tenant_id, name)` WHERE deleted |
| `USER_ROLE` | `UserRole` | Tenancy | `IUserRoleRepository` | - | ⏳ Not Implemented | `GetByUserAsync`, `GetByRoleAsync`, `AssignRoleAsync`, `RemoveRoleAsync` | Many-to-many user-role mapping | `UNIQUE (tenant_id, user_id, role_id)` WHERE deleted |
| `USER_GROUP` | `UserGroup` | Tenancy | `IUserGroupRepository` | - | ⏳ Not Implemented | `GetByUserAsync`, `GetByGroupAsync`, `AssignGroupAsync`, `RemoveGroupAsync` | Many-to-many user-group mapping | `UNIQUE (tenant_id, user_id, group_id)` WHERE deleted |

### Content Module - Core

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `CONTENT_TYPE` | `ContentType` | Content | `IContentTypeRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByTypeKeyAsync`, `GetByTenantAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Schema registry for content modeling | `UNIQUE (tenant_id, type_key)` WHERE deleted |
| `CONTENT_TYPE_FIELD` | `ContentTypeField` | Content | `IContentTypeFieldRepository` | - | ⏳ Not Implemented | `GetByContentTypeAsync`, `GetByFieldKeyAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Field definitions with validation constraints | `UNIQUE (tenant_id, content_type_id, field_key)` WHERE deleted |
| `CONTENT_ITEM` | `ContentItem` | Content | `IContentItemRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetBySiteAsync`, `GetByTypeAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `ArchiveAsync` | Content instance (stable identity across versions) | Referenced by versions |
| `CONTENT_VERSION` | `ContentVersion` | Content | `IContentVersionRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByItemAsync`, `GetPublishedAsync`, `GetLatestDraftAsync`, `AddAsync`, `PublishAsync`, `ArchiveAsync` | Versioned content lifecycle (draft→review→published) | `UNIQUE (tenant_id, content_item_id, version_number)` |
| `CONTENT_FIELD_VALUE` | `ContentFieldValue` | Content | `IContentFieldValueRepository` | - | ⏳ Not Implemented | `GetByVersionAsync`, `GetByFieldKeyAsync`, `GetLocalizedAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Stores typed field values per version + locale | `UNIQUE (tenant_id, content_version_id, field_key, locale)` WHERE deleted |

### Content Module - Hierarchy

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `CONTENT_NODE` | `ContentNode` | Content | `IContentNodeRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByPathAsync`, `GetChildrenAsync`, `GetTreeAsync`, `AddAsync`, `MoveAsync`, `DeleteAsync` | Content tree navigation (folders, items, links, mounts) | `UNIQUE (tenant_id, site_id, path)` WHERE deleted |
| `ROUTE` | `Route` | Content | `IRouteRepository` | - | ⏳ Not Implemented | `GetByNodeAsync`, `GetByRoutePathAsync`, `GetPrimaryAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Delivery routing (friendly URLs → nodes) | `UNIQUE (tenant_id, site_id, route_path)` WHERE deleted |

### Content Module - Layout

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `LAYOUT_DEFINITION` | `LayoutDefinition` | Content | `ILayoutDefinitionRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByKeyAsync`, `GetByTenantAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Reusable layout templates (regions + rules) | `UNIQUE (tenant_id, layout_key, version)` WHERE deleted |
| `COMPONENT_DEFINITION` | `ComponentDefinition` | Content | `IComponentDefinitionRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByKeyAsync`, `GetByModuleAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Component registry (module-owned components + schemas) | `UNIQUE (tenant_id, component_key, version)` WHERE deleted |
| `CONTENT_LAYOUT` | `ContentLayout` | Content | `IContentLayoutRepository` | - | ⏳ Not Implemented | `GetByVersionAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Composed layout JSON per content version | `UNIQUE (tenant_id, content_version_id)` WHERE deleted |

### Workflow Module

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `WORKFLOW_DEFINITION` | `WorkflowDefinition` | Workflow | `IWorkflowDefinitionRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByKeyAsync`, `GetDefaultAsync`, `GetByTenantAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Workflow graph definition (Draft → Review → Publish) | `UNIQUE (tenant_id, workflow_key)` WHERE deleted |
| `WORKFLOW_STATE` | `WorkflowState` | Workflow | `IWorkflowStateRepository` | - | ⏳ Not Implemented | `GetByDefinitionAsync`, `GetByKeyAsync`, `GetTerminalStatesAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | States within workflow definition | `UNIQUE (tenant_id, workflow_definition_id, state_key)` WHERE deleted |
| `WORKFLOW_TRANSITION` | `WorkflowTransition` | Workflow | `IWorkflowTransitionRepository` | - | ⏳ Not Implemented | `GetByDefinitionAsync`, `GetAllowedTransitionsAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Allowed state transitions with required actions | `UNIQUE (tenant_id, workflow_definition_id, from_state_id, to_state_id)` WHERE deleted |

### Security Module

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `ACL_ENTRY` | `AclEntry` | Security | `IAclRepository` | - | ⏳ Not Implemented | `GetByScopeAsync`, `GetByPrincipalAsync`, `CheckPermissionAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Fine-grained permissions (scope + principal + actions) | Inheritance via `ContentNode.inheritAcl` |
| `PREVIEW_TOKEN` | `PreviewToken` | Security | `IPreviewTokenRepository` | - | ⏳ Not Implemented | `GetByTokenHashAsync`, `CreateAsync`, `MarkUsedAsync`, `CleanupExpiredAsync` | Secure preview links (hashed tokens, time-bound) | `UNIQUE (tenant_id, token_hash)` |
| `AUDIT_LOG` | `AuditLog` | Security | `IAuditLogRepository` | - | ⏳ Not Implemented | `AddAsync`, `GetByEntityAsync`, `GetByActorAsync`, `GetByDateRangeAsync` | Append-only audit trail (immutable) | No updates/deletes allowed |

### Module Management

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `MODULE` | `Module` | Modules | `IModuleRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByKeyAsync`, `GetInstalledAsync`, `InstallAsync`, `UninstallAsync`, `UpdateStatusAsync` | Module registry and lifecycle management | `UNIQUE (tenant_id, module_key)` WHERE deleted |
| `MODULE_CAPABILITY` | `ModuleCapability` | Modules | `IModuleCapabilityRepository` | - | ⏳ Not Implemented | `GetByModuleAsync`, `GetByKeyAsync`, `EnableAsync`, `DisableAsync` | Feature flags and licensing per module | `UNIQUE (tenant_id, module_id, capability_key)` |
| `MODULE_SETTING` | `ModuleSetting` | Modules | `IModuleSettingRepository` | - | ⏳ Not Implemented | `GetByModuleAsync`, `GetByKeyAsync`, `UpdateAsync` | Module-specific configuration (per tenant/site) | `UNIQUE (tenant_id, module_id, site_id, setting_key)` |
| `MODULE_MIGRATION` | `ModuleMigration` | Modules | `IModuleMigrationRepository` | - | ⏳ Not Implemented | `GetByModuleAsync`, `RecordMigrationAsync`, `GetPendingAsync` | Tracks module schema migrations | `UNIQUE (tenant_id, module_id, migration_name)` |

### Entity System

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `ENTITY_DEFINITION` | `EntityDefinition` | Entities | `IEntityDefinitionRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByKeyAsync`, `GetByModuleAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Schema registry for domain entities (Tickets, Patients, Orders) | `UNIQUE (tenant_id, module_id, entity_key)` WHERE deleted |
| `ENTITY_INSTANCE` | `EntityInstance` | Entities | `IEntityInstanceRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByDefinitionAsync`, `GetByKeyAsync`, `QueryAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | Instances of domain entities (flexible JSONB storage) | Indexes on `data_json` for queries |
| `ENTITY_RELATIONSHIP` | `EntityRelationship` | Entities | `IEntityRelationshipRepository` | - | ⏳ Not Implemented | `GetBySourceAsync`, `GetByTargetAsync`, `AddAsync`, `RemoveAsync` | Cross-module entity relationships | Indexes on source/target |

### Business Process

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `PROCESS_DEFINITION` | `ProcessDefinition` | Process | `IProcessDefinitionRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByKeyAsync`, `GetByModuleAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` | State machines and workflows (order fulfillment, ticket resolution) | `UNIQUE (tenant_id, module_id, process_key)` WHERE deleted |
| `PROCESS_INSTANCE` | `ProcessInstance` | Process | `IProcessInstanceRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByDefinitionAsync`, `GetOverdueAsync`, `StartAsync`, `UpdateStateAsync`, `CompleteAsync` | Running process instances with SLA tracking | Indexes on `is_overdue`, `status` |

### Collaboration

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `ATTACHMENT` | `Attachment` | Media | `IAttachmentRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByEntityAsync`, `AddAsync`, `DeleteAsync`, `GetUnscannedAsync` | Universal file attachment support | Indexes on `entity_instance_id`, `scan_status` |
| `COMMENT` | `Comment` | Collaboration | `ICommentRepository` | - | ⏳ Not Implemented | `GetByEntityAsync`, `GetByUserAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `ResolveAsync` | Generic commenting system (nested comments) | Indexes on `entity_instance_id`, `created_on` |
| `ACTIVITY_LOG` | `ActivityLog` | Collaboration | `IActivityLogRepository` | - | ⏳ Not Implemented | `AddAsync`, `GetByEntityAsync`, `GetByActorAsync`, `GetByDateRangeAsync` | Universal activity/change tracking | Partitioned by month |

### Search & Discovery

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `SEARCH_INDEX_ENTRY` | `SearchIndexEntry` | Search | `ISearchIndexRepository` | - | ⏳ Not Implemented | `IndexAsync`, `SearchAsync`, `GetByEntityAsync`, `DeleteAsync`, `RebuildIndexAsync` | Unified full-text search across modules | GIN index on `searchable_text` (tsvector) |

### AI & Vector Search

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `CONTENT_RAG_CHUNKS` | `ContentRagChunk` | AI | `IContentRagChunkRepository` | - | ⏳ Not Implemented | `GetBySourceAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `GetByLocaleAsync` | Text chunking for RAG (independent of embeddings) | `UNIQUE (tenant_id, source_type, source_id, source_version_id, chunk_index)` WHERE deleted |
| `CONTENT_EMBEDDING` | `ContentEmbedding` | AI | `IContentEmbeddingRepository` | - | ⏳ Not Implemented | `GetByChunkAsync`, `SearchSimilarAsync`, `AddAsync`, `DeleteAsync`, `RebuildAsync` | Vector embeddings for semantic search (pgvector) | HNSW/IVFFlat vector index, `UNIQUE (chunk_id, embedding_model)` |
| `IMAGE_EMBEDDING` | `ImageEmbedding` | AI | `IImageEmbeddingRepository` | - | ⏳ Not Implemented | `GetByAttachmentAsync`, `SearchSimilarAsync`, `AddAsync`, `DeleteAsync` | Visual similarity search (CLIP, DINOv2) | HNSW vector index, `UNIQUE (attachment_id, embedding_model)` |

### Background Jobs

| DB Entity | Domain Entity | Feature | Repository Interface | Repository Implementation | Status | Methods | Description | Constraints |
|-----------|--------------|---------|---------------------|--------------------------|--------|---------|-------------|-------------|
| `JOB_DEFINITION` | `JobDefinition` | Jobs | `IJobDefinitionRepository` | - | ⏳ Not Implemented | `GetByIdAsync`, `GetByKeyAsync`, `GetScheduledAsync`, `AddAsync`, `UpdateAsync`, `EnableAsync`, `DisableAsync` | Job metadata and scheduling configuration | `UNIQUE (tenant_id, job_key)` WHERE deleted |
| `JOB_EXECUTION` | `JobExecution` | Jobs | `IJobExecutionRepository` | - | ⏳ Not Implemented | `GetPendingAsync`, `ClaimAsync`, `UpdateHeartbeatAsync`, `CompleteAsync`, `FailAsync`, `FindOrphansAsync` | Individual job execution with distributed locking | Uses `SELECT ... FOR UPDATE SKIP LOCKED` |
| `JOB_EXECUTION_HISTORY` | `JobExecutionHistory` | Jobs | `IJobExecutionHistoryRepository` | - | ⏳ Not Implemented | `AddAsync`, `GetByJobAsync`, `GetByDateRangeAsync` | Long-term audit trail (partitioned by month) | Partitioned by `archived_at` |

---

## Summary Statistics

| Status | Count | Percentage |
|--------|-------|------------|
| ✅ Implemented | 1 | 2.6% |
| ⏳ Not Implemented | 37 | 97.4% |
| **Total Entities** | **38** | **100%** |

---

## Implementation Guidelines

### Creating a New Repository

1. **Define Domain Entity (Feature Project)**
   ```csharp
   // TechWayFit.ContentOS.Content/Domain/Core/ContentType.cs
   public class ContentType
   {
       public Guid Id { get; set; }
       public Guid TenantId { get; set; }
       public string TypeKey { get; set; } = string.Empty;
       public string DisplayName { get; set; } = string.Empty;
       // ... other properties
   }
   ```

2. **Define Repository Interface (Feature Project)**
   ```csharp
   // TechWayFit.ContentOS.Content/Ports/Core/IContentTypeRepository.cs
   public interface IContentTypeRepository : IRepository<ContentType, Guid>
   {
       Task<ContentType?> GetByTypeKeyAsync(Guid tenantId, string typeKey);
       Task<IEnumerable<ContentType>> GetByTenantAsync(Guid tenantId);
   }
   ```

3. **Implement Repository (Persistence Project)**
   ```csharp
   // TechWayFit.ContentOS.Infrastructure.Persistence/Repositories/Content/ContentTypeRepository.cs
   public class ContentTypeRepository : EfCoreRepository<ContentType, ContentTypeRow, Guid>, IContentTypeRepository
   {
       public ContentTypeRepository(DbContext dbContext) : base(dbContext) { }

       protected override ContentType MapToDomain(ContentTypeRow row)
       {
           return new ContentType
           {
               Id = row.Id,
               TenantId = row.TenantId,
               TypeKey = row.TypeKey,
               DisplayName = row.DisplayName,
               // ... all properties
           };
       }

       protected override ContentTypeRow MapToRow(ContentType entity)
       {
           return new ContentTypeRow
           {
               Id = entity.Id,
               TenantId = entity.TenantId,
               TypeKey = entity.TypeKey,
               DisplayName = entity.DisplayName,
               // ... all properties
           };
       }

       protected override Expression<Func<ContentTypeRow, bool>> CreateRowPredicate(Guid id)
       {
           return row => row.Id == id;
       }

       public async Task<ContentType?> GetByTypeKeyAsync(Guid tenantId, string typeKey)
       {
           var row = await DbSet
               .FirstOrDefaultAsync(r => r.TenantId == tenantId && r.TypeKey == typeKey);
           return row != null ? MapToDomain(row) : null;
       }
   }
   ```

4. **Add Postgres-Specific Overrides (If Needed)**
   ```csharp
   // TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/Repositories/PostgresContentTypeRepository.cs
   public class PostgresContentTypeRepository : ContentTypeRepository
   {
       public PostgresContentTypeRepository(ContentOsDbContext dbContext) : base(dbContext) { }

       // Override with Postgres-specific features (ILike, full-text search, etc.)
       public override async Task<ContentType?> GetByTypeKeyAsync(Guid tenantId, string typeKey)
       {
           var row = await DbSet
               .FirstOrDefaultAsync(r => r.TenantId == tenantId 
                   && EF.Functions.ILike(r.TypeKey, typeKey)); // Case-insensitive
           return row != null ? MapToDomain(row) : null;
       }
   }
   ```

5. **Register in DI Container**
   ```csharp
   // TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/DependencyInjection.cs
   services.AddScoped<IContentTypeRepository, PostgresContentTypeRepository>();
   ```

---

## Testing Strategy

### Unit Tests
- **Location:** `tests/TechWayFit.ContentOS.{Feature}.Tests/`
- **Scope:** Domain entities, use-cases (mock repositories)
- **Tools:** xUnit, Moq

### Integration Tests
- **Location:** `tests/TechWayFit.ContentOS.Api.Tests/`
- **Scope:** Repository implementations against real database
- **Tools:** xUnit, Testcontainers (Postgres Docker), WebApplicationFactory

### Repository Integration Test Example
```csharp
public class ContentTypeRepositoryTests : IAsyncLifetime
{
    private PostgreSqlContainer _postgres;
    private ContentOsDbContext _dbContext;
    private IContentTypeRepository _repository;

    public async Task InitializeAsync()
    {
        _postgres = new PostgreSqlBuilder().Build();
        await _postgres.StartAsync();
        
        var options = new DbContextOptionsBuilder<ContentOsDbContext>()
            .UseNpgsql(_postgres.GetConnectionString())
            .Options;
        
        _dbContext = new ContentOsDbContext(options);
        await _dbContext.Database.MigrateAsync();
        
        _repository = new PostgresContentTypeRepository(_dbContext);
    }

    [Fact]
    public async Task GetByTypeKeyAsync_ReturnsContentType()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var contentType = new ContentType
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            TypeKey = "page.article",
            DisplayName = "Article"
        };
        await _repository.AddAsync(contentType);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetByTypeKeyAsync(tenantId, "page.article");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Article", result.DisplayName);
    }
}
```

---

## Future Enhancements

### Planned Improvements

1. **Specification Pattern**
   - Introduce reusable query specifications for complex filtering
   - Example: `new ActiveContentItemsSpec(tenantId, siteId)`

2. **CQRS Optimization**
   - Separate read models for delivery APIs (denormalized views)
   - Write models remain as-is (normalized domain entities)

3. **Caching Strategy**
   - Add distributed caching layer (Redis) for read-heavy entities
   - Cache invalidation via domain events

4. **Polyglot Persistence**
   - Content: PostgreSQL (current)
   - Media metadata: PostgreSQL, blobs: S3/Azure Blob Storage
   - Search: Elasticsearch or Azure AI Search
   - Vector embeddings: Postgres pgvector (current) or Pinecone (future)

5. **Event Sourcing (Selective)**
   - Event-sourced aggregates for audit-heavy scenarios (workflow transitions, permissions)
   - Hybrid approach: Event sourcing for writes, materialized views for reads

---

## References

- [Architecture Overview](./architecture.md)
- [Database Design](./db-design.md)
- [Module Boundaries](./module-boundaries.md)
- [ADR-015: Kernel-Contracts-Abstractions](./adr/adr-015-Kernel-Contracts-Abstractions.md)
- [Domain Entity vs Content Entity](./domain-entity-vs-content-entity.md)
