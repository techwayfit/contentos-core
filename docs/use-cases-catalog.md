# ContentOS Use Cases Catalog

This document catalogs all use cases for domain operations across ContentOS features.

---

## 1. Content Feature

### 1.1 Content Item Management

#### Core CRUD Operations
- **CreateContentItemUseCase**
  - Create a new content item with default language
  - Validate content type
  - Initialize workflow status as Draft
  - Create initial localization
  - Raise `ContentItemCreated` event

- **UpdateContentItemUseCase**
  - Update content item metadata (type, default language)
  - Check workflow status permissions
  - Update modification timestamp
  - Raise `ContentItemUpdated` event

- **DeleteContentItemUseCase**
  - Soft delete content item
  - Archive all localizations
  - Update workflow status to Archived
  - Check for dependencies (routes, layouts)
  - Raise `ContentItemDeleted` event

- **GetContentItemByIdUseCase**
  - Retrieve content item by ID
  - Include localizations (optional)
  - Filter by tenant and site

- **GetContentItemBySlugUseCase**
  - Retrieve content item by slug and language
  - Resolve route information
  - Include published versions only (optional)

- **ListContentItemsUseCase**
  - List content items with pagination
  - Filter by: content type, status, site, language
  - Sort by: created date, updated date, title
  - Search by title/slug

#### Versioning
- **CreateContentVersionUseCase**
  - Create a new version from current content
  - Snapshot field values
  - Associate with user and timestamp
  - Raise `ContentVersionCreated` event

- **RestoreContentVersionUseCase**
  - Restore content to a specific version
  - Update field values from snapshot
  - Create new version for rollback
  - Raise `ContentVersionRestored` event

- **ListContentVersionsUseCase**
  - List all versions for a content item
  - Include version metadata (author, date, version number)
  - Filter by date range

- **CompareContentVersionsUseCase**
  - Compare two versions side-by-side
  - Highlight field differences
  - Return structured diff

#### Publishing & Workflow
- **PublishContentItemUseCase**
  - Validate workflow transition
  - Check user permissions
  - Update status to Published
  - Set publish date
  - Raise `ContentItemPublished` event

- **UnpublishContentItemUseCase**
  - Transition from Published to Draft
  - Validate permissions
  - Raise `ContentItemUnpublished` event

- **ArchiveContentItemUseCase**
  - Move content to archived state
  - Retain history
  - Remove from active routes
  - Raise `ContentItemArchived` event

- **SubmitContentForApprovalUseCase**
  - Transition from Draft to InReview
  - Assign to reviewers
  - Notify approvers
  - Raise `ContentSubmittedForReview` event

- **ApproveContentItemUseCase**
  - Approve content in review
  - Validate approver permissions
  - Transition to Approved state
  - Raise `ContentApproved` event

- **RejectContentItemUseCase**
  - Reject content in review
  - Include rejection reason
  - Transition back to Draft
  - Notify author
  - Raise `ContentRejected` event

### 1.2 Content Localization Management

- **AddLocalizationUseCase**
  - Add new language variant to content item
  - Initialize field values from default language (optional)
  - Validate language code
  - Raise `LocalizationAdded` event

- **UpdateLocalizationUseCase**
  - Update field values for specific language
  - Update modification metadata
  - Raise `LocalizationUpdated` event

- **RemoveLocalizationUseCase**
  - Remove language variant
  - Cannot remove default language
  - Raise `LocalizationRemoved` event

- **GetLocalizationUseCase**
  - Retrieve localization by content ID and language
  - Include field values
  - Check publish status

- **ListLocalizationsUseCase**
  - List all localizations for a content item
  - Filter by published status
  - Include field values (optional)

- **CloneLocalizationUseCase**
  - Copy field values from one language to another
  - Preserve source localization
  - Create or update target localization
  - Raise `LocalizationCloned` event

### 1.3 Content Type Management

- **CreateContentTypeUseCase**
  - Create new content type definition
  - Define fields and validation rules
  - Set icon, description
  - Raise `ContentTypeCreated` event

- **UpdateContentTypeUseCase**
  - Update content type metadata
  - Add/remove/modify fields
  - Validate backward compatibility
  - Raise `ContentTypeUpdated` event

- **DeleteContentTypeUseCase**
  - Delete content type
  - Check for existing content items
  - Prevent deletion if in use
  - Raise `ContentTypeDeleted` event

- **GetContentTypeUseCase**
  - Retrieve content type by ID or key
  - Include field definitions

- **ListContentTypesUseCase**
  - List all content types for tenant
  - Filter by category
  - Include usage statistics (optional)

- **ValidateContentTypeFieldsUseCase**
  - Validate field configuration
  - Check for naming conflicts
  - Validate field types and constraints

### 1.4 Content Field Management

- **AddFieldToContentTypeUseCase**
  - Add new field to content type
  - Validate field type and constraints
  - Set default value (optional)
  - Raise `FieldAddedToContentType` event

- **UpdateContentTypeFieldUseCase**
  - Update field configuration
  - Validate data compatibility
  - Update existing content (if needed)
  - Raise `ContentTypeFieldUpdated` event

- **RemoveFieldFromContentTypeUseCase**
  - Remove field from content type
  - Check for existing data
  - Archive field values
  - Raise `FieldRemovedFromContentType` event

- **ReorderFieldsUseCase**
  - Update field display order
  - Validate new ordering
  - Update UI configuration

- **SetFieldValueUseCase**
  - Update single field value for localization
  - Validate against field type constraints
  - Trigger field-level validation
  - Raise `FieldValueUpdated` event

- **BulkUpdateFieldValuesUseCase**
  - Update multiple field values at once
  - Validate all values
  - Transactional update
  - Raise `FieldValuesUpdated` event

### 1.5 Content Hierarchy Management

- **CreateContentNodeUseCase**
  - Create node in content tree
  - Set parent node
  - Define sort order
  - Raise `ContentNodeCreated` event

- **MoveContentNodeUseCase**
  - Move node to new parent
  - Update hierarchy path
  - Reorder siblings
  - Raise `ContentNodeMoved` event

- **DeleteContentNodeUseCase**
  - Remove node from hierarchy
  - Handle child nodes (cascade or reassign)
  - Update routes
  - Raise `ContentNodeDeleted` event

- **GetContentTreeUseCase**
  - Retrieve hierarchical content tree
  - Filter by depth level
  - Include metadata (optional)

- **GetBreadcrumbsUseCase**
  - Get ancestor path for content node
  - Return ordered list from root to current
  - Include URLs

- **ReorderSiblingsUseCase**
  - Update sort order for sibling nodes
  - Validate new ordering
  - Update navigation

### 1.6 Route Management

- **CreateRouteUseCase**
  - Create URL route for content item
  - Validate route pattern uniqueness
  - Set language and site scope
  - Raise `RouteCreated` event

- **UpdateRouteUseCase**
  - Update route pattern
  - Validate no conflicts
  - Set up redirect from old route (optional)
  - Raise `RouteUpdated` event

- **DeleteRouteUseCase**
  - Remove route
  - Configure redirect handling
  - Raise `RouteDeleted` event

- **ResolveRouteUseCase**
  - Find content by URL pattern
  - Match language and site
  - Return content item or 404

- **ListRoutesForContentUseCase**
  - Get all routes for a content item
  - Include language variants
  - Show active/inactive status

- **ValidateRouteUniquenessUseCase**
  - Check if route pattern is unique
  - Scope by site and language
  - Return conflicts

### 1.7 Layout Management

- **CreateLayoutDefinitionUseCase**
  - Create reusable layout template
  - Define zones and structure
  - Set metadata (name, description)
  - Raise `LayoutDefinitionCreated` event

- **UpdateLayoutDefinitionUseCase**
  - Update layout structure
  - Modify zones
  - Validate compatibility with existing usage
  - Raise `LayoutDefinitionUpdated` event

- **DeleteLayoutDefinitionUseCase**
  - Remove layout definition
  - Check for usage in content
  - Prevent deletion if in use
  - Raise `LayoutDefinitionDeleted` event

- **AssignLayoutToContentUseCase**
  - Link layout definition to content item
  - Configure zone mappings
  - Set as default (optional)
  - Raise `LayoutAssignedToContent` event

- **RemoveLayoutFromContentUseCase**
  - Unlink layout from content
  - Clear zone configurations
  - Raise `LayoutRemovedFromContent` event

### 1.8 Component Management

- **CreateComponentDefinitionUseCase**
  - Create reusable component
  - Define component schema
  - Set configuration options
  - Raise `ComponentDefinitionCreated` event

- **UpdateComponentDefinitionUseCase**
  - Update component definition
  - Modify schema
  - Validate backward compatibility
  - Raise `ComponentDefinitionUpdated` event

- **DeleteComponentDefinitionUseCase**
  - Remove component definition
  - Check usage in layouts
  - Prevent deletion if in use
  - Raise `ComponentDefinitionDeleted` event

- **AddComponentToLayoutUseCase**
  - Place component in layout zone
  - Configure component settings
  - Set rendering order
  - Raise `ComponentAddedToLayout` event

- **RemoveComponentFromLayoutUseCase**
  - Remove component from zone
  - Update layout configuration
  - Raise `ComponentRemovedFromLayout` event

---

## 2. Workflow Feature

### 2.1 Workflow Definition Management

- **CreateWorkflowDefinitionUseCase**
  - Create new workflow template
  - Define initial state
  - Set workflow metadata
  - Raise `WorkflowDefinitionCreated` event

- **UpdateWorkflowDefinitionUseCase**
  - Update workflow metadata
  - Modify states and transitions
  - Validate workflow integrity
  - Raise `WorkflowDefinitionUpdated` event

- **DeleteWorkflowDefinitionUseCase**
  - Remove workflow definition
  - Check for active usage
  - Prevent deletion if in use
  - Raise `WorkflowDefinitionDeleted` event

- **GetWorkflowDefinitionUseCase**
  - Retrieve workflow by ID
  - Include states and transitions

- **ListWorkflowDefinitionsUseCase**
  - List all workflows for tenant
  - Filter by active/inactive
  - Include usage statistics

- **ValidateWorkflowDefinitionUseCase**
  - Check workflow integrity
  - Validate state connectivity
  - Ensure no orphaned states
  - Check for cycles

### 2.2 Workflow State Management

- **AddStateToWorkflowUseCase**
  - Add new state to workflow
  - Define state type (draft/published/archived)
  - Set permissions
  - Raise `StateAddedToWorkflow` event

- **UpdateWorkflowStateUseCase**
  - Update state configuration
  - Modify permissions
  - Update metadata
  - Raise `WorkflowStateUpdated` event

- **RemoveStateFromWorkflowUseCase**
  - Remove state from workflow
  - Check for content in this state
  - Migrate content to another state
  - Raise `StateRemovedFromWorkflow` event

- **SetInitialStateUseCase**
  - Define starting state for workflow
  - Validate state exists
  - Update workflow configuration

### 2.3 Workflow Transition Management

- **AddTransitionToWorkflowUseCase**
  - Create transition between states
  - Define required permissions
  - Set conditions (optional)
  - Raise `TransitionAddedToWorkflow` event

- **UpdateWorkflowTransitionUseCase**
  - Update transition rules
  - Modify permissions
  - Update conditions
  - Raise `WorkflowTransitionUpdated` event

- **RemoveTransitionFromWorkflowUseCase**
  - Remove transition
  - Validate workflow still valid
  - Raise `TransitionRemovedFromWorkflow` event

- **GetAvailableTransitionsUseCase**
  - Get transitions available from current state
  - Filter by user permissions
  - Include transition metadata

### 2.4 Workflow Execution

- **ExecuteTransitionUseCase**
  - Execute workflow transition
  - Validate permissions
  - Evaluate conditions
  - Update content state
  - Trigger actions (notifications, etc.)
  - Raise `TransitionExecuted` event

- **GetWorkflowHistoryUseCase**
  - Retrieve workflow history for content
  - Include all transitions
  - Show users and timestamps

- **GetCurrentWorkflowStateUseCase**
  - Get current state for content item
  - Include available transitions
  - Show state metadata

---

## 3. Tenancy Feature

### 3.1 Tenant Management

- **CreateTenantUseCase**
  - Create new tenant
  - Initialize tenant database schema
  - Set up default configurations
  - Raise `TenantCreated` event

- **UpdateTenantUseCase**
  - Update tenant metadata
  - Modify settings
  - Update subscription info
  - Raise `TenantUpdated` event

- **SuspendTenantUseCase**
  - Suspend tenant access
  - Disable login
  - Set suspension reason
  - Raise `TenantSuspended` event

- **ActivateTenantUseCase**
  - Reactivate suspended tenant
  - Restore access
  - Raise `TenantActivated` event

- **DeleteTenantUseCase**
  - Permanently delete tenant (SuperAdmin only)
  - Archive all tenant data
  - Remove from active tenants
  - Raise `TenantDeleted` event

- **GetTenantUseCase**
  - Retrieve tenant by ID
  - Include configuration and status

- **ListTenantsUseCase** (SuperAdmin only)
  - List all tenants in system
  - Filter by status
  - Include usage statistics

### 3.2 Site Management

- **CreateSiteUseCase**
  - Create new site within tenant
  - Set domain and configuration
  - Initialize default settings
  - Raise `SiteCreated` event

- **UpdateSiteUseCase**
  - Update site metadata
  - Modify domain and settings
  - Update theme configuration
  - Raise `SiteUpdated` event

- **DeleteSiteUseCase**
  - Remove site from tenant
  - Check for content dependencies
  - Archive site data
  - Raise `SiteDeleted` event

- **GetSiteUseCase**
  - Retrieve site by ID or domain
  - Include configuration

- **ListSitesUseCase**
  - List all sites for tenant
  - Filter by status
  - Include statistics

- **SetDefaultSiteUseCase**
  - Set default site for tenant
  - Update tenant configuration

### 3.3 User Management

- **CreateUserUseCase**
  - Create new user account
  - Set email and credentials
  - Assign default role
  - Send welcome email
  - Raise `UserCreated` event

- **UpdateUserUseCase**
  - Update user profile
  - Modify email, name
  - Update preferences
  - Raise `UserUpdated` event

- **DeactivateUserUseCase**
  - Deactivate user account
  - Revoke access
  - Retain user data
  - Raise `UserDeactivated` event

- **ReactivateUserUseCase**
  - Reactivate deactivated user
  - Restore permissions
  - Raise `UserReactivated` event

- **DeleteUserUseCase**
  - Permanently remove user
  - Anonymize or reassign content
  - Remove from groups
  - Raise `UserDeleted` event

- **GetUserUseCase**
  - Retrieve user by ID or email
  - Include roles and groups

- **ListUsersUseCase**
  - List users for tenant
  - Filter by role, status
  - Search by name/email

- **ChangeUserPasswordUseCase**
  - Update user password
  - Validate password strength
  - Hash and store securely
  - Invalidate existing sessions (optional)

- **ResetUserPasswordUseCase**
  - Send password reset email
  - Generate reset token
  - Set token expiration

### 3.4 Role Management

- **CreateRoleUseCase**
  - Create new role
  - Define permissions
  - Set role metadata
  - Raise `RoleCreated` event

- **UpdateRoleUseCase**
  - Update role configuration
  - Add/remove permissions
  - Modify metadata
  - Raise `RoleUpdated` event

- **DeleteRoleUseCase**
  - Remove role
  - Check for assigned users
  - Migrate users to another role
  - Raise `RoleDeleted` event

- **AssignRoleToUserUseCase**
  - Grant role to user
  - Update user permissions
  - Raise `RoleAssignedToUser` event

- **RemoveRoleFromUserUseCase**
  - Revoke role from user
  - Update permissions
  - Raise `RoleRemovedFromUser` event

- **GetRoleUseCase**
  - Retrieve role by ID or name
  - Include permissions

- **ListRolesUseCase**
  - List all roles for tenant
  - Include permission counts

### 3.5 Group Management

- **CreateGroupUseCase**
  - Create user group
  - Set group metadata
  - Raise `GroupCreated` event

- **UpdateGroupUseCase**
  - Update group information
  - Modify metadata
  - Raise `GroupUpdated` event

- **DeleteGroupUseCase**
  - Remove group
  - Remove user memberships
  - Raise `GroupDeleted` event

- **AddUserToGroupUseCase**
  - Add user to group
  - Update group membership
  - Raise `UserAddedToGroup` event

- **RemoveUserFromGroupUseCase**
  - Remove user from group
  - Update membership
  - Raise `UserRemovedFromGroup` event

- **GetGroupUseCase**
  - Retrieve group by ID
  - Include member count

- **ListGroupsUseCase**
  - List all groups for tenant
  - Filter by type
  - Include member counts

- **ListGroupMembersUseCase**
  - List users in a group
  - Include user metadata
  - Pagination support

---

## 4. Media Feature (To Be Defined)

### 4.1 Media Asset Management

- **UploadMediaAssetUseCase**
  - Upload file to storage
  - Generate thumbnails
  - Extract metadata
  - Raise `MediaAssetUploaded` event

- **UpdateMediaMetadataUseCase**
  - Update alt text, title, description
  - Add tags and categories
  - Raise `MediaMetadataUpdated` event

- **DeleteMediaAssetUseCase**
  - Remove asset from storage
  - Check for usage in content
  - Soft delete with grace period
  - Raise `MediaAssetDeleted` event

- **GetMediaAssetUseCase**
  - Retrieve asset by ID
  - Include metadata and URLs

- **ListMediaAssetsUseCase**
  - List assets with pagination
  - Filter by type, tags, date
  - Search by name

### 4.2 Media Folder Management

- **CreateMediaFolderUseCase**
  - Create folder for organization
  - Set parent folder
  - Define permissions
  - Raise `MediaFolderCreated` event

- **MoveMediaAssetUseCase**
  - Move asset to different folder
  - Update paths
  - Raise `MediaAssetMoved` event

- **DeleteMediaFolderUseCase**
  - Remove folder
  - Handle contained assets
  - Raise `MediaFolderDeleted` event

### 4.3 Media Processing

- **GenerateThumbnailsUseCase**
  - Create thumbnails for images
  - Multiple sizes
  - Store in CDN

- **OptimizeImageUseCase**
  - Compress image
  - Convert format
  - Update metadata

- **ExtractMediaMetadataUseCase**
  - Extract EXIF data
  - Get dimensions, format
  - Detect content type

---

## 5. Search Feature (To Be Defined)

### 5.1 Index Management

- **IndexContentItemUseCase**
  - Add content to search index
  - Extract searchable fields
  - Apply boosting rules
  - Raise `ContentIndexed` event

- **ReindexContentItemUseCase**
  - Update existing index entry
  - Refresh all fields
  - Raise `ContentReindexed` event

- **RemoveFromIndexUseCase**
  - Remove content from index
  - Clean up references
  - Raise `ContentRemovedFromIndex` event

- **BulkReindexUseCase**
  - Reindex multiple items
  - Background processing
  - Progress tracking

### 5.2 Search Operations

- **SearchContentUseCase**
  - Full-text search
  - Filter by content type, status
  - Faceted search
  - Pagination and sorting

- **SuggestSearchTermsUseCase**
  - Auto-complete suggestions
  - Based on index
  - Personalized (optional)

- **GetPopularSearchesUseCase**
  - Retrieve trending searches
  - Time-based filtering
  - Tenant-specific

### 5.3 AI-Powered Search (To Be Defined)

- **VectorSearchUseCase**
  - Semantic search using embeddings
  - Find similar content
  - Ranking by relevance

- **GenerateSearchEmbeddingsUseCase**
  - Create vector embeddings for content
  - Store in vector database
  - Update on content change

---

## 6. AI Feature (To Be Defined)

### 6.1 Content Generation

- **GenerateContentDraftUseCase**
  - AI-generated content from prompt
  - Based on content type
  - Apply brand guidelines

- **SuggestContentImprovementsUseCase**
  - Analyze existing content
  - Suggest improvements
  - SEO optimization

### 6.2 Content Analysis

- **AnalyzeSentimentUseCase**
  - Determine content sentiment
  - Multi-language support
  - Return confidence score

- **ExtractKeywordsUseCase**
  - Identify key topics
  - Generate tags
  - SEO keyword suggestions

- **CheckContentQualityUseCase**
  - Grammar and spelling
  - Readability score
  - Accessibility checks

---

## Implementation Notes

### Cross-Cutting Concerns

All use cases should implement:

1. **Validation**
   - Input validation
   - Business rule validation
   - Permission checks

2. **Events**
   - Domain event publishing
   - Event metadata (timestamp, user)

3. **Audit**
   - Operation logging
   - User tracking
   - Change history

4. **Tenancy**
   - Tenant isolation
   - Multi-tenancy enforcement
   - Tenant-specific configuration

5. **Error Handling**
   - Return `Result<T>` pattern
   - Structured error messages
   - Localized error text

6. **Transactions**
   - Use `IUnitOfWork`
   - Atomic operations
   - Rollback on failure

### Use Case Structure

```csharp
public class SampleUseCase
{
    private readonly IRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPolicyEvaluator _policyEvaluator;
    private readonly IEventPublisher _eventPublisher;

    public async Task<Result<Output>> ExecuteAsync(
        Input input,
        CancellationToken cancellationToken = default)
    {
        // 1. Validate input
        // 2. Check permissions
        // 3. Execute business logic
        // 4. Persist changes
        // 5. Publish events
        // 6. Return result
    }
}
```

### Naming Conventions

- Use cases end with `UseCase`
- Commands are imperative (Create, Update, Delete)
- Queries describe what they return (Get, List, Search)
- Events are past tense (Created, Updated, Deleted)

---

## Next Steps

1. **Prioritize Implementation**
   - MVP: Content CRUD, Tenant Management, Basic Workflow
   - Phase 2: Advanced Content Features, Media
   - Phase 3: Search, AI Features

2. **Create Use Case Classes**
   - Implement one feature at a time
   - Start with Content feature
   - Follow architecture rules strictly

3. **Add Integration Tests**
   - Test each use case
   - Verify event publishing
   - Check permission enforcement

4. **Document API Endpoints**
   - Map use cases to HTTP endpoints
   - Define request/response DTOs
   - Document authentication requirements
