# Repository Implementation Guide

This guide provides step-by-step templates for creating repositories for all ContentOS entities following the established patterns.

## Repository Creation Checklist

For each entity, you need to create:
1. ✅ Domain Entity (Feature project)
2. ✅ DB Row Entity (Persistence.Postgres)
3. ✅ Mapper (Persistence.Postgres)
4. ✅ Repository Interface (Feature Ports)
5. ✅ Repository Implementation (Persistence.Postgres)

---

## Template 1: Domain Entity

**Location**: `src/features/{feature}/TechWayFit.ContentOS.{Feature}/Domain/{EntityName}.cs`

```csharp
namespace TechWayFit.ContentOS.{Feature}.Domain;

/// <summary>
/// {EntityName} domain entity
/// Represents {business description}
/// </summary>
public sealed class {EntityName}
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; } // Multi-tenancy required
    public string Name { get; private set; } = default!;
    public {Status}Status Status { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTimeOffset? UpdatedOn { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    
    // Soft delete fields (if applicable)
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedOn { get; private set; }
    public Guid? DeletedBy { get; private set; }
    
    // System fields (if applicable)
    public bool IsActive { get; private set; }
    public bool CanDelete { get; private set; }
    public bool IsSystem { get; private set; }

    // Private constructor for EF Core/mapper
    private {EntityName}() { }

    /// <summary>
    /// Factory method to create new entity
    /// </summary>
    public static {EntityName} Create(Guid tenantId, string name, Guid createdBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        return new {EntityName}
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = name,
            Status = {Status}Status.Active,
            CreatedOn = DateTimeOffset.UtcNow,
            CreatedBy = createdBy,
            IsActive = true,
            CanDelete = true,
            IsDeleted = false,
            IsSystem = false
        };
    }

    /// <summary>
    /// Update entity
    /// </summary>
    public void Update(string name, Guid updatedBy)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name;
        UpdatedOn = DateTimeOffset.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Soft delete
    /// </summary>
    public void Delete(Guid deletedBy)
    {
        if (!CanDelete)
            throw new InvalidOperationException("This entity cannot be deleted");

        IsDeleted = true;
        DeletedOn = DateTimeOffset.UtcNow;
        DeletedBy = deletedBy;
        IsActive = false;
    }
}

/// <summary>
/// Entity status enumeration
/// </summary>
public enum {Status}Status
{
    Active = 0,
    Inactive = 1,
    Archived = 2
}
```

---

## Template 2: DB Row Entity

**Location**: `src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/Entities/{EntityName}Row.cs`

```csharp
namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

/// <summary>
/// Database row for {EntityName} entity
/// </summary>
public sealed class {EntityName}Row
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = default!;
    public int Status { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    public bool IsActive { get; set; }
    public bool CanDelete { get; set; }
    public bool IsSystem { get; set; }
    
    // Navigation properties (if needed)
    // public virtual TenantRow Tenant { get; set; } = default!;
}
```

---

## Template 3: Mapper

**Location**: `src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/Mappers/{EntityName}Mapper.cs`

```csharp
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;
using TechWayFit.ContentOS.{Feature}.Domain;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Mappers;

/// <summary>
/// Mapper between {EntityName} domain entity and {EntityName}Row
/// </summary>
public static class {EntityName}Mapper
{
    public static {EntityName}Row ToRow({EntityName} domain)
    {
        return new {EntityName}Row
        {
            Id = domain.Id,
            TenantId = domain.TenantId,
            Name = domain.Name,
            Status = (int)domain.Status,
            CreatedOn = domain.CreatedOn,
            CreatedBy = domain.CreatedBy,
            UpdatedOn = domain.UpdatedOn,
            UpdatedBy = domain.UpdatedBy,
            IsDeleted = domain.IsDeleted,
            DeletedOn = domain.DeletedOn,
            DeletedBy = domain.DeletedBy,
            IsActive = domain.IsActive,
            CanDelete = domain.CanDelete,
            IsSystem = domain.IsSystem
        };
    }

    public static {EntityName} ToDomain({EntityName}Row row)
    {
        // Use reflection to create domain entity (since constructor is private)
        var entity = ({EntityName})System.Runtime.Serialization.FormatterServices
            .GetUninitializedObject(typeof({EntityName}));

        var type = typeof({EntityName});
        type.GetProperty(nameof({EntityName}.Id))!.SetValue(entity, row.Id);
        type.GetProperty(nameof({EntityName}.TenantId))!.SetValue(entity, row.TenantId);
        type.GetProperty(nameof({EntityName}.Name))!.SetValue(entity, row.Name);
        type.GetProperty(nameof({EntityName}.Status))!.SetValue(entity, ({Status}Status)row.Status);
        type.GetProperty(nameof({EntityName}.CreatedOn))!.SetValue(entity, row.CreatedOn);
        type.GetProperty(nameof({EntityName}.CreatedBy))!.SetValue(entity, row.CreatedBy);
        type.GetProperty(nameof({EntityName}.UpdatedOn))!.SetValue(entity, row.UpdatedOn);
        type.GetProperty(nameof({EntityName}.UpdatedBy))!.SetValue(entity, row.UpdatedBy);
        type.GetProperty(nameof({EntityName}.IsDeleted))!.SetValue(entity, row.IsDeleted);
        type.GetProperty(nameof({EntityName}.DeletedOn))!.SetValue(entity, row.DeletedOn);
        type.GetProperty(nameof({EntityName}.DeletedBy))!.SetValue(entity, row.DeletedBy);
        type.GetProperty(nameof({EntityName}.IsActive))!.SetValue(entity, row.IsActive);
        type.GetProperty(nameof({EntityName}.CanDelete))!.SetValue(entity, row.CanDelete);
        type.GetProperty(nameof({EntityName}.IsSystem))!.SetValue(entity, row.IsSystem);

        return entity;
    }
}
```

---

## Template 4: Repository Interface

**Location**: `src/features/{feature}/TechWayFit.ContentOS.{Feature}/Ports/I{EntityName}Repository.cs`

```csharp
using TechWayFit.ContentOS.Abstractions.Repositories;
using TechWayFit.ContentOS.{Feature}.Domain;

namespace TechWayFit.ContentOS.{Feature}.Ports;

/// <summary>
/// Repository contract for {EntityName} persistence
/// Inherits base CRUD operations from IRepository
/// </summary>
public interface I{EntityName}Repository : IRepository<{EntityName}, Guid>
{
    /// <summary>
    /// Get {EntityName} by tenant
    /// </summary>
    Task<IReadOnlyList<{EntityName}>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get {EntityName} by name (tenant-scoped)
    /// </summary>
    Task<{EntityName}?> GetByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if name exists (tenant-scoped)
    /// </summary>
    Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active entities by tenant
    /// </summary>
    Task<IReadOnlyList<{EntityName}>> GetActiveByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    
    // Add other specific methods as needed
}
```

---

## Template 5: Repository Implementation

**Location**: `src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/Repositories/{EntityName}Repository.cs`

```csharp
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Mappers;
using TechWayFit.ContentOS.{Feature}.Domain;
using TechWayFit.ContentOS.{Feature}.Ports;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Repositories;

/// <summary>
/// EF Core implementation of I{EntityName}Repository
/// Inherits from EfCoreBaseRepository for common CRUD operations
/// </summary>
public sealed class {EntityName}Repository : EfCoreBaseRepository<{EntityName}, {EntityName}Row, Guid>, I{EntityName}Repository
{
    public {EntityName}Repository(ContentOsDbContext context) : base(context)
    {
    }

    // ==================== MAPPER IMPLEMENTATIONS ====================

    protected override {EntityName} MapToDomain({EntityName}Row row) => {EntityName}Mapper.ToDomain(row);
    protected override {EntityName}Row MapToRow({EntityName} entity) => {EntityName}Mapper.ToRow(entity);
    protected override Guid GetRowId({EntityName}Row row) => row.Id;
    protected override Guid GetEntityId({EntityName} entity) => entity.Id;

    protected override Expression<Func<{EntityName}Row, bool>>? MapToRowPredicate(Expression<Func<{EntityName}, bool>> predicate)
    {
        // Complex predicate mapping can be added if needed
        return null;
    }

    protected override Expression<Func<{EntityName}Row, bool>>? GetSearchPredicate(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return null;

        var term = searchTerm.ToLower();
        return row => row.Name.ToLower().Contains(term);
    }

    // ==================== SPECIFIC REPOSITORY METHODS ====================

    public async Task<IReadOnlyList<{EntityName}>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<{EntityName}Row>()
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return rows.Select(MapToDomain).ToList();
    }

    public async Task<{EntityName}?> GetByNameAsync(Guid tenantId, string name, CancellationToken cancellationToken = default)
    {
        var row = await Context.Set<{EntityName}Row>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Name == name && !x.IsDeleted, cancellationToken);

        return row == null ? null : MapToDomain(row);
    }

    public async Task<bool> NameExistsAsync(Guid tenantId, string name, CancellationToken cancellationToken = default)
    {
        return await Context.Set<{EntityName}Row>()
            .AnyAsync(x => x.TenantId == tenantId && x.Name == name && !x.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<{EntityName}>> GetActiveByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var rows = await Context.Set<{EntityName}Row>()
            .AsNoTracking()
            .Where(x => x.TenantId == tenantId && x.IsActive && !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return rows.Select(MapToDomain).ToList();
    }
}
```

---

## EF Core Configuration Template

**Location**: `src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/Configurations/{EntityName}Configuration.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Entities;

namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Configurations;

/// <summary>
/// EF Core configuration for {EntityName}Row
/// </summary>
public class {EntityName}Configuration : IEntityTypeConfiguration<{EntityName}Row>
{
    public void Configure(EntityTypeBuilder<{EntityName}Row> builder)
    {
        builder.ToTable("{table_name}"); // Use snake_case

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired();

        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .IsRequired();

        // Indexes
        builder.HasIndex(x => new { x.TenantId, x.Name })
            .IsUnique()
            .HasFilter("is_deleted = false"); // PostgreSQL syntax for soft delete

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => new { x.TenantId, x.IsActive });
        builder.HasIndex(x => new { x.TenantId, x.Status });

        // Global query filter for soft delete
        builder.HasQueryFilter(x => !x.IsDeleted);

        // Foreign keys (if applicable)
        // builder.HasOne(x => x.Tenant)
        //     .WithMany()
        //     .HasForeignKey(x => x.TenantId)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

## Quick Reference: Entity List

Based on db-design.md, here are all entities that need repositories:

### Core (13 entities)
1. ✅ Tenant
2. Site
3. User
4. Role
5. Group
6. UserRole
7. UserGroup
8. ContentNode
9. Route
10. ContentType
11. ContentTypeField
12. ContentItem
13. ContentVersion

### Remaining (25 entities)
14. ContentFieldValue
15. LayoutDefinition
16. ComponentDefinition
17. ContentLayout
18. AclEntry
19. PreviewToken
20. WorkflowDefinition
21. WorkflowState
22. WorkflowTransition
23. AuditLog
24. Module
25. ModuleCapability
26. ModuleSetting
27. ModuleMigration
28. EntityDefinition
29. EntityInstance
30. EntityRelationship
31. ProcessDefinition
32. ProcessInstance
33. Attachment
34. Comment
35. ActivityLog
36. SearchIndexEntry
37. ContentRagChunk
38. ContentEmbedding
39. ImageEmbedding
40. JobDefinition
41. JobExecution
42. JobExecutionHistory

---

## Multi-Tenancy Rules

**CRITICAL**: All entities (except Tenant itself) must:
1. Include `TenantId` field
2. Filter by `TenantId` in ALL queries
3. Have unique constraints scoped by `TenantId`
4. Use global query filters where appropriate

Example:
```csharp
// BAD - Missing tenant filter
var users = await Context.Set<UserRow>().ToListAsync();

// GOOD - Always filter by tenant
var users = await Context.Set<UserRow>()
    .Where(x => x.TenantId == tenantId)
    .ToListAsync();
```

---

## UnitOfWork Usage in Use-Cases

```csharp
public class CreateUserUseCase
{
    private readonly IUserRepository _userRepo;
    private readonly IUserRoleRepository _userRoleRepo;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<Guid>> ExecuteAsync(CreateUserCommand cmd)
    {
        // 1. Validate
        if (await _userRepo.EmailExistsAsync(cmd.TenantId, cmd.Email))
            return Result.Failure<Guid>("Email already exists");

        // 2. Create domain entities
        var user = User.Create(cmd.TenantId, cmd.Email, cmd.DisplayName);
        var userRole = UserRole.Create(cmd.TenantId, user.Id, cmd.RoleId);

        // 3. Add to repositories (NO SaveChanges called here)
        await _userRepo.AddAsync(user);
        await _userRoleRepo.AddAsync(userRole);

        // 4. Commit via UnitOfWork
        await _unitOfWork.SaveChangesAsync();

        return Result.Success(user.Id);
    }
}
```

---

## Testing Repositories

```csharp
public class UserRepositoryTests
{
    private readonly ContentOsDbContext _context;
    private readonly UserRepository _repository;

    [Fact]
    public async Task GetByEmailAsync_ReturnsUser_WhenExists()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var user = User.Create(tenantId, "test@example.com", "Test User");
        await _repository.AddAsync(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(tenantId, "test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }
}
```

---

## Summary

This guide provides templates for all components needed to implement the 38+ repositories. Follow these patterns for consistency, multi-tenancy compliance, and UnitOfWork integration.

**Key Points:**
- Repositories do NOT call SaveChanges
- Use-cases control transactions via UnitOfWork
- Always filter by TenantId
- Inherit from EfCoreBaseRepository for common operations
- Add specific methods to repository interfaces as needed
