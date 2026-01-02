# Module Boundaries – TechWayFit ContentOS

This document defines **strict architectural boundaries** for each project in TechWayFit ContentOS.
If a change violates these rules, it is an **architecture defect** and must be corrected or justified via ADR.

Mental model:
- **Kernel** → what the platform *is* (core truth)
- **Domain Bounded Contexts** → domain meaning per area (Content, Media, Tenancy, Search, Workflow)
- **Contracts** → how the system *talks* (DTOs/events/extension surfaces)
- **Abstractions** → what the system *needs* (interfaces)
- **AI** → optional augmentation layer (never owns domain truth)
- **Infrastructure** → implementations (DB, identity, search engine, storage, events)
- **API** → transport + composition root

---

## 1) TechWayFit.ContentOS.Kernel
### Purpose
Core cross-domain primitives and platform-wide truth.

### Contains
- Shared primitives/value objects: `TenantId`, `UserId`, `ModuleId`, `EntityId`, `Slug`, `VersionNumber`
- Base domain types: `Entity`, `AggregateRoot`, `DomainEvent` (base type), `Result`, `Error`
- Platform-wide exceptions only (truly cross-domain)

### Must NOT contain
- DTOs, controllers, persistence, EF, serialization attributes, logging
- Tenant resolver/context implementations
- Event bus implementations (or transport-specific logic)
- Repository/service interfaces (these go to Abstractions)

### Dependency rule
Kernel has **zero project dependencies**.

---

## 2) Domain Bounded Contexts (Domain projects)
These projects hold **domain meaning** (entities, invariants, lifecycle) for each area.
They may reference **Kernel**, but must not depend on Infrastructure or API.

### 2.1) TechWayFit.ContentOS.Content
**Purpose:** content semantics, schema, versioning, lifecycle.

**Contains**
- `ContentItem`, `ContentType`, `FieldDefinition`, `FieldValue`, `ContentSchema`, `ContentVersion`
- Invariants: publish rules, required fields, schema validation, version constraints
- Domain events (internal facts): `ContentPublished`, `ContentVersionAdded`

**Must NOT contain**
- API DTOs, controllers
- EF Core mappings/DbContext, SQL
- Search indexing implementations, AI generation, rendering pipelines

---

### 2.2) TechWayFit.ContentOS.Media
**Purpose:** media assets semantics (files/images/metadata) as domain intent.

**Contains**
- `MediaAsset`, `MediaFolder`, `RenditionRequest`, `MediaMetadata`
- Invariants: allowed types, lifecycle, ownership, tenant scoping
- Domain events: `MediaUploaded`, `RenditionRequested`

**Must NOT contain**
- Blob/S3/Azure SDK code
- Image processing implementations
- DTOs/controllers

---

### 2.3) TechWayFit.ContentOS.Tenancy
**Purpose:** tenant model, lifecycle, and tenant isolation rules.

**Contains**
- `Tenant`, `TenantStatus`, `TenantPlan`, `TenantSettings`
- Invariants: state transitions, uniqueness rules, tenant-scoped constraints
- Domain events: `TenantCreated`, `TenantSuspended`

**Must NOT contain**
- Tenant resolver/context implementations
- Superadmin API controllers
- Persistence implementations / EF

---

### 2.4) TechWayFit.ContentOS.Search
**Purpose:** search semantics (query model, index intent) as domain meaning.

**Contains**
- `SearchQuery`, `SearchFilter`, `IndexDefinition`, `SearchResult`
- Policies/intents: “what should be indexed”, indexing triggers (as intent)
- Domain events: `IndexingRequested`

**Must NOT contain**
- Lucene/OpenSearch implementations
- Index writers/readers/analyzers implementations
- DTOs/controllers

---

### 2.5) TechWayFit.ContentOS.Workflow
**Purpose:** workflow semantics (states, transitions, approvals, assignments).

**Contains**
- `WorkflowDefinition`, `WorkflowInstance`, `State`, `Transition`, `Approval`
- Invariants: valid transitions, role requirements (as domain rules)
- Domain events: `WorkflowStarted`, `StateChanged`

**Must NOT contain**
- UI concerns
- Persistence implementation / EF
- Controllers

---

## 3) TechWayFit.ContentOS.Contracts
### Purpose
Stable, versionable public surface for:
- API consumers
- modules (Forge)
- integration/eventing messages

### Contains
- API DTOs: `CreateContentRequest`, `ContentResponse`, `PublishContentRequest`
- Public/integration events/messages: `ContentPublishedEventV1`, `TenantCreatedEventV1`
- Module extension contracts: `IModule`, `IModuleManifest`, `IModuleContext`
- Policy contracts (interfaces meant to be implemented elsewhere): `IAuthorizationPolicy`, `ILicensingPolicy`

### Must NOT contain
- Domain rules/workflows/invariants
- EF entities/DbContexts/persistence models
- Controllers
- Infrastructure implementations

### Stability rule
Breaking changes require **version bump + ADR**.

---

## 4) TechWayFit.ContentOS.Abstractions
### Purpose
Dependency inversion layer: “what the system needs” as interfaces.

### Contains
- Repository interfaces: `IContentRepository`, `IMediaRepository`, `ITenantRepository`
- Cross-cutting interfaces: `IUnitOfWork`, `IClock`, `IAuditLogger`
- Tenant/user context interfaces (contracts only):
  - `ICurrentTenantProvider`
  - `ITenantResolver`
  - `ICurrentUserProvider`
- Eventing interfaces:
  - `IEventBus` / `IEventPublisher`
  - (optional) `IEventHandler<T>` if you define your own minimal handler contract
- Search/storage/AI provider interfaces (interfaces only):
  - `ISearchProvider`, `IStorageProvider`, `IAIProvider`

### Must NOT contain
- DTOs
- Domain logic
- Implementations (no EF, no SDK clients, no HTTP)

---

## 5) TechWayFit.ContentOS.AI
### Purpose
Provides **AI capabilities as an optional, policy-driven augmentation layer**.
AI augments the platform but **never owns domain truth** and **never mutates core state directly**.

### Contains
- AI services (implementations):
  - `ContentGenerationService`
  - `SEOAssistService`
  - `ImageMatchingService`
- Prompt construction + context assembly + response post-processing
- AI policy implementations:
  - cost/rate limiting
  - tenant AI enablement checks (consuming evaluated policy/flags)
  - safety filters and guardrails
- Provider adapters (implementation details):
  - OpenAI / Azure OpenAI / local LLM connectors
- Mapping between domain reads → AI inputs → AI outputs

### Must NOT contain
- Domain entities/invariants
- Publish/approve/workflow orchestration (must remain deterministic elsewhere)
- Persistence logic (EF/DbContext/SQL)
- Controllers/transport concerns
- Cross-tenant logic

### Dependency rules
- May reference: Kernel, Domain projects (read-only), Abstractions, Contracts
- Must NOT be referenced by: Kernel or Domain projects
- Must remain removable without breaking the platform

### AI boundary rule (critical)
AI may **suggest/enrich/generate/rank**.  
AI must **not** publish/approve/persist/mutate domain state directly.  
Violations require an ADR.

---

## 6) TechWayFit.ContentOS.Api
### Purpose
Transport layer + composition root.

### Contains
- Controllers/endpoints
- Middleware, auth wiring, DI composition
- API versioning, OpenAPI configuration
- Request/response shaping using **Contracts**
- Tenant resolution wiring (middleware) by calling Infrastructure implementations via Abstractions

### Must NOT contain
- Domain logic
- Persistence logic
- Business workflows/orchestration
- Direct DB calls

---

## 7) Infrastructure Projects
### Purpose
Concrete implementations of Abstractions. Swappable details.

General rules:
- Can reference: Kernel, Domain projects, Abstractions, Contracts
- Must never be referenced by: Kernel or Domain projects

---

### 7.1) TechWayFit.ContentOS.Infrastructure.Identity
**Contains**
- AuthN/AuthZ integration (JWT/OIDC providers, claims mapping)
- Implementations of identity abstractions (e.g., `ICurrentUserProvider`)
- Permission/role evaluation mechanisms (implementation)

**Must NOT contain**
- Controllers (belong in API)
- Domain truth (belongs to Kernel/Tenancy)

---

### 7.2) TechWayFit.ContentOS.Infrastructure.Persistence
**Contains**
- Persistence-agnostic helpers and base patterns (repo base classes, conventions)
- Unit-of-work implementation (if it remains provider-neutral)
- Shared transaction utilities

**Must NOT contain**
- Postgres-specific code
- EF provider specifics

---

### 7.3) TechWayFit.ContentOS.Infrastructure.Persistence.Postgres
**Contains**
- EF Core + Npgsql DbContext, mappings, migrations
- Postgres repository implementations:
  - `ContentRepository : IContentRepository`
  - `TenantRepository : ITenantRepository`
- Postgres-specific optimizations

**Must NOT contain**
- Domain rules/invariants
- API DTOs/controllers

---

### 7.4) TechWayFit.ContentOS.Infrastructure.Search
**Contains**
- Implementations of `ISearchProvider` (Lucene/OpenSearch/etc.)
- Index writers/readers/analyzers (implementation)
- Background indexing workers (implementation)

**Must NOT contain**
- Search domain truth (belongs in `ContentOS.Search`)
- Controllers

---

### 7.5) TechWayFit.ContentOS.Infrastructure.Storage
**Contains**
- Implementations of blob/file storage providers (Local/S3/Azure Blob/etc.)
- Streaming, multipart upload, presigned URL generation (if required)
- Encryption-at-rest and storage-specific security details

**Must NOT contain**
- Media domain rules
- Controllers

---

### 7.6) TechWayFit.ContentOS.Infrastructure.Events
**Purpose**
Central home for eventing implementations (in-memory, MediatR, outbox, broker adapters).

**Contains**
- Concrete event bus implementations:
  - `InMemoryEventBus : IEventBus` (dev/test)
  - `MediatREventBus : IEventBus` (if using MediatR)
  - `OutboxEventBus` (future: reliable delivery)
- Event dispatch plumbing, handler registration, pipeline behaviors (implementation)
- Outbox processor, broker publishers (if/when introduced)
- Event serialization for transport (if needed)

**Must NOT contain**
- Domain rules/invariants
- API controllers
- Transport-specific tenant resolution (that stays in API/Identity middleware)
- Public event *definitions* (those belong in Contracts)

**Guidance**
- Use `InMemoryEventBus` only for dev/test; it is not durable.
- Production reliability should move toward Outbox + publisher pattern.

---

## 8) Events & Messaging Rules (Cross-cutting)
- **Domain events** (internal facts) live in the relevant **Domain project** (Content/Tenancy/etc.).
- **Integration/public events** live in **Contracts** and are versioned (`*EventV1`).
- **Event bus interfaces** live in **Abstractions**.
- **Event bus implementations** live in **Infrastructure.Events**.

Kernel must not contain event bus implementations.

---

## 9) Tenant Resolution Rules (Cross-cutting)
- Kernel/Domain define `Tenant` and `TenantId` and invariants only.
- `ITenantResolver` / `ICurrentTenantProvider` interfaces live in Abstractions.
- Implementations (header/jwt/subdomain/background-job resolvers) live in Infrastructure (Identity/Events/etc. as appropriate).
- API wires resolution via middleware and DI.

---

## 10) Quick Placement Guide (common items)

| Item | Goes Where |
|---|---|
`ContentItem` entity + publish rules | `ContentOS.Content` |
`CreateContentRequest` DTO | `ContentOS.Contracts` |
`IContentRepository` | `ContentOS.Abstractions` |
EF DbContext + Postgres repos | `Infrastructure.Persistence.Postgres` |
Search query model (domain) | `ContentOS.Search` |
Lucene/OpenSearch implementation | `Infrastructure.Search` |
Media asset domain model | `ContentOS.Media` |
Blob/S3/Azure storage implementation | `Infrastructure.Storage` |
`IEventBus` interface | `ContentOS.Abstractions` |
`InMemoryEventBus` implementation | `Infrastructure.Events` |
Domain event `ContentPublished` | `ContentOS.Content` |
Integration event `ContentPublishedEventV1` | `ContentOS.Contracts` |
Tenant domain + lifecycle | `ContentOS.Tenancy` |
Tenant resolver implementation | `Infrastructure.Identity` (or another infra project) |
Controllers + auth wiring | `ContentOS.Api` |
AI content generation | `ContentOS.AI` |

---

## 11) Allowed Dependency Direction (Enforced)

```text
Kernel
  ↑
Domain Bounded Contexts (Content/Media/Tenancy/Search/Workflow)
  ↑
Abstractions
  ↑
AI (optional, consumes domain via abstractions/contracts)
  ↑
Infrastructure (Persistence/Search/Storage/Identity/Events)
  ↑
API
```
Contracts are referenced where needed (API/AI/Infrastructure) but must not contain domain truth.
## 12) If Unsure → ADR
If placement is ambiguous and impacts architecture, do not guess—create/update an ADR.

If you want, I can also produce an updated **ADR-INDEX.md** entry for the new `Infrastructure.Events` project (and add a small “eventing strategy” note that points to ADR-006).