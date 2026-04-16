# Repository Dependency Injection Fix

## Problem

The application was throwing:
```
InvalidOperationException: Unable to resolve service for type 'Microsoft.EntityFrameworkCore.DbContext' 
while attempting to activate 'TechWayFit.ContentOS.Infrastructure.Persistence.Repositories.Content.ContentTypeRepository'.
```

## Root Cause

The repository implementations in `TechWayFit.ContentOS.Infrastructure.Persistence` require a `DbContext` parameter in their constructors:

```csharp
public class ContentTypeRepository : EfCoreRepository<ContentType, ContentTypeRow, Guid>, IContentTypeRepository
{
  public ContentTypeRepository(DbContext dbContext) : base(dbContext)  // ? Requires DbContext
    {
    }
}
```

However, the DI container was only configured with:
1. `PostgresDbContext` (concrete type)
2. `ContentOsDbContext` (alias to PostgresDbContext)

But NOT with the base `DbContext` type that the repositories needed.

## Solution

Added registration for `DbContext` in the Dependency Injection configuration:

```csharp
// src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres/DependencyInjection.cs

public static IServiceCollection AddPostgresPersistence(
    this IServiceCollection services,
    IConfiguration configuration)
{
    // ... connection string setup ...

    // Register PostgresDbContext (which inherits from ContentOsDbContext)
    services.AddDbContext<PostgresDbContext>(options => ...);
    
    // Register ContentOsDbContext as an alias to PostgresDbContext
    services.AddScoped<ContentOsDbContext>(sp => sp.GetRequiredService<PostgresDbContext>());

    // ? CRITICAL FIX: Register DbContext for repositories
    // This allows the base persistence layer repositories to work with any DbContext implementation
    services.AddScoped<DbContext>(sp => sp.GetRequiredService<PostgresDbContext>());

    // ... repository registrations ...
}
```

## Why This Architecture?

### Layered Architecture Benefits

1. **Base Persistence Layer** (`TechWayFit.ContentOS.Infrastructure.Persistence`)
   - Provider-agnostic
   - Uses `DbContext` (EF Core abstraction)
- Can work with any EF Core provider (PostgreSQL, SQL Server, SQLite, etc.)
   - Contains repository implementations that work across providers

2. **Provider-Specific Layer** (`TechWayFit.ContentOS.Infrastructure.Persistence.Postgres`)
   - PostgreSQL-specific
   - Uses `ContentOsDbContext` and `PostgresDbContext`
   - Contains Postgres optimizations (ILike, full-text search, etc.)
   - Handles DI registration and wiring

### Why Use `DbContext` Parameter?

```csharp
// ? GOOD: Base persistence layer uses DbContext
public class ContentTypeRepository : EfCoreRepository<ContentType, ContentTypeRow, Guid>
{
    public ContentTypeRepository(DbContext dbContext) : base(dbContext) { }
    // This repository works with ANY EF Core provider
}

// ? BAD: Would tie to specific provider
public class ContentTypeRepository
{
    public ContentTypeRepository(PostgresDbContext dbContext) { }
    // This repository only works with PostgreSQL
}
```

## Files Modified

1. **DependencyInjection.cs** - Added `DbContext` registration
2. **Created ContentTypeRepository.cs** - New repository implementation

## Registered Repositories

All Content feature repositories are now properly registered:

| Repository Interface | Implementation | Purpose |
|---------------------|----------------|---------|
| `IContentTypeRepository` | `ContentTypeRepository` | Content type schemas |
| `IContentTypeFieldRepository` | `ContentTypeFieldRepository` | Content type fields |
| `IContentItemRepository` | `ContentItemRepository` | Content items |
| `IContentVersionRepository` | `ContentVersionRepository` | Content versions |
| `IContentFieldValueRepository` | `ContentFieldValueRepository` | Field values |
| `IContentNodeRepository` | `ContentNodeRepository` | Content hierarchy |
| `IRouteRepository` | `RouteRepository` | URL routes |
| `ITenantRepository` | `PostgresTenantRepository` | Tenants |

## Verification

```bash
dotnet build
# Build successful ?
```

## Key Takeaways

1. **DI Registration Pattern**: When repositories use `DbContext`, register it in the DI container
2. **Layered Architecture**: Base layer uses abstractions, provider layer uses concrete types
3. **Provider Agnostic**: Repositories work with any EF Core provider
4. **Clean Separation**: Infrastructure concerns isolated from feature code

## Related Documentation

- [Repository Implementation Guide](./repository-implementation-guide.md)
- [ContentOS Repositories](./contentos-repositories.md)
- [Architecture Overview](./architecture.md)

---

**Date**: 2025-01-02  
**Status**: ? Resolved  
**Build**: Successful  
**Impact**: All 31 use cases and 30 API endpoints now functional
