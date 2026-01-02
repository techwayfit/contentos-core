# TechWayFit.ContentOS.Api

This project is the **HTTP host** for ContentOS. It is a **thin transport layer**:
- defines endpoints (HTTP)
- handles authentication/authorization
- validates requests
- maps DTOs <-> domain models
- orchestrates use-cases

## Core Rule: API must remain thin

### ✅ Allowed in Api
- Controllers / Minimal APIs / Endpoints
- Request/response DTOs (from `TechWayFit.ContentOS.Contracts`)
- Authentication (JWT bearer)
- Authorization policies (permission-based)
- Validation (e.g., FluentValidation)
- Middleware (exception handling, correlation IDs)
- DI composition root (wiring services via extension methods)
- Mapping DTO -> command/query objects

### ❌ Not allowed in Api
- EF Core queries, DbContext usage
- DB entities (Row models)
- Lucene/OpenSearch direct calls
- Filesystem/S3 direct calls
- Business rules & domain logic
- Repository implementations

Api should never contain `DbSet<>`, entity configurations, or LINQ-to-Entities queries.
Those belong in Infrastructure projects.

## Dependency boundaries

Allowed references:
- Api → Kernel + Abstractions + Contracts + Feature projects + Infrastructure.* (only for registration)
- Api should reference Infrastructure projects only to call `services.AddXyz(...)`.

Not allowed:
- Api calling Infrastructure internal types like `ContentItemRow` or `DbContext` directly.

## Composition Root (required pattern)

Api registers infrastructure through extension methods:

- `services.AddContentOSPersistence(config);`
- `services.AddContentOSSearch(config);`
- `services.AddContentOSStorage(config);`

Endpoints then depend on **ports/use-cases** from feature projects.

## Endpoint pattern (recommended)

- Keep endpoint handlers very small:
  - validate input
  - authorize
  - call use-case
  - return DTO

Example skeleton:

```csharp
app.MapPost("/api/content", async (
    CreateContentRequest request,
    ICreateContentUseCase useCase,
    IPolicyEvaluator policy,
    CancellationToken ct) =>
{
    await policy.RequireAsync(Permissions.ContentCreate, ct);

    var result = await useCase.ExecuteAsync(request.ToCommand(), ct);
    return result.Match(
        ok => Results.Created($"/api/content/{ok.Id}", ok.ToResponse()),
        err => Results.Problem(err.ToProblemDetails())
    );
});
