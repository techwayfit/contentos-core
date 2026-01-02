# ContentOS Architecture & Boundary Rules

> **This repository follows a strict layered architecture.  
These rules exist to prevent architectural drift and must be followed by humans and Copilot alike.**

---

## 1. High-level layering

┌──────────────────────────┐
│ API (HTTP) │
│ TechWayFit.ContentOS.Api│
└───────────▲──────────────┘
│ calls
┌───────────┴──────────────┐
│ Application / Domain │
│ Content / Media / Search │
│ Workflow (Business Logic)│
└───────────▲──────────────┘
│ ports
┌───────────┴──────────────┐
│ Kernel │
│ Runtime, Policies, Events │
└───────────▲──────────────┘
│ abstractions
┌───────────┴──────────────┐
│ Abstractions │
│ Ports, Capabilities │
└───────────▲──────────────┘
│ implements
┌───────────┴──────────────┐
│ Infrastructure │
│ EF / Lucene / Blob Store │
└──────────────────────────┘

---

## 2. Project responsibilities

### TechWayFit.ContentOS.Api
**Transport + composition root only**

Allowed:
- HTTP endpoints
- Authentication (JWT)
- Authorization (policies, permissions)
- Request validation
- DTO ↔ command/query mapping
- Dependency Injection wiring via extension methods

Not allowed:
- EF Core usage
- DbContext
- Database entities
- Business logic
- Repository implementations

---

### Feature projects  
`Content`, `Media`, `Search`, `Workflow`

These projects contain **business logic and domain models**.

Allowed:
- Domain entities (business meaning)
- Use-cases / application services
- Business rules
- Ports (interfaces) for persistence, storage, search
- Domain events

Not allowed:
- EF attributes
- DbContext
- SQL
- Lucene APIs
- File system access

---

### Infrastructure projects  
`TechWayFit.ContentOS.Infrastructure.*`

These projects contain **technology-specific implementations**.

Allowed:
- EF Core
- DbContext
- DB entities (Row models)
- Migrations
- Lucene adapters
- Blob storage implementations
- Mapping between DB ↔ Domain entities

Not allowed:
- Business rules
- HTTP concerns

---

### TechWayFit.ContentOS.Kernel
**Platform runtime**

Contains:
- Permissions and policies
- Eventing
- Module runtime
- Cross-cutting concerns (tenancy, auditing)

No infrastructure dependencies allowed.

---

### TechWayFit.ContentOS.Abstractions
**Pure contracts**

Contains:
- Ports and interfaces
- Capability definitions
- Shared primitives

No implementation, no EF, no HTTP.

---

## 3. Domain Entity vs DB Entity (critical rule)

### Domain Entity
- Lives in feature projects
- Represents business concepts
- Uses value objects
- No persistence concerns

Example:
Content/Domain/ContentItem.cs

### DB Entity (EF Row Model)
- Lives in Infrastructure.Persistence
- Represents table structure
- Optimized for storage
- Must end with `Row`

Example:
Infrastructure.Persistence/Entities/ContentItemRow.cs

### Mapping rule
**All mapping between Domain ↔ DB entities must happen in Infrastructure projects.**

Feature projects must never depend on database shapes.

---

## 4. Repository rule (Ports & Adapters)

- Feature projects define repository **ports** (interfaces)
- Infrastructure projects **implement** those ports
- API wires implementations via DI

Feature → IContentRepository
Infrastructure → ContentRepository : IContentRepository
API → services.AddContentOSPersistence()

---

## 5. Dependency rules

### Allowed references
- Api → Kernel, Abstractions, Contracts, Features, Infrastructure (registration only)
- Features → Kernel, Abstractions
- Infrastructure → Kernel, Abstractions, Features (Ports only)
- Kernel → Abstractions

### Forbidden references
- Feature → Infrastructure
- Domain → EF / DbContext / SQL
- Api → DbContext / DB entities
- Contracts → Domain

---

## 6. Copilot guidance

When generating code:

- Always create **separate Domain and DB entities**
- Always place EF entities in Infrastructure with `Row` suffix
- Always place mapping logic in Infrastructure
- Never add EF or DB code to Api or Feature projects
- Api endpoints must call **use-cases**, not repositories

If Copilot suggests mixing concerns, reject and refactor.

---

## 7. Architectural intent

This structure ensures:
- Clear separation of concerns
- Replaceable infrastructure
- Testable business logic
- Long-term scalability from CMS to platform

**Do not trade architectural clarity for short-term convenience.**