# TechWayFit.ContentOS.Infrastructure.Persistence.Postgres

## Purpose
PostgreSQL-specific implementation of persistence layer.

## Responsibilities
- **PostgreSQL DbContext**: Extends base ContentOsDbContext
- **PostgreSQL-specific configurations**: Optimizations and extensions
- **PostgreSQL migrations**: Database schema migrations for PostgreSQL
- **Provider registration**: PostgreSQL-specific DI setup

## Key Principles
- **Extends base**: Builds on top of Infrastructure.Persistence
- **PostgreSQL optimizations**: Uses PostgreSQL-specific features
- **Production-ready**: Enterprise-grade database support
- **Multi-tenant**: Tenant isolation with PostgreSQL schemas or filters

## Structure
```
PostgresDbContext.cs           # PostgreSQL-specific DbContext
DependencyInjection.cs         # PostgreSQL registration
Configurations/                # PostgreSQL-specific EF configurations
Migrations/                    # PostgreSQL migrations
```

## Usage
In API/Program.cs:
```csharp
// Use PostgreSQL instead of SQLite
services.AddPostgresPersistence(configuration);
```

## Configuration
```json
{
  "ConnectionStrings": {
    "PostgreSQL": "Host=localhost;Database=contentos;Username=postgres;Password=..."
  }
}
```

## Features
- Full-text search (PostgreSQL native)
- JSON/JSONB column support
- Array types
- Advanced indexing
- Partitioning support

## Dependencies
- Infrastructure.Persistence (base)
- Npgsql.EntityFrameworkCore.PostgreSQL
