# ContentOS Core - Infrastructure Projects

## Overview
The ContentOS Core solution now includes proper separation of concerns through Infrastructure projects that implement concrete versions of abstractions defined in the core layers.

## Infrastructure Projects

### 1. TechWayFit.ContentOS.Infrastructure.Persistence
**Purpose**: Data access and persistence layer using Entity Framework Core

**Contains**:
- `ContentOsDbContext` - Main database context
- `Entities/` - EF database entities (rows) - **NOT** domain entities
- `Configurations/` - EF entity configurations
- `Repositories/` - Repository implementations
- `Migrations/` - EF Core migrations

**Key Principle**: Domain entities ≠ DB entities. Mapping happens in this layer.

**Usage in API**:
```csharp
services.AddPersistence(configuration);
```

---

### 2. TechWayFit.ContentOS.Infrastructure.Storage
**Purpose**: Blob/file storage implementations

**Contains**:
- `Providers/LocalFileSystemBlobStore` - Local filesystem storage
- `Providers/AzureBlobStore` - Azure Blob Storage integration
- `Providers/S3BlobStore` - AWS S3 integration

**Configuration-driven**: Choose provider via appsettings

**Usage in API**:
```csharp
services.AddBlobStorage(configuration);
```

---

### 3. TechWayFit.ContentOS.Infrastructure.Search
**Purpose**: Search indexing and querying implementations

**Contains**:
- `Providers/LuceneSearchIndex` - Lucene.NET local search
- `Providers/AzureSearchIndex` - Azure Cognitive Search
- `Models/` - Search-specific models and DTOs

**Configuration-driven**: Choose search provider via appsettings

**Usage in API**:
```csharp
services.AddSearch(configuration);
```

---

### 4. TechWayFit.ContentOS.Infrastructure.Identity
**Purpose**: Authentication and authorization implementations

**Contains**:
- `Services/` - JWT validation, claims transformation, token generation
- `Providers/` - OIDC, JWT, API Key authentication providers

**Supports**: OAuth 2.0, OpenID Connect, JWT Bearer

**Usage in API**:
```csharp
services.AddIdentity(configuration);
```

---

## Feature Project Structure

Each feature project (Content, Media, Search, Workflow) now follows clean architecture:

```
TechWayFit.ContentOS.[Feature]/
├── Domain/           # Business entities (NOT EF entities)
├── Application/      # Use cases, handlers, services
└── Ports/           # Interface contracts for infrastructure
```

### Separation Benefits:
- ✅ Domain stays persistence-ignorant
- ✅ Business logic independent of EF/Lucene/etc
- ✅ Easy to swap infrastructure providers
- ✅ Testable without database/external services
- ✅ Clear boundaries and dependencies

---

## Dependency Flow

```
API (Host)
  ↓ references
Infrastructure.* Projects
  ↓ implements
Abstractions (Interfaces)
  ↑ used by
Feature Projects (Content/Media/Search/Workflow)
  ↓ references
Kernel (Core runtime)
```

---

## Configuration Examples

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=contentos.db"
  },
  "BlobStorage": {
    "Provider": "LocalFileSystem",
    "LocalFileSystem": {
      "RootPath": "./storage"
    }
  },
  "Search": {
    "Provider": "Lucene",
    "Lucene": {
      "IndexPath": "./indexes"
    }
  },
  "Authentication": {
    "Provider": "JWT",
    "JWT": {
      "Issuer": "https://contentos.example.com",
      "Audience": "contentos-api",
      "SecretKey": "your-secret-key-here"
    }
  }
}
```

---

## API/Program.cs Integration

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add infrastructure services
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddBlobStorage(builder.Configuration);
builder.Services.AddSearch(builder.Configuration);
builder.Services.AddIdentity(builder.Configuration);

// Add feature services
builder.Services.AddContent();
builder.Services.AddMedia();
builder.Services.AddWorkflow();

var app = builder.Build();
```

---

## Next Steps

1. **Implement Repository Interfaces**: Create concrete repositories in `Infrastructure.Persistence/Repositories/`
2. **Define Domain Entities**: Add business entities in feature projects' `Domain/` folders
3. **Create EF Entities**: Add database models in `Infrastructure.Persistence/Entities/`
4. **Add Migrations**: Run `dotnet ef migrations add Initial` in Persistence project
5. **Implement Storage Providers**: Complete blob store implementations
6. **Implement Search Providers**: Complete search index implementations

---

## Architecture Enforcement

### Rules:
- ❌ Domain projects NEVER reference EF, Lucene, Azure SDKs
- ❌ Infrastructure projects NEVER contain business logic
- ✅ API only orchestrates - calls use cases/handlers
- ✅ All infrastructure binding happens in API host via DI
- ✅ Domain entities stay separate from DB entities
