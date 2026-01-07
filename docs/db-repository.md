# ContentOS Repository & Mapping Architecture

## Overview

This document provides a comprehensive guide to the repository pattern, mapping strategy, and data access layer for ContentOS Core. It lists all repositories, their methods, and mapping classes across the system.

---

## Architecture Layers

### 1. Domain Entities
- **Location**: `src/features/{feature}/Domain/`
- **Purpose**: Business logic and domain models
- **Rules**: 
  - No EF Core dependencies
  - No persistence annotations
  - Rich domain behavior
  - Private setters with factory methods

### 2. DB Row Entities
- **Location**: `src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/Entities/`
- **Purpose**: Database table representations
- **Rules**:
  - Must end with `Row` suffix
  - EF Core configurations via Fluent API
  - Optimized for storage
  - Public getters/setters

### 3. Mappers
- **Location**: `src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/Mappers/`
- **Purpose**: Bidirectional mapping between Domain ↔ DB Row
- **Pattern**: Static mapper classes with `ToDomain()` and `ToRow()` methods

### 4. Repository Interfaces
- **Location**: `src/features/{feature}/Ports/`
- **Purpose**: Define data access contracts
- **Pattern**: Inherit from `IRepository<TEntity, TKey>` for common CRUD operations

### 5. Repository Implementations
- **Common Logic**: `src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence/Repositories/BaseRepository.cs`
- **Provider-Specific**: `src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/Repositories/`
- **Pattern**: Inherit from `BaseRepository<TEntity, TRow, TKey>` and implement provider-specific logic

---

## Base Repository Interface

### IRepository<TEntity, TKey>
**Location**: `TechWayFit.ContentOS.Abstractions/Repositories/IRepository.cs`

#### CRUD Operations
```csharp
Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
Task<IReadOnlyList<TEntity>> GetByIdsAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
```

#### Filtering & Querying
```csharp
Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
Task<IReadOnlyList<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
Task<IReadOnlyList<TEntity>> FindWithSpecificationAsync(FilterSpecification<TEntity> specification, CancellationToken cancellationToken = default);
Task<PagedResult<TEntity>> FindPagedAsync(Expression<Func<TEntity, bool>>? predicate, PaginationParameters pagination, Expression<Func<TEntity, object>>? orderBy = null, bool descending = false, CancellationToken cancellationToken = default);
```

#### Count & Existence
```csharp
Task<int> CountAsync(CancellationToken cancellationToken = default);
Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
Task<bool> AnyAsync(CancellationToken cancellationToken = default);
Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);
```

#### Search
```csharp
Task<IReadOnlyList<TEntity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
Task<PagedResult<TEntity>> SearchPagedAsync(string searchTerm, PaginationParameters pagination, CancellationToken cancellationToken = default);
```

---

## Entity & Repository Catalog

### 1. Tenancy Module

#### 1.1 Tenant
- **Domain Entity**: `TechWayFit.ContentOS.Tenancy.Domain.Tenant`
- **DB Row**: `TenantRow`
- **Mapper**: `TenantMapper`
- **Repository Interface**: `ITenantRepository`
- **Repository Implementation**: `TenantRepository`

**Specific Methods**:
```csharp
// ITenantRepository (inherits from IRepository<Tenant, Guid>)
Task<Tenant?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
Task<bool> KeyExistsAsync(string key, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Tenant>> ListByStatusAsync(TenantStatus status, CancellationToken cancellationToken = default);
```

**Mapper Methods**:
```csharp
// TenantMapper
public static TenantRow ToRow(Tenant domain);
public static Tenant ToDomain(TenantRow row);
```

---

#### 1.2 Site
- **Domain Entity**: `TechWayFit.ContentOS.Tenancy.Domain.Site`
- **DB Row**: `SiteRow`
- **Mapper**: `SiteMapper`
- **Repository Interface**: `ISiteRepository`
- **Repository Implementation**: `SiteRepository`

**Specific Methods**:
```csharp
// ISiteRepository (inherits from IRepository<Site, Guid>)
Task<IReadOnlyList<Site>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<Site?> GetByHostNameAsync(Guid tenantId, string hostName, CancellationToken cancellationToken = default);
Task<bool> HostNameExistsAsync(Guid tenantId, string hostName, CancellationToken cancellationToken = default);
```

---

### 2. Identity Module

#### 2.1 User
- **Domain Entity**: `TechWayFit.ContentOS.Identity.Domain.User`
- **DB Row**: `UserRow`
- **Mapper**: `UserMapper`
- **Repository Interface**: `IUserRepository`
- **Repository Implementation**: `UserRepository`

**Specific Methods**:
```csharp
// IUserRepository (inherits from IRepository<User, Guid>)
Task<User?> GetByEmailAsync(Guid tenantId, string email, CancellationToken cancellationToken = default);
Task<User?> GetByUsernameAsync(Guid tenantId, string username, CancellationToken cancellationToken = default);
Task<bool> EmailExistsAsync(Guid tenantId, string email, CancellationToken cancellationToken = default);
Task<IReadOnlyList<User>> GetByRoleAsync(Guid tenantId, Guid roleId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<User>> GetByGroupAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken = default);
Task<PagedResult<User>> SearchByNameOrEmailAsync(Guid tenantId, string searchTerm, PaginationParameters pagination, CancellationToken cancellationToken = default);
```

---

#### 2.2 Role
- **Domain Entity**: `TechWayFit.ContentOS.Identity.Domain.Role`
- **DB Row**: `RoleRow`
- **Mapper**: `RoleMapper`
- **Repository Interface**: `IRoleRepository`
- **Repository Implementation**: `RoleRepository`

**Specific Methods**:
```csharp
// IRoleRepository (inherits from IRepository<Role, Guid>)
Task<Role?> GetByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Role>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Role>> GetSystemRolesAsync(CancellationToken cancellationToken = default);
Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);
```

---

#### 2.3 Group
- **Domain Entity**: `TechWayFit.ContentOS.Identity.Domain.Group`
- **DB Row**: `GroupRow`
- **Mapper**: `GroupMapper`
- **Repository Interface**: `IGroupRepository`
- **Repository Implementation**: `GroupRepository`

**Specific Methods**:
```csharp
// IGroupRepository (inherits from IRepository<Group, Guid>)
Task<Group?> GetByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Group>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Group>> GetByUserIdAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);
```

---

#### 2.4 UserRole
- **Domain Entity**: `TechWayFit.ContentOS.Identity.Domain.UserRole`
- **DB Row**: `UserRoleRow`
- **Mapper**: `UserRoleMapper`
- **Repository Interface**: `IUserRoleRepository`
- **Repository Implementation**: `UserRoleRepository`

**Specific Methods**:
```csharp
// IUserRoleRepository (inherits from IRepository<UserRole, Guid>)
Task<IReadOnlyList<UserRole>> GetByUserIdAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<UserRole>> GetByRoleIdAsync(Guid tenantId, Guid roleId, CancellationToken cancellationToken = default);
Task<bool> ExistsAsync(Guid tenantId, Guid userId, Guid roleId, CancellationToken cancellationToken = default);
Task DeleteByUserAndRoleAsync(Guid tenantId, Guid userId, Guid roleId, CancellationToken cancellationToken = default);
```

---

#### 2.5 UserGroup
- **Domain Entity**: `TechWayFit.ContentOS.Identity.Domain.UserGroup`
- **DB Row**: `UserGroupRow`
- **Mapper**: `UserGroupMapper`
- **Repository Interface**: `IUserGroupRepository`
- **Repository Implementation**: `UserGroupRepository`

**Specific Methods**:
```csharp
// IUserGroupRepository (inherits from IRepository<UserGroup, Guid>)
Task<IReadOnlyList<UserGroup>> GetByUserIdAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<UserGroup>> GetByGroupIdAsync(Guid tenantId, Guid groupId, CancellationToken cancellationToken = default);
Task<bool> ExistsAsync(Guid tenantId, Guid userId, Guid groupId, CancellationToken cancellationToken = default);
Task DeleteByUserAndGroupAsync(Guid tenantId, Guid userId, Guid groupId, CancellationToken cancellationToken = default);
```

---

### 3. Content Module

#### 3.1 ContentNode
- **Domain Entity**: `TechWayFit.ContentOS.Content.Domain.ContentNode`
- **DB Row**: `ContentNodeRow`
- **Mapper**: `ContentNodeMapper`
- **Repository Interface**: `IContentNodeRepository`
- **Repository Implementation**: `ContentNodeRepository`

**Specific Methods**:
```csharp
// IContentNodeRepository (inherits from IRepository<ContentNode, Guid>)
Task<ContentNode?> GetByPathAsync(Guid tenantId, Guid siteId, string path, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentNode>> GetChildrenAsync(Guid tenantId, Guid parentId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentNode>> GetByContentItemIdAsync(Guid tenantId, Guid contentItemId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentNode>> GetBySiteIdAsync(Guid tenantId, Guid siteId, CancellationToken cancellationToken = default);
Task<ContentNode?> GetRootNodeAsync(Guid tenantId, Guid siteId, CancellationToken cancellationToken = default);
Task<bool> PathExistsAsync(Guid tenantId, Guid siteId, string path, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentNode>> GetDescendantsAsync(Guid tenantId, Guid nodeId, CancellationToken cancellationToken = default);
```

---

#### 3.2 Route
- **Domain Entity**: `TechWayFit.ContentOS.Content.Domain.Route`
- **DB Row**: `RouteRow`
- **Mapper**: `RouteMapper`
- **Repository Interface**: `IRouteRepository`
- **Repository Implementation**: `RouteRepository`

**Specific Methods**:
```csharp
// IRouteRepository (inherits from IRepository<Route, Guid>)
Task<Route?> GetByRoutePathAsync(Guid tenantId, Guid siteId, string routePath, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Route>> GetByNodeIdAsync(Guid tenantId, Guid nodeId, CancellationToken cancellationToken = default);
Task<Route?> GetPrimaryRouteAsync(Guid tenantId, Guid nodeId, CancellationToken cancellationToken = default);
Task<bool> RoutePathExistsAsync(Guid tenantId, Guid siteId, string routePath, CancellationToken cancellationToken = default);
```

---

#### 3.3 ContentType
- **Domain Entity**: `TechWayFit.ContentOS.Content.Domain.ContentType`
- **DB Row**: `ContentTypeRow`
- **Mapper**: `ContentTypeMapper`
- **Repository Interface**: `IContentTypeRepository`
- **Repository Implementation**: `ContentTypeRepository`

**Specific Methods**:
```csharp
// IContentTypeRepository (inherits from IRepository<ContentType, Guid>)
Task<ContentType?> GetByTypeKeyAsync(Guid tenantId, string typeKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentType>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<bool> TypeKeyExistsAsync(Guid tenantId, string typeKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentType>> GetSystemTypesAsync(CancellationToken cancellationToken = default);
```

---

#### 3.4 ContentTypeField
- **Domain Entity**: `TechWayFit.ContentOS.Content.Domain.ContentTypeField`
- **DB Row**: `ContentTypeFieldRow`
- **Mapper**: `ContentTypeFieldMapper`
- **Repository Interface**: `IContentTypeFieldRepository`
- **Repository Implementation**: `ContentTypeFieldRepository`

**Specific Methods**:
```csharp
// IContentTypeFieldRepository (inherits from IRepository<ContentTypeField, Guid>)
Task<IReadOnlyList<ContentTypeField>> GetByContentTypeIdAsync(Guid tenantId, Guid contentTypeId, CancellationToken cancellationToken = default);
Task<ContentTypeField?> GetByFieldKeyAsync(Guid tenantId, Guid contentTypeId, string fieldKey, CancellationToken cancellationToken = default);
Task<bool> FieldKeyExistsAsync(Guid tenantId, Guid contentTypeId, string fieldKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentTypeField>> GetLocalizedFieldsAsync(Guid tenantId, Guid contentTypeId, CancellationToken cancellationToken = default);
```

---

#### 3.5 ContentItem
- **Domain Entity**: `TechWayFit.ContentOS.Content.Domain.ContentItem`
- **DB Row**: `ContentItemRow`
- **Mapper**: `ContentItemMapper`
- **Repository Interface**: `IContentItemRepository`
- **Repository Implementation**: `ContentItemRepository`

**Specific Methods**:
```csharp
// IContentItemRepository (inherits from IRepository<ContentItem, Guid>)
Task<IReadOnlyList<ContentItem>> GetByContentTypeIdAsync(Guid tenantId, Guid contentTypeId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentItem>> GetBySiteIdAsync(Guid tenantId, Guid siteId, CancellationToken cancellationToken = default);
Task<PagedResult<ContentItem>> GetByContentTypePagedAsync(Guid tenantId, Guid contentTypeId, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentItem>> GetByStatusAsync(Guid tenantId, ContentItemStatus status, CancellationToken cancellationToken = default);
```

---

#### 3.6 ContentVersion
- **Domain Entity**: `TechWayFit.ContentOS.Content.Domain.ContentVersion`
- **DB Row**: `ContentVersionRow`
- **Mapper**: `ContentVersionMapper`
- **Repository Interface**: `IContentVersionRepository`
- **Repository Implementation**: `ContentVersionRepository`

**Specific Methods**:
```csharp
// IContentVersionRepository (inherits from IRepository<ContentVersion, Guid>)
Task<IReadOnlyList<ContentVersion>> GetByContentItemIdAsync(Guid tenantId, Guid contentItemId, CancellationToken cancellationToken = default);
Task<ContentVersion?> GetLatestVersionAsync(Guid tenantId, Guid contentItemId, CancellationToken cancellationToken = default);
Task<ContentVersion?> GetPublishedVersionAsync(Guid tenantId, Guid contentItemId, CancellationToken cancellationToken = default);
Task<ContentVersion?> GetByVersionNumberAsync(Guid tenantId, Guid contentItemId, int versionNumber, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentVersion>> GetByLifecycleAsync(Guid tenantId, Guid contentItemId, VersionLifecycle lifecycle, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentVersion>> GetByWorkflowStateAsync(Guid tenantId, Guid workflowStateId, CancellationToken cancellationToken = default);
```

---

#### 3.7 ContentFieldValue
- **Domain Entity**: `TechWayFit.ContentOS.Content.Domain.ContentFieldValue`
- **DB Row**: `ContentFieldValueRow`
- **Mapper**: `ContentFieldValueMapper`
- **Repository Interface**: `IContentFieldValueRepository`
- **Repository Implementation**: `ContentFieldValueRepository`

**Specific Methods**:
```csharp
// IContentFieldValueRepository (inherits from IRepository<ContentFieldValue, Guid>)
Task<IReadOnlyList<ContentFieldValue>> GetByVersionIdAsync(Guid tenantId, Guid contentVersionId, CancellationToken cancellationToken = default);
Task<ContentFieldValue?> GetByFieldKeyAsync(Guid tenantId, Guid contentVersionId, string fieldKey, string locale, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentFieldValue>> GetByLocaleAsync(Guid tenantId, Guid contentVersionId, string locale, CancellationToken cancellationToken = default);
Task DeleteByVersionIdAsync(Guid tenantId, Guid contentVersionId, CancellationToken cancellationToken = default);
```

---

#### 3.8 LayoutDefinition
- **Domain Entity**: `TechWayFit.ContentOS.Content.Domain.LayoutDefinition`
- **DB Row**: `LayoutDefinitionRow`
- **Mapper**: `LayoutDefinitionMapper`
- **Repository Interface**: `ILayoutDefinitionRepository`
- **Repository Implementation**: `LayoutDefinitionRepository`

**Specific Methods**:
```csharp
// ILayoutDefinitionRepository (inherits from IRepository<LayoutDefinition, Guid>)
Task<LayoutDefinition?> GetByLayoutKeyAsync(Guid tenantId, string layoutKey, int version, CancellationToken cancellationToken = default);
Task<IReadOnlyList<LayoutDefinition>> GetByLayoutKeyAllVersionsAsync(Guid tenantId, string layoutKey, CancellationToken cancellationToken = default);
Task<LayoutDefinition?> GetLatestVersionAsync(Guid tenantId, string layoutKey, CancellationToken cancellationToken = default);
```

---

#### 3.9 ComponentDefinition
- **Domain Entity**: `TechWayFit.ContentOS.Content.Domain.ComponentDefinition`
- **DB Row**: `ComponentDefinitionRow`
- **Mapper**: `ComponentDefinitionMapper`
- **Repository Interface**: `IComponentDefinitionRepository`
- **Repository Implementation**: `ComponentDefinitionRepository`

**Specific Methods**:
```csharp
// IComponentDefinitionRepository (inherits from IRepository<ComponentDefinition, Guid>)
Task<ComponentDefinition?> GetByComponentKeyAsync(Guid tenantId, string componentKey, int version, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ComponentDefinition>> GetByComponentKeyAllVersionsAsync(Guid tenantId, string componentKey, CancellationToken cancellationToken = default);
Task<ComponentDefinition?> GetLatestVersionAsync(Guid tenantId, string componentKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ComponentDefinition>> GetByOwnerModuleAsync(Guid tenantId, string ownerModule, CancellationToken cancellationToken = default);
```

---

#### 3.10 ContentLayout
- **Domain Entity**: `TechWayFit.ContentOS.Content.Domain.ContentLayout`
- **DB Row**: `ContentLayoutRow`
- **Mapper**: `ContentLayoutMapper`
- **Repository Interface**: `IContentLayoutRepository`
- **Repository Implementation**: `ContentLayoutRepository`

**Specific Methods**:
```csharp
// IContentLayoutRepository (inherits from IRepository<ContentLayout, Guid>)
Task<ContentLayout?> GetByContentVersionIdAsync(Guid tenantId, Guid contentVersionId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentLayout>> GetByLayoutDefinitionIdAsync(Guid tenantId, Guid layoutDefinitionId, CancellationToken cancellationToken = default);
Task DeleteByContentVersionIdAsync(Guid tenantId, Guid contentVersionId, CancellationToken cancellationToken = default);
```

---

### 4. Security Module

#### 4.1 AclEntry
- **Domain Entity**: `TechWayFit.ContentOS.Security.Domain.AclEntry`
- **DB Row**: `AclEntryRow`
- **Mapper**: `AclEntryMapper`
- **Repository Interface**: `IAclEntryRepository`
- **Repository Implementation**: `AclEntryRepository`

**Specific Methods**:
```csharp
// IAclEntryRepository (inherits from IRepository<AclEntry, Guid>)
Task<IReadOnlyList<AclEntry>> GetByScopeAsync(Guid tenantId, string scopeType, Guid scopeId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<AclEntry>> GetByPrincipalAsync(Guid tenantId, string principalType, Guid principalId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<AclEntry>> GetEffectiveEntriesAsync(Guid tenantId, Guid userId, string scopeType, Guid scopeId, CancellationToken cancellationToken = default);
Task<bool> HasPermissionAsync(Guid tenantId, Guid userId, string action, string scopeType, Guid scopeId, CancellationToken cancellationToken = default);
```

---

#### 4.2 PreviewToken
- **Domain Entity**: `TechWayFit.ContentOS.Security.Domain.PreviewToken`
- **DB Row**: `PreviewTokenRow`
- **Mapper**: `PreviewTokenMapper`
- **Repository Interface**: `IPreviewTokenRepository`
- **Repository Implementation**: `PreviewTokenRepository`

**Specific Methods**:
```csharp
// IPreviewTokenRepository (inherits from IRepository<PreviewToken, Guid>)
Task<PreviewToken?> GetByTokenHashAsync(Guid tenantId, string tokenHash, CancellationToken cancellationToken = default);
Task<IReadOnlyList<PreviewToken>> GetByContentVersionIdAsync(Guid tenantId, Guid contentVersionId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<PreviewToken>> GetExpiredTokensAsync(CancellationToken cancellationToken = default);
Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default);
Task MarkAsUsedAsync(Guid tenantId, Guid tokenId, CancellationToken cancellationToken = default);
```

---

### 5. Workflow Module

#### 5.1 WorkflowDefinition
- **Domain Entity**: `TechWayFit.ContentOS.Workflow.Domain.WorkflowDefinition`
- **DB Row**: `WorkflowDefinitionRow`
- **Mapper**: `WorkflowDefinitionMapper`
- **Repository Interface**: `IWorkflowDefinitionRepository`
- **Repository Implementation**: `WorkflowDefinitionRepository`

**Specific Methods**:
```csharp
// IWorkflowDefinitionRepository (inherits from IRepository<WorkflowDefinition, Guid>)
Task<WorkflowDefinition?> GetByWorkflowKeyAsync(Guid tenantId, string workflowKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<WorkflowDefinition>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<WorkflowDefinition?> GetDefaultWorkflowAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<WorkflowDefinition>> GetSystemWorkflowsAsync(CancellationToken cancellationToken = default);
```

---

#### 5.2 WorkflowState
- **Domain Entity**: `TechWayFit.ContentOS.Workflow.Domain.WorkflowState`
- **DB Row**: `WorkflowStateRow`
- **Mapper**: `WorkflowStateMapper`
- **Repository Interface**: `IWorkflowStateRepository`
- **Repository Implementation**: `WorkflowStateRepository`

**Specific Methods**:
```csharp
// IWorkflowStateRepository (inherits from IRepository<WorkflowState, Guid>)
Task<IReadOnlyList<WorkflowState>> GetByWorkflowDefinitionIdAsync(Guid tenantId, Guid workflowDefinitionId, CancellationToken cancellationToken = default);
Task<WorkflowState?> GetByStateKeyAsync(Guid tenantId, Guid workflowDefinitionId, string stateKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<WorkflowState>> GetTerminalStatesAsync(Guid tenantId, Guid workflowDefinitionId, CancellationToken cancellationToken = default);
```

---

#### 5.3 WorkflowTransition
- **Domain Entity**: `TechWayFit.ContentOS.Workflow.Domain.WorkflowTransition`
- **DB Row**: `WorkflowTransitionRow`
- **Mapper**: `WorkflowTransitionMapper`
- **Repository Interface**: `IWorkflowTransitionRepository`
- **Repository Implementation**: `WorkflowTransitionRepository`

**Specific Methods**:
```csharp
// IWorkflowTransitionRepository (inherits from IRepository<WorkflowTransition, Guid>)
Task<IReadOnlyList<WorkflowTransition>> GetByWorkflowDefinitionIdAsync(Guid tenantId, Guid workflowDefinitionId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<WorkflowTransition>> GetByFromStateIdAsync(Guid tenantId, Guid fromStateId, CancellationToken cancellationToken = default);
Task<WorkflowTransition?> GetTransitionAsync(Guid tenantId, Guid fromStateId, Guid toStateId, CancellationToken cancellationToken = default);
Task<bool> TransitionExistsAsync(Guid tenantId, Guid fromStateId, Guid toStateId, CancellationToken cancellationToken = default);
```

---

### 6. Audit Module

#### 6.1 AuditLog
- **Domain Entity**: `TechWayFit.ContentOS.Audit.Domain.AuditLog`
- **DB Row**: `AuditLogRow`
- **Mapper**: `AuditLogMapper`
- **Repository Interface**: `IAuditLogRepository`
- **Repository Implementation**: `AuditLogRepository`

**Note**: Audit logs are append-only (no updates or deletes)

**Specific Methods**:
```csharp
// IAuditLogRepository (inherits from IRepository<AuditLog, Guid>)
Task<IReadOnlyList<AuditLog>> GetByEntityAsync(Guid tenantId, string entityType, Guid entityId, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<IReadOnlyList<AuditLog>> GetByActorAsync(Guid tenantId, Guid actorUserId, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<IReadOnlyList<AuditLog>> GetByActionKeyAsync(Guid tenantId, string actionKey, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<PagedResult<AuditLog>> GetByDateRangeAsync(Guid tenantId, DateTimeOffset startDate, DateTimeOffset endDate, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<int> GetCountByActionAsync(Guid tenantId, string actionKey, DateTimeOffset? since = null, CancellationToken cancellationToken = default);
```

---

### 7. Module Management

#### 7.1 Module
- **Domain Entity**: `TechWayFit.ContentOS.Modules.Domain.Module`
- **DB Row**: `ModuleRow`
- **Mapper**: `ModuleMapper`
- **Repository Interface**: `IModuleRepository`
- **Repository Implementation**: `ModuleRepository`

**Specific Methods**:
```csharp
// IModuleRepository (inherits from IRepository<Module, Guid>)
Task<Module?> GetByModuleKeyAsync(Guid tenantId, string moduleKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Module>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Module>> GetByStatusAsync(Guid tenantId, ModuleInstallationStatus status, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Module>> GetSystemModulesAsync(CancellationToken cancellationToken = default);
Task<bool> ModuleKeyExistsAsync(Guid tenantId, string moduleKey, CancellationToken cancellationToken = default);
```

---

#### 7.2 ModuleCapability
- **Domain Entity**: `TechWayFit.ContentOS.Modules.Domain.ModuleCapability`
- **DB Row**: `ModuleCapabilityRow`
- **Mapper**: `ModuleCapabilityMapper`
- **Repository Interface**: `IModuleCapabilityRepository`
- **Repository Implementation**: `ModuleCapabilityRepository`

**Specific Methods**:
```csharp
// IModuleCapabilityRepository (inherits from IRepository<ModuleCapability, Guid>)
Task<IReadOnlyList<ModuleCapability>> GetByModuleIdAsync(Guid tenantId, Guid moduleId, CancellationToken cancellationToken = default);
Task<ModuleCapability?> GetByCapabilityKeyAsync(Guid tenantId, Guid moduleId, string capabilityKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ModuleCapability>> GetEnabledCapabilitiesAsync(Guid tenantId, Guid moduleId, CancellationToken cancellationToken = default);
```

---

#### 7.3 ModuleSetting
- **Domain Entity**: `TechWayFit.ContentOS.Modules.Domain.ModuleSetting`
- **DB Row**: `ModuleSettingRow`
- **Mapper**: `ModuleSettingMapper`
- **Repository Interface**: `IModuleSettingRepository`
- **Repository Implementation**: `ModuleSettingRepository`

**Specific Methods**:
```csharp
// IModuleSettingRepository (inherits from IRepository<ModuleSetting, Guid>)
Task<IReadOnlyList<ModuleSetting>> GetByModuleIdAsync(Guid tenantId, Guid moduleId, Guid? siteId = null, CancellationToken cancellationToken = default);
Task<ModuleSetting?> GetBySettingKeyAsync(Guid tenantId, Guid moduleId, string settingKey, Guid? siteId = null, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ModuleSetting>> GetEncryptedSettingsAsync(Guid tenantId, Guid moduleId, CancellationToken cancellationToken = default);
```

---

#### 7.4 ModuleMigration
- **Domain Entity**: `TechWayFit.ContentOS.Modules.Domain.ModuleMigration`
- **DB Row**: `ModuleMigrationRow`
- **Mapper**: `ModuleMigrationMapper`
- **Repository Interface**: `IModuleMigrationRepository`
- **Repository Implementation**: `ModuleMigrationRepository`

**Specific Methods**:
```csharp
// IModuleMigrationRepository (inherits from IRepository<ModuleMigration, Guid>)
Task<IReadOnlyList<ModuleMigration>> GetByModuleIdAsync(Guid tenantId, Guid moduleId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ModuleMigration>> GetPendingMigrationsAsync(Guid tenantId, Guid moduleId, CancellationToken cancellationToken = default);
Task<ModuleMigration?> GetByMigrationNameAsync(Guid tenantId, Guid moduleId, string migrationName, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ModuleMigration>> GetFailedMigrationsAsync(Guid tenantId, CancellationToken cancellationToken = default);
```

---

### 8. Domain Entity System

#### 8.1 EntityDefinition
- **Domain Entity**: `TechWayFit.ContentOS.Entities.Domain.EntityDefinition`
- **DB Row**: `EntityDefinitionRow`
- **Mapper**: `EntityDefinitionMapper`
- **Repository Interface**: `IEntityDefinitionRepository`
- **Repository Implementation**: `EntityDefinitionRepository`

**Specific Methods**:
```csharp
// IEntityDefinitionRepository (inherits from IRepository<EntityDefinition, Guid>)
Task<EntityDefinition?> GetByEntityKeyAsync(Guid tenantId, Guid moduleId, string entityKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<EntityDefinition>> GetByModuleIdAsync(Guid tenantId, Guid moduleId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<EntityDefinition>> GetSystemEntitiesAsync(CancellationToken cancellationToken = default);
```

---

#### 8.2 EntityInstance
- **Domain Entity**: `TechWayFit.ContentOS.Entities.Domain.EntityInstance`
- **DB Row**: `EntityInstanceRow`
- **Mapper**: `EntityInstanceMapper`
- **Repository Interface**: `IEntityInstanceRepository`
- **Repository Implementation**: `EntityInstanceRepository`

**Specific Methods**:
```csharp
// IEntityInstanceRepository (inherits from IRepository<EntityInstance, Guid>)
Task<IReadOnlyList<EntityInstance>> GetByEntityDefinitionIdAsync(Guid tenantId, Guid entityDefinitionId, CancellationToken cancellationToken = default);
Task<EntityInstance?> GetByInstanceKeyAsync(Guid tenantId, Guid entityDefinitionId, string instanceKey, CancellationToken cancellationToken = default);
Task<PagedResult<EntityInstance>> GetByEntityDefinitionPagedAsync(Guid tenantId, Guid entityDefinitionId, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<IReadOnlyList<EntityInstance>> GetByStatusAsync(Guid tenantId, Guid entityDefinitionId, string status, CancellationToken cancellationToken = default);
Task<IReadOnlyList<EntityInstance>> GetChildrenAsync(Guid tenantId, Guid parentInstanceId, CancellationToken cancellationToken = default);
Task<PagedResult<EntityInstance>> SearchByDataAsync(Guid tenantId, Guid entityDefinitionId, string jsonPath, object value, PaginationParameters pagination, CancellationToken cancellationToken = default);
```

---

#### 8.3 EntityRelationship
- **Domain Entity**: `TechWayFit.ContentOS.Entities.Domain.EntityRelationship`
- **DB Row**: `EntityRelationshipRow`
- **Mapper**: `EntityRelationshipMapper`
- **Repository Interface**: `IEntityRelationshipRepository`
- **Repository Implementation**: `EntityRelationshipRepository`

**Specific Methods**:
```csharp
// IEntityRelationshipRepository (inherits from IRepository<EntityRelationship, Guid>)
Task<IReadOnlyList<EntityRelationship>> GetBySourceInstanceAsync(Guid tenantId, Guid sourceInstanceId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<EntityRelationship>> GetByTargetInstanceAsync(Guid tenantId, Guid targetInstanceId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<EntityRelationship>> GetByRelationshipTypeAsync(Guid tenantId, Guid sourceInstanceId, string relationshipType, CancellationToken cancellationToken = default);
Task DeleteBySourceInstanceAsync(Guid tenantId, Guid sourceInstanceId, CancellationToken cancellationToken = default);
```

---

### 9. Business Process & Workflow Extension

#### 9.1 ProcessDefinition
- **Domain Entity**: `TechWayFit.ContentOS.Process.Domain.ProcessDefinition`
- **DB Row**: `ProcessDefinitionRow`
- **Mapper**: `ProcessDefinitionMapper`
- **Repository Interface**: `IProcessDefinitionRepository`
- **Repository Implementation**: `ProcessDefinitionRepository`

**Specific Methods**:
```csharp
// IProcessDefinitionRepository (inherits from IRepository<ProcessDefinition, Guid>)
Task<ProcessDefinition?> GetByProcessKeyAsync(Guid tenantId, Guid moduleId, string processKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ProcessDefinition>> GetByModuleIdAsync(Guid tenantId, Guid moduleId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ProcessDefinition>> GetByProcessTypeAsync(Guid tenantId, string processType, CancellationToken cancellationToken = default);
```

---

#### 9.2 ProcessInstance
- **Domain Entity**: `TechWayFit.ContentOS.Process.Domain.ProcessInstance`
- **DB Row**: `ProcessInstanceRow`
- **Mapper**: `ProcessInstanceMapper`
- **Repository Interface**: `IProcessInstanceRepository`
- **Repository Implementation**: `ProcessInstanceRepository`

**Specific Methods**:
```csharp
// IProcessInstanceRepository (inherits from IRepository<ProcessInstance, Guid>)
Task<IReadOnlyList<ProcessInstance>> GetByProcessDefinitionIdAsync(Guid tenantId, Guid processDefinitionId, CancellationToken cancellationToken = default);
Task<ProcessInstance?> GetByCorrelationKeyAsync(Guid tenantId, string correlationKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ProcessInstance>> GetByEntityInstanceIdAsync(Guid tenantId, Guid entityInstanceId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ProcessInstance>> GetByStatusAsync(Guid tenantId, string status, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ProcessInstance>> GetOverdueInstancesAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<PagedResult<ProcessInstance>> GetRunningInstancesAsync(Guid tenantId, PaginationParameters pagination, CancellationToken cancellationToken = default);
```

---

### 10. Collaboration & Engagement

#### 10.1 Attachment
- **Domain Entity**: `TechWayFit.ContentOS.Collaboration.Domain.Attachment`
- **DB Row**: `AttachmentRow`
- **Mapper**: `AttachmentMapper`
- **Repository Interface**: `IAttachmentRepository`
- **Repository Implementation**: `AttachmentRepository`

**Specific Methods**:
```csharp
// IAttachmentRepository (inherits from IRepository<Attachment, Guid>)
Task<IReadOnlyList<Attachment>> GetByEntityInstanceIdAsync(Guid tenantId, Guid entityInstanceId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Attachment>> GetByModuleIdAsync(Guid tenantId, Guid moduleId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Attachment>> GetUnscannedAttachmentsAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Attachment>> GetByUploadedByAsync(Guid tenantId, Guid uploadedBy, CancellationToken cancellationToken = default);
Task<long> GetTotalSizeByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
```

---

#### 10.2 Comment
- **Domain Entity**: `TechWayFit.ContentOS.Collaboration.Domain.Comment`
- **DB Row**: `CommentRow`
- **Mapper**: `CommentMapper`
- **Repository Interface**: `ICommentRepository`
- **Repository Implementation**: `CommentRepository`

**Specific Methods**:
```csharp
// ICommentRepository (inherits from IRepository<Comment, Guid>)
Task<IReadOnlyList<Comment>> GetByEntityInstanceIdAsync(Guid tenantId, Guid entityInstanceId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Comment>> GetByCreatedByAsync(Guid tenantId, Guid createdBy, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Comment>> GetChildCommentsAsync(Guid tenantId, Guid parentCommentId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<Comment>> GetInternalCommentsAsync(Guid tenantId, Guid entityInstanceId, CancellationToken cancellationToken = default);
Task<PagedResult<Comment>> GetPagedByEntityAsync(Guid tenantId, Guid entityInstanceId, PaginationParameters pagination, CancellationToken cancellationToken = default);
```

---

#### 10.3 ActivityLog
- **Domain Entity**: `TechWayFit.ContentOS.Collaboration.Domain.ActivityLog`
- **DB Row**: `ActivityLogRow`
- **Mapper**: `ActivityLogMapper`
- **Repository Interface**: `IActivityLogRepository`
- **Repository Implementation**: `ActivityLogRepository`

**Specific Methods**:
```csharp
// IActivityLogRepository (inherits from IRepository<ActivityLog, Guid>)
Task<IReadOnlyList<ActivityLog>> GetByEntityInstanceIdAsync(Guid tenantId, Guid entityInstanceId, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ActivityLog>> GetByActorUserIdAsync(Guid tenantId, Guid actorUserId, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ActivityLog>> GetByActivityTypeAsync(Guid tenantId, string activityType, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<PagedResult<ActivityLog>> GetByDateRangeAsync(Guid tenantId, DateTimeOffset startDate, DateTimeOffset endDate, PaginationParameters pagination, CancellationToken cancellationToken = default);
```

---

### 11. Search & Discovery

#### 11.1 SearchIndexEntry
- **Domain Entity**: `TechWayFit.ContentOS.Search.Domain.SearchIndexEntry`
- **DB Row**: `SearchIndexEntryRow`
- **Mapper**: `SearchIndexEntryMapper`
- **Repository Interface**: `ISearchIndexEntryRepository`
- **Repository Implementation**: `SearchIndexEntryRepository`

**Specific Methods**:
```csharp
// ISearchIndexEntryRepository (inherits from IRepository<SearchIndexEntry, Guid>)
Task<IReadOnlyList<SearchIndexEntry>> SearchFullTextAsync(Guid tenantId, string searchTerm, CancellationToken cancellationToken = default);
Task<PagedResult<SearchIndexEntry>> SearchPagedAsync(Guid tenantId, string searchTerm, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<IReadOnlyList<SearchIndexEntry>> GetByEntityInstanceIdAsync(Guid tenantId, Guid entityInstanceId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<SearchIndexEntry>> GetByTagsAsync(Guid tenantId, string[] tags, CancellationToken cancellationToken = default);
Task DeleteByEntityInstanceIdAsync(Guid tenantId, Guid entityInstanceId, CancellationToken cancellationToken = default);
Task<int> RebuildIndexAsync(Guid tenantId, Guid? siteId = null, CancellationToken cancellationToken = default);
```

---

### 12. AI & Vector Search

#### 12.1 ContentRagChunk
- **Domain Entity**: `TechWayFit.ContentOS.AI.Domain.ContentRagChunk`
- **DB Row**: `ContentRagChunkRow`
- **Mapper**: `ContentRagChunkMapper`
- **Repository Interface**: `IContentRagChunkRepository`
- **Repository Implementation**: `ContentRagChunkRepository`

**Specific Methods**:
```csharp
// IContentRagChunkRepository (inherits from IRepository<ContentRagChunk, Guid>)
Task<IReadOnlyList<ContentRagChunk>> GetBySourceAsync(Guid tenantId, string sourceType, Guid sourceId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentRagChunk>> GetBySourceVersionAsync(Guid tenantId, string sourceType, Guid sourceId, Guid sourceVersionId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentRagChunk>> GetByParentChunkAsync(Guid tenantId, Guid parentChunkId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentRagChunk>> GetByLocaleAsync(Guid tenantId, string locale, CancellationToken cancellationToken = default);
Task DeleteBySourceAsync(Guid tenantId, string sourceType, Guid sourceId, CancellationToken cancellationToken = default);
```

---

#### 12.2 ContentEmbedding
- **Domain Entity**: `TechWayFit.ContentOS.AI.Domain.ContentEmbedding`
- **DB Row**: `ContentEmbeddingRow`
- **Mapper**: `ContentEmbeddingMapper`
- **Repository Interface**: `IContentEmbeddingRepository`
- **Repository Implementation**: `ContentEmbeddingRepository`

**Specific Methods**:
```csharp
// IContentEmbeddingRepository (inherits from IRepository<ContentEmbedding, Guid>)
Task<ContentEmbedding?> GetByChunkAndModelAsync(Guid tenantId, Guid chunkId, string embeddingModel, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentEmbedding>> GetByChunkIdAsync(Guid tenantId, Guid chunkId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentEmbedding>> SearchSimilarAsync(Guid tenantId, float[] queryVector, string embeddingModel, int topK, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentEmbedding>> SearchSimilarWithMetadataAsync(Guid tenantId, float[] queryVector, string embeddingModel, Dictionary<string, object> metadataFilters, int topK, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ContentEmbedding>> GetExpiredEmbeddingsAsync(CancellationToken cancellationToken = default);
Task DeleteByChunkIdAsync(Guid tenantId, Guid chunkId, CancellationToken cancellationToken = default);
```

---

#### 12.3 ImageEmbedding
- **Domain Entity**: `TechWayFit.ContentOS.AI.Domain.ImageEmbedding`
- **DB Row**: `ImageEmbeddingRow`
- **Mapper**: `ImageEmbeddingMapper`
- **Repository Interface**: `IImageEmbeddingRepository`
- **Repository Implementation**: `ImageEmbeddingRepository`

**Specific Methods**:
```csharp
// IImageEmbeddingRepository (inherits from IRepository<ImageEmbedding, Guid>)
Task<ImageEmbedding?> GetByAttachmentAndModelAsync(Guid tenantId, Guid attachmentId, string embeddingModel, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ImageEmbedding>> GetByAttachmentIdAsync(Guid tenantId, Guid attachmentId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ImageEmbedding>> SearchSimilarAsync(Guid tenantId, float[] queryVector, string embeddingModel, int topK, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ImageEmbedding>> SearchByOcrTextAsync(Guid tenantId, string searchTerm, CancellationToken cancellationToken = default);
Task<IReadOnlyList<ImageEmbedding>> GetHighQualityImagesAsync(Guid tenantId, float minQualityScore, CancellationToken cancellationToken = default);
```

---

### 13. Background Jobs

#### 13.1 JobDefinition
- **Domain Entity**: `TechWayFit.ContentOS.Jobs.Domain.JobDefinition`
- **DB Row**: `JobDefinitionRow`
- **Mapper**: `JobDefinitionMapper`
- **Repository Interface**: `IJobDefinitionRepository`
- **Repository Implementation**: `JobDefinitionRepository`

**Specific Methods**:
```csharp
// IJobDefinitionRepository (inherits from IRepository<JobDefinition, Guid>)
Task<JobDefinition?> GetByJobKeyAsync(Guid tenantId, string jobKey, CancellationToken cancellationToken = default);
Task<IReadOnlyList<JobDefinition>> GetEnabledJobsAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<JobDefinition>> GetDueJobsAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<JobDefinition>> GetByScheduleTypeAsync(Guid tenantId, string scheduleType, CancellationToken cancellationToken = default);
Task UpdateNextRunTimeAsync(Guid jobDefinitionId, DateTimeOffset nextRunAt, CancellationToken cancellationToken = default);
```

---

#### 13.2 JobExecution
- **Domain Entity**: `TechWayFit.ContentOS.Jobs.Domain.JobExecution`
- **DB Row**: `JobExecutionRow`
- **Mapper**: `JobExecutionMapper`
- **Repository Interface**: `IJobExecutionRepository`
- **Repository Implementation**: `JobExecutionRepository`

**Specific Methods**:
```csharp
// IJobExecutionRepository (inherits from IRepository<JobExecution, Guid>)
Task<IReadOnlyList<JobExecution>> GetByJobDefinitionIdAsync(Guid tenantId, Guid jobDefinitionId, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<IReadOnlyList<JobExecution>> GetPendingExecutionsAsync(Guid tenantId, CancellationToken cancellationToken = default);
Task<JobExecution?> ClaimNextPendingJobAsync(Guid tenantId, string workerInstanceId, CancellationToken cancellationToken = default);
Task<IReadOnlyList<JobExecution>> GetOrphanedExecutionsAsync(CancellationToken cancellationToken = default);
Task<IReadOnlyList<JobExecution>> GetByWorkerInstanceAsync(string workerInstanceId, CancellationToken cancellationToken = default);
Task UpdateHeartbeatAsync(Guid executionId, DateTimeOffset heartbeatAt, DateTimeOffset lockExpiresAt, CancellationToken cancellationToken = default);
Task MarkAsCompletedAsync(Guid executionId, object? resultData, long durationMs, CancellationToken cancellationToken = default);
Task MarkAsFailedAsync(Guid executionId, string errorMessage, string errorType, string? stackTrace, CancellationToken cancellationToken = default);
```

---

#### 13.3 JobExecutionHistory
- **Domain Entity**: `TechWayFit.ContentOS.Jobs.Domain.JobExecutionHistory`
- **DB Row**: `JobExecutionHistoryRow`
- **Mapper**: `JobExecutionHistoryMapper`
- **Repository Interface**: `IJobExecutionHistoryRepository`
- **Repository Implementation**: `JobExecutionHistoryRepository`

**Specific Methods**:
```csharp
// IJobExecutionHistoryRepository (inherits from IRepository<JobExecutionHistory, Guid>)
Task<PagedResult<JobExecutionHistory>> GetByJobDefinitionIdAsync(Guid tenantId, Guid jobDefinitionId, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<PagedResult<JobExecutionHistory>> GetByDateRangeAsync(Guid tenantId, DateTimeOffset startDate, DateTimeOffset endDate, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task<IReadOnlyList<JobExecutionHistory>> GetFailedExecutionsAsync(Guid tenantId, PaginationParameters pagination, CancellationToken cancellationToken = default);
Task ArchiveExecutionAsync(JobExecution execution, CancellationToken cancellationToken = default);
Task PurgeOldHistoryAsync(DateTimeOffset olderThan, CancellationToken cancellationToken = default);
```

---

## Mapper Pattern

All mappers follow a consistent pattern with two static methods:

```csharp
public static class {Entity}Mapper
{
    /// <summary>
    /// Map domain entity to database row
    /// </summary>
    public static {Entity}Row ToRow({Entity} domain)
    {
        return new {Entity}Row
        {
            // Map all properties
        };
    }

    /// <summary>
    /// Map database row to domain entity
    /// </summary>
    public static {Entity} ToDomain({Entity}Row row)
    {
        // Use reflection or constructor to create domain entity
        // (since domain entities have private setters)
        return entity;
    }
}
```

---

## Summary Statistics

### Total Entities: 38

**By Module**:
- Tenancy: 2 (Tenant, Site)
- Identity: 5 (User, Role, Group, UserRole, UserGroup)
- Content: 10 (ContentNode, Route, ContentType, ContentTypeField, ContentItem, ContentVersion, ContentFieldValue, LayoutDefinition, ComponentDefinition, ContentLayout)
- Security: 2 (AclEntry, PreviewToken)
- Workflow: 3 (WorkflowDefinition, WorkflowState, WorkflowTransition)
- Audit: 1 (AuditLog)
- Modules: 4 (Module, ModuleCapability, ModuleSetting, ModuleMigration)
- Domain Entities: 3 (EntityDefinition, EntityInstance, EntityRelationship)
- Process: 2 (ProcessDefinition, ProcessInstance)
- Collaboration: 3 (Attachment, Comment, ActivityLog)
- Search: 1 (SearchIndexEntry)
- AI: 3 (ContentRagChunk, ContentEmbedding, ImageEmbedding)
- Jobs: 3 (JobDefinition, JobExecution, JobExecutionHistory)

**Total Repository Methods**: ~400+ (including base + specific methods)

---

## Implementation Guidelines

### 1. Creating New Entities

1. **Domain Entity** (in feature project):
   - Use private setters
   - Include factory methods
   - Add validation logic
   - No EF dependencies

2. **DB Row Entity** (in Persistence.Postgres):
   - Name with `Row` suffix
   - Public getters/setters
   - Configure with Fluent API in separate configuration class

3. **Mapper** (in Persistence.Postgres):
   - Static class with `ToRow()` and `ToDomain()`
   - Use reflection for domain entities with private setters

4. **Repository Interface** (in feature project Ports folder):
   - Inherit from `IRepository<TEntity, TKey>`
   - Add entity-specific methods

5. **Repository Implementation** (in Persistence.Postgres):
   - Inherit from `BaseRepository<TEntity, TRow, TKey>`
   - Implement provider-specific logic
   - Override search if needed

### 2. Multi-Tenancy

ALL repositories (except Tenant itself) must:
- Filter by `TenantId` in all queries
- Include `TenantId` in uniqueness constraints
- Use global query filters in DbContext

### 3. Soft Deletes

Entities with soft delete support:
- Include `IsDeleted`, `DeletedOn`, `DeletedBy` fields
- Filter out deleted records by default
- Provide methods to include deleted records if needed

### 4. Performance Considerations

- Use pagination for large result sets
- Implement search predicates only where needed
- Consider caching for frequently accessed data
- Use indexes on commonly queried fields

---

## File Locations

```
src/
├── core/
│   └── TechWayFit.ContentOS.Abstractions/
│       ├── Repositories/
│       │   └── IRepository.cs
│       ├── Pagination/
│       │   ├── PagedResult.cs
│       │   └── PaginationParameters.cs
│       └── Filtering/
│           └── FilterSpecification.cs
│
├── features/
│   ├── tenancy/TechWayFit.ContentOS.Tenancy/
│   │   ├── Domain/
│   │   │   ├── Tenant.cs
│   │   │   └── Site.cs
│   │   └── Ports/
│   │       ├── ITenantRepository.cs
│   │       └── ISiteRepository.cs
│   │
│   ├── [other features follow same pattern]
│
└── infrastructure/
    └── persistence/
        ├── TechWayFit.ContentOS.Infrastructure.Persistence/
        │   └── Repositories/
        │       └── BaseRepository.cs
        │
        └── TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/
            ├── Entities/
            │   ├── TenantRow.cs
            │   ├── SiteRow.cs
            │   └── [all other Row entities]
            │
            ├── Mappers/
            │   ├── TenantMapper.cs
            │   ├── SiteMapper.cs
            │   └── [all other mappers]
            │
            └── Repositories/
                ├── TenantRepository.cs
                ├── SiteRepository.cs
                └── [all other repository implementations]
```

---

## Next Steps

To implement this architecture:

1. ✅ Create base interfaces and abstractions
2. ✅ Create BaseRepository implementation
3. ⏳ Create all domain entities (38 entities)
4. ⏳ Create all DB row entities (38 Row classes)
5. ⏳ Create all mappers (38 mapper classes)
6. ⏳ Create all repository interfaces (38 interfaces)
7. ⏳ Create all repository implementations (38 implementations)
8. ⏳ Add EF Core configurations
9. ⏳ Create database migrations
10. ⏳ Add unit tests for repositories

---

**Document Version**: 1.0  
**Last Updated**: {{ today }}  
**Status**: In Progress
