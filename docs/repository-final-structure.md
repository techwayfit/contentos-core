# Repository Architecture - Final Structure

## ✅ Corrected Implementation

Successfully reorganized repository architecture to properly separate **provider-agnostic** code from **provider-specific** implementations.

---

## Architecture Layers

```
┌────────────────────────────────────────────────────────────────┐
│  Persistence (Provider-Agnostic)                               │
│  ├── Entities/                                                 │
│  │   ├── Core/                 (Tenant, Site, User, etc.)     │
│  │   ├── Content/              (ContentItem, ContentType...)   │
│  │   ├── Security/             (AclEntry...)                   │
│  │   └── [Other modules]                                       │
│  ├── Repositories/                                             │
│  │   ├── EfCoreRepository.cs   (Base - uses DbContext)        │
│  │   ├── Core/                                                 │
│  │   │   └── TenantRepository.cs                              │
│  │   ├── Content/                                              │
│  │   └── [Other modules]                                       │
└────────────────────────────────────────────────────────────────┘
                            ↓
┌────────────────────────────────────────────────────────────────┐
│  Persistence.Postgres (Provider-Specific)                      │
│  ├── Repositories/                                             │
│  │   └── PostgresTenantRepository.cs  (Postgres optimizations)│
│  ├── Configurations/        (EF Fluent API configs)           │
│  └── DependencyInjection.cs (Registers PostgresTenantRepo)    │
└────────────────────────────────────────────────────────────────┘
```

---

## Directory Structure

```
src/infrastructure/persistence/
├── TechWayFit.ContentOS.Infrastructure.Persistence/
│   ├── Entities/
│   │   ├── Core/
│   │   │   ├── TenantRow.cs ✅
│   │   │   ├── SiteRow.cs
│   │   │   ├── UserRow.cs
│   │   │   ├── RoleRow.cs
│   │   │   └── GroupRow.cs
│   │   ├── Content/
│   │   │   ├── ContentItemRow.cs
│   │   │   ├── ContentTypeRow.cs
│   │   │   ├── ContentNodeRow.cs
│   │   │   └── RouteRow.cs
│   │   ├── Security/
│   │   │   └── AclEntryRow.cs
│   │   ├── Workflow/
│   │   ├── AI/
│   │   ├── Jobs/
│   │   └── [Other modules]/
│   │
│   ├── Repositories/
│   │   ├── EfCoreRepository.cs ✅ (Base - uses DbContext)
│   │   ├── Core/
│   │   │   └── TenantRepository.cs ✅
│   │   ├── Content/
│   │   ├── Security/
│   │   └── [Other modules]/
│   │
│   └── TechWayFit.ContentOS.Infrastructure.Persistence.csproj
│       (References: EF Core, Abstractions, Tenancy feature)
│
└── TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/
    ├── Repositories/
    │   └── PostgresTenantRepository.cs ✅ (Overrides with Postgres features)
    ├── Configurations/
    ├── Migrations/
    └── DependencyInjection.cs ✅
```

---

## Key Design Principles

### 1. **Provider-Agnostic Layer (Persistence)**
- **Location**: `TechWayFit.ContentOS.Infrastructure.Persistence`
- **Contains**:
  - Row entities (database schema representation)
  - Base repository implementations using standard EF Core
  - Uses `DbContext` (not provider-specific context)
  
**Example**:
```csharp
// Entities/Core/TenantRow.cs
namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
public sealed class TenantRow { ... }

// Repositories/Core/TenantRepository.cs
namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Core;
public class TenantRepository : EfCoreRepository<Tenant, TenantRow, Guid>, ITenantRepository
{
    public TenantRepository(DbContext context) : base(context) { }
    // Standard EF Core queries
}
```

### 2. **Provider-Specific Layer (Persistence.Postgres)**
- **Location**: `TechWayFit.ContentOS.Infrastructure.Persistence.Postgres`
- **Contains**:
  - Postgres-specific repository implementations (optional)
  - EF Core configurations
  - Migrations
  - DI registration
  
**Example**:
```csharp
// Repositories/PostgresTenantRepository.cs
namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Repositories;
public sealed class PostgresTenantRepository : TenantRepository
{
    public PostgresTenantRepository(DbContext context) : base(context) { }
    
    // Override with Postgres-specific optimizations
    public override async Task<Tenant?> GetByKeyAsync(string key, ...)
    {
        // Use Postgres ILIKE for case-insensitive search
        var row = await Context.Set<TenantRow>()
            .FirstOrDefaultAsync(t => EF.Functions.ILike(t.Key, key), ...);
        return row == null ? null : MapToDomain(row);
    }
}
```

### 3. **Folder Organization Matches Entities**
- Both `Entities/` and `Repositories/` follow the same module structure
- Makes it easy to find related code
- Clear organization by domain/feature

**Modules**:
- `Core/` - Tenant, Site, User, Role, Group
- `Content/` - ContentItem, ContentType, ContentNode
- `Security/` - AclEntry
- `Workflow/` - WorkflowDefinition, WorkflowState
- `AI/`, `Jobs/`, `Modules/`, etc.

---

## Migration Path

### For Each Entity (e.g., User, Site, ContentItem):

#### Step 1: Create Row Entity (if not exists)
```
Persistence/Entities/{Module}/{Entity}Row.cs
```

#### Step 2: Create Provider-Agnostic Repository
```
Persistence/Repositories/{Module}/{Entity}Repository.cs
```
- Inherits from `EfCoreRepository<TEntity, TEntityRow, TKey>`
- Implements `I{Entity}Repository`
- Uses standard EF Core queries
- Constructor takes `DbContext`

#### Step 3: (Optional) Create Provider-Specific Override
```
Persistence.Postgres/Repositories/Postgres{Entity}Repository.cs
```
- Inherits from `{Entity}Repository`
- Overrides methods with Postgres-specific optimizations
- Uses Postgres functions: `ILike`, `ToTsVector`, etc.

#### Step 4: Register in DI
```csharp
// Persistence.Postgres/DependencyInjection.cs
services.AddScoped<I{Entity}Repository, Postgres{Entity}Repository>();
```

---

## Benefits of This Structure

### ✅ **True Provider Agnosticism**
- Base repositories work with any EF Core provider (PostgreSQL, SQL Server, SQLite)
- Can swap databases by changing DI registrations
- No Postgres-specific code in base layer

### ✅ **Performance Optimization**
- Provider-specific repositories can use database-specific features
- Example: Postgres `ILIKE` vs SQL Server `COLLATE`
- Full-text search varies by provider

### ✅ **Clear Separation of Concerns**
```
Persistence:         Standard EF Core (works everywhere)
Persistence.Postgres: Postgres optimizations (optional)
```

### ✅ **Easy to Find Code**
```
Need ContentItemRow?     → Entities/Content/ContentItemRow.cs
Need ContentRepository?  → Repositories/Content/ContentItemRepository.cs
```

### ✅ **Scalable Pattern**
- Add new modules easily (e.g., `Entities/Media/`, `Repositories/Media/`)
- Follows existing entity organization
- No confusion about where files belong

---

## Build Verification

✅ **Persistence** - Provider-agnostic layer builds successfully  
✅ **Persistence.Postgres** - Postgres-specific layer builds successfully  
✅ **Tenancy Feature** - Feature layer builds successfully  
✅ **No compilation errors**

---

## Example: Complete Tenant Flow

### 1. Domain Entity (Feature Layer)
```csharp
// features/tenancy/Domain/Tenant.cs
public sealed class Tenant { ... }
```

### 2. Repository Interface (Feature Layer)
```csharp
// features/tenancy/Ports/ITenantRepository.cs
public interface ITenantRepository : IRepository<Tenant, Guid> { ... }
```

### 3. Row Entity (Persistence - Provider Agnostic)
```csharp
// Persistence/Entities/Core/TenantRow.cs
namespace TechWayFit.ContentOS.Infrastructure.Persistence.Entities.Core;
public sealed class TenantRow { ... }
```

### 4. Base Repository (Persistence - Provider Agnostic)
```csharp
// Persistence/Repositories/Core/TenantRepository.cs
namespace TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Core;
public class TenantRepository : EfCoreRepository<Tenant, TenantRow, Guid>, ITenantRepository
{
    public TenantRepository(DbContext context) : base(context) { }
    // Standard EF Core implementation
}
```

### 5. Postgres Override (Persistence.Postgres - Provider Specific)
```csharp
// Persistence.Postgres/Repositories/PostgresTenantRepository.cs
namespace TechWayFit.ContentOS.Infrastructure.Persistence.Postgres.Repositories;
public sealed class PostgresTenantRepository : TenantRepository
{
    public PostgresTenantRepository(DbContext context) : base(context) { }
    // Postgres-specific optimizations (ILIKE, full-text search, etc.)
}
```

### 6. DI Registration (Persistence.Postgres)
```csharp
services.AddScoped<ITenantRepository, PostgresTenantRepository>();
```

---

## Next Steps

Ready to implement remaining 37 repositories following this structure:

1. Create `{Entity}Row` in `Persistence/Entities/{Module}/`
2. Create `{Entity}Repository` in `Persistence/Repositories/{Module}/`
3. (Optional) Create `Postgres{Entity}Repository` in `Persistence.Postgres/Repositories/`
4. Register in `Persistence.Postgres/DependencyInjection.cs`

**The foundation is clean, scalable, and follows architectural best practices.** 🎯
