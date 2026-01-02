# TechWayFit.ContentOS.Infrastructure.Persistence

This project contains **all database-specific infrastructure** for ContentOS.
It is the **only** place where EF Core, DbContext, migrations, and **DB entities (EF row models)** are allowed.

## Core Rules (must follow)

### ✅ Domain Entities vs DB Entities
- **Domain entities** live in feature projects (e.g., `TechWayFit.ContentOS.Content/Domain`) and represent business concepts.
- **DB entities** live here (Infrastructure) and represent persistence shape (tables/columns).
- Domain entities **must never** contain:
  - EF attributes
  - `DbContext`
  - `IQueryable`
  - navigation properties designed for EF convenience

### ✅ Mapping location
All mapping between DB entities and Domain entities must be implemented **inside Infrastructure.Persistence**.
Feature projects should depend only on **Ports/Interfaces** (e.g., `IContentRepository`), never on EF.

### ✅ Dependency boundaries
Allowed references:
- Infrastructure.Persistence → Abstractions + Kernel + Feature projects (Ports/Domain models only)

Not allowed:
- Feature projects → Infrastructure.Persistence
- Api → direct EF code except via registration + composition root

## Folder Structure (recommended)

- `Db/`
  - `ContentOSDbContext.cs`
  - `Configurations/` (EF Fluent configs)
  - `Migrations/`
- `Entities/` (DB models)
  - `ContentItemRow.cs`
  - `ContentVersionRow.cs`
  - `WorkflowInstanceRow.cs`
- `Repositories/` (implements ports)
  - `ContentRepository.cs`
  - `WorkflowRepository.cs`
- `Mapping/` (domain <-> row mapping)
  - `ContentItemMapper.cs`
  - `WorkflowMapper.cs`
- `Extensions/`
  - `ServiceCollectionExtensions.cs` (AddPersistence)

## Naming Convention

- DB entities must end with `Row` (or `Db`), e.g.:
  - `ContentItemRow`, `MediaAssetRow`
- Domain entities should **not** use `Row` suffix.

## Implementation Pattern (required)

### 1) Define Ports in Feature Project
Example (in `TechWayFit.ContentOS.Content`):
- `Ports/IContentRepository.cs`
- `Ports/IContentUnitOfWork.cs` (or use `IUnitOfWork` from Abstractions)

### 2) Implement Ports here
Repositories in this project implement those ports:
- `ContentRepository : IContentRepository`

### 3) Map DB <-> Domain here
Example skeleton:

```csharp
// DB entity
public sealed class ContentItemRow
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public int Status { get; set; }
}

// Mapper (Infrastructure only)
public static class ContentItemMapper
{
    public static ContentItem ToDomain(ContentItemRow row) => ContentItem.Rehydrate(
        id: new ContentId(row.Id),
        tenantId: new TenantId(row.TenantId),
        title: row.Title,
        slug: row.Slug,
        status: (ContentStatus)row.Status
    );

    public static ContentItemRow ToRow(ContentItem domain) => new()
    {
        Id = domain.Id.Value,
        TenantId = domain.TenantId.Value,
        Title = domain.Title.Value,
        Slug = domain.Slug.Value,
        Status = (int)domain.Status
    };
}