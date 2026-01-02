# ContentOS – Copilot Instructions

> These instructions define **non-negotiable architectural rules** for the ContentOS codebase.  
> All generated code (by Copilot or humans) **must comply**.

---

## 1. Architecture Overview

ContentOS follows a **strict layered architecture**:

API (HTTP / Transport)
↓
Feature Projects (Domain + Application)
↓
Kernel (Runtime / Policies / Events)
↓
Abstractions (Ports / Contracts)
↓
Infrastructure (Provider-specific implementations)

**Rule of thumb**
- Upper layers depend on lower layers
- Lower layers must never depend on upper layers

---

## 2. Project Responsibilities

### TechWayFit.ContentOS.Api
**Thin transport layer only**

Allowed:
- HTTP endpoints (Minimal APIs)
- Authentication & Authorization wiring
- Request validation
- DTO ↔ command mapping
- DI composition root
- Middleware (tenant resolution, correlation id)

Not allowed:
- EF Core usage
- DbContext
- SQL queries
- Repository implementations
- Business logic
- Domain rules

Endpoints must call **use-cases**, not repositories.

---

### Feature Projects  
(`Content`, `Workflow`, `Search`, `Media`, `Tenancy`)

Contain **business logic and domain models**.

Allowed:
- Domain entities (business meaning)
- Use-cases / application services
- Domain rules & invariants
- Ports (interfaces) such as repositories
- Domain events

Not allowed:
- EF Core
- DbContext
- SQL
- Npgsql
- File system / blob SDKs
- Infrastructure concerns

---

### TechWayFit.ContentOS.Kernel
**Platform runtime**

Contains:
- Policy & permission model
- Domain event contracts
- Event bus abstractions
- Cross-cutting rules (tenancy, audit, security hooks)

Not allowed:
- EF Core
- Infrastructure implementations

---

### TechWayFit.ContentOS.Abstractions
**Pure contracts**

Contains:
- Ports/interfaces (`IUnitOfWork`, `ITenantContext`, etc.)
- Shared primitives (Result, Paging, Identifiers)

No implementation code.
No EF.
No HTTP.

---

### TechWayFit.ContentOS.Infrastructure.Persistence
**Persistence abstractions only (provider-agnostic)**

Allowed:
- Persistence options & conventions
- Provider-neutral contracts (if needed)
- Naming / tenant isolation conventions

Not allowed:
- DbContext
- EF migrations
- Npgsql
- Concrete repositories

---

### TechWayFit.ContentOS.Infrastructure.Persistence.Postgres
**Concrete EF Core + PostgreSQL implementation**

Allowed:
- DbContext
- EF Core Row entities
- Fluent configurations
- EF migrations
- Repository implementations
- Domain ↔ DB mapping
- Npgsql-specific configuration

Not allowed:
- Business logic
- API concerns

---

## 3. Domain Entity vs DB Entity (CRITICAL)

### Domain Entities
- Live in **feature projects**
- Represent business concepts
- Use value objects
- No persistence annotations
- No EF references

Example:
Content/Domain/ContentItem.cs

### DB Entities (Row Models)
- Live in **Infrastructure.Persistence.Postgres**
- Represent table shape
- Optimized for storage
- Must end with `Row`

Example:
Infrastructure.Persistence.Postgres/Entities/ContentItemRow.cs

### Mapping Rule
**All Domain ↔ DB mapping must be implemented in Infrastructure.Persistence.Postgres.**

Feature projects must never know DB shapes.

---

## 4. Multi-Tenancy (MANDATORY)

- Every persisted entity must include `TenantId`
- Every table must have `tenant_id`
- All uniqueness must be scoped by tenant
  - e.g. `unique (tenant_id, slug)`
- DbContext must apply **global query filters** for `TenantId`

Tenant is resolved per request:
- MVP: `X-Tenant-Id` header
- Admin routes are tenant-agnostic

---

## 5. Tenancy Management (SuperAdmin Only)

### Tenancy feature
- Domain: `Tenant`
- Port: `ITenantRepository`
- Use-cases: Create / Update / List tenants

### Repository location
- **Interface** → `TechWayFit.ContentOS.Tenancy`
- **Implementation** → `Infrastructure.Persistence.Postgres`

### Security rule
Tenant management APIs:
- Must require **SuperAdmin scope**
- Must NOT require `X-Tenant-Id`

---

## 6. Security & Authorization

### Permissions
- Feature permissions (e.g. `content:create`)
- Platform permissions (e.g. `platform:superadmin`)

### Policy enforcement
- All permission checks go through `IPolicyEvaluator`
- Feature code declares required permission
- API enforces it

### MVP
- SuperAdmin resolved via `X-SuperAdmin: true` header

### Future
- JWT + claims (`scope`, `role`)
- Same policy interface, stronger implementation

---

## 7. Persistence Rules

- Feature projects define **repository ports**
- Infrastructure implements them
- API wires implementations via DI

Never:
- Leak `IQueryable`
- Return EF entities outside Infrastructure
- Execute SQL outside Infrastructure

---

## 8. Events & Workflow

- Domain events raised in feature projects
- Event bus lives in Kernel
- Handlers live in feature or integration layers
- MVP uses in-memory event bus

Workflow:
- Separate feature
- Publishing must depend on approved workflow state (enforced in use-case)

---

## 9. Testing & Enforcement

- Architecture rules are enforced using **NetArchTest**
- Feature projects must not reference Infrastructure
- Only Persistence.Postgres may reference EF/Npgsql
- API must not reference `DbContext` or `*Row` entities

Violations must fail the build.

---

## 10. Copilot-Specific Guidance

When generating code:

- Always create **Domain entity + Row entity separately**
- Always place EF code in `Infrastructure.Persistence.Postgres`
- Always create repository **interfaces** in feature projects
- Always map Domain ↔ Row in Infrastructure
- Never put EF, SQL, or Npgsql in API or feature projects
- Admin APIs must check SuperAdmin policy
- TenantId must be included everywhere

If Copilot suggests mixing concerns:
**Reject it and refactor immediately.**

---

## 11. Design Intent

ContentOS is:
- API-first
- Multi-tenant by design
- Modular & extensible
- Infrastructure-replaceable
- Enterprise-grade

**Architectural clarity is more important than short-term convenience.**