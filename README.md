# TechWayFit ContentOS Core

Enterprise-grade, headless-first content management platform built on .NET.

## Overview

ContentOS Core is a modular, API-first CMS platform designed for multi-tenant, multi-site, and multi-environment deployments. The platform provides core content management capabilities with optional AI assistance, event-driven architecture, and role-based access control.

## Platform Principles

- **Headless & API-First**: All functionality exposed via REST APIs, no built-in UI
- **Multi-Tenant Foundation**: Native support for tenant → site → environment hierarchy
- **Deterministic Baseline**: Core features work without AI dependencies
- **AI-Optional**: AI capabilities as an assistance layer, never a requirement
- **Event-Driven**: Domain events enable loose coupling and extensibility
- **No Business Logic**: Platform provides infrastructure, not domain-specific logic
- **No Licensing/Billing**: Focus on core CMS capabilities only

## Architecture & Boundaries

## 📚 Documentation

- 🏗️ [Architecture Overview](docs/architecture.md)
- 📐 [Module & Boundary Rules](docs/module-boundaries.md)
- 🧭 [Extension Models](docs/extension-model.md)


## Architecture & Boundaries

- 📐 [Architecture & Boundary Rules](ARCHITECTURE.md)


### Project Structure

```
contentos-core/
├── docs/                                     # Architecture Decision Records
├── src/
│   ├── TechWayFit.ContentOS.Abstractions/    # Core interfaces and contracts
│   ├── TechWayFit.ContentOS.Contracts/       # DTOs, schemas, domain events
│   ├── TechWayFit.ContentOS.Kernel/          # Tenant, RBAC, event bus
│   ├── TechWayFit.ContentOS.Content/         # Content schema and CRUD
│   ├── TechWayFit.ContentOS.Workflow/        # Draft/review/publish lifecycle
│   ├── TechWayFit.ContentOS.Media/           # Digital asset management
│   ├── TechWayFit.ContentOS.Search/          # Keyword-based search
│   ├── TechWayFit.ContentOS.AI/              # Optional AI orchestration
│   └── TechWayFit.ContentOS.Api/             # REST API gateway
└── tests/
    └── [Corresponding test projects]
```

### Module Responsibilities

#### Abstractions
Core interfaces and capability contracts that define platform abstractions:
- `ITenantContext`: Multi-tenant context
- `IRepository<TEntity, TKey>`: Data access abstraction
- `IEventBus`: Event publishing and subscription
- `IAuthorizationPolicy`: RBAC enforcement

#### Contracts
Data transfer objects, schemas, and domain events:
- DTOs for API contracts
- Domain event definitions
- Tenant and site configuration models

#### Kernel
Platform foundation services:
- **Tenancy**: Tenant/site/environment resolution and isolation
- **Security**: Role-based access control (RBAC)
- **Events**: In-memory event bus for domain events

#### Content
Content management core:
- Content schema definition and validation
- CRUD operations for content items
- Content type management

#### Workflow
Content lifecycle management:
- Draft → Review → Publish workflow
- State transition management
- Workflow status tracking

#### Media
Digital asset management (DAM-lite):
- File upload and storage
- Asset metadata management
- Download and retrieval

#### Search
Keyword-based content discovery:
- Content indexing
- Keyword search
- Index management

#### AI
Optional AI orchestration (provider-agnostic):
- AI provider abstraction
- Content generation assistance
- Content analysis capabilities
- **Note**: AI is optional and should not be required for core functionality

#### Api
REST API gateway:
- HTTP endpoints
- Request/response handling
- API versioning support

## Platform Boundaries

### In Scope
- Multi-tenant infrastructure
- Content schema and CRUD
- Workflow state management
- Digital asset management
- Event-driven architecture
- Role-based access control
- Keyword search capabilities
- Optional AI orchestration

### Out of Scope
- User interface (UI)
- Domain-specific business logic
- Licensing and billing systems
- Specific AI provider implementations
- Frontend frameworks
- Authentication providers (platform provides RBAC, not auth)

## Getting Started

### Prerequisites
- .NET 9.0 SDK or later
- A code editor (Visual Studio, VS Code, Rider)

### Building the Solution

```bash
# Restore dependencies
dotnet restore

# Build all projects
dotnet build

# Run tests
dotnet test
```

### Running the API

```bash
cd src/TechWayFit.ContentOS.Api
dotnet run
```

The API will be available at `https://localhost:5001` (or the port specified in launch settings).

## Architecture Decisions

See [docs/adr](docs/adr) for Architecture Decision Records documenting key design decisions.

## Start here (for humans + Copilot)
- **Copilot context:** `COPILOT_CONTEXT.md`
- **Core boundaries:** `src/ContentOS.Abstractions/README.md`
- **Composition + hosting:** `src/ContentOS.Core/README.md`
- **HTTP API rules:** `src/ContentOS.Api/README.md`

> Architecture decisions (ADRs) live in `contentos-docs/adr/ADR-000-Index.md`.


## Contributing

This is a scaffold project. Contributions should focus on:
- Implementing core platform capabilities
- Adding comprehensive tests
- Improving documentation
- Maintaining clear separation of concerns

## License

See [LICENSE](LICENSE) file for details.
