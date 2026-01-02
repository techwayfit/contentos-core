# ContentOS – Architecture Guardrails

> This document defines **hard architectural rules** for ContentOS.  
> These are **guardrails**, not suggestions.

If any rule below is violated, the change **must not be merged**.

---

## 1. Layering Rules (Non-Negotiable)

API
↓
Feature Projects (Domain + Application)
↓
Kernel
↓
Abstractions
↓
Infrastructure (Provider-specific)

- Dependencies may only flow **downwards**
- No circular dependencies
- Infrastructure is always the **lowest layer**

---

## 2. Project-Level Responsibilities

### API (`TechWayFit.ContentOS.Api`)
**Transport only**

✅ Allowed  
- HTTP endpoints (Minimal APIs)
- Authentication & authorization wiring
- Request validation
- DTO ↔ command mapping
- Dependency Injection composition root
- Middleware (tenant, correlation id)

❌ Forbidden  
- EF Core usage
- DbContext
- SQL queries
- Repository implementations
- Domain/business logic
- `*Row` entities

---

### Feature Projects  
(`Content`, `Workflow`, `Search`, `Media`, `Tenancy`)

**Business logic only**

✅ Allowed  
- Domain entities
- Use-cases / application services
- Domain rules & invariants
- Repository **interfaces** (ports)
- Domain events

❌ Forbidden  
- EF Core
- DbContext
- SQL
- Npgsql
- Infrastructure SDKs
- File system / blob APIs

---

### Kernel (`TechWayFit.ContentOS.Kernel`)
**Platform runtime**

✅ Allowed  
- Policies & permissions
- Event contracts & bus abstraction
- Cross-cutting rules (tenancy, auditing hooks)

❌ Forbidden  
- EF Core
- Infrastructure implementations

---

### Abstractions (`TechWayFit.ContentOS.Abstractions`)
**Pure contracts**

✅ Allowed  
- Interfaces / ports
- Shared primitives
- Identifiers, paging, results

❌ Forbidden  
- Implementations
- EF Core
- HTTP concerns

---

### Infrastructure.Persistence (Abstract)
**Provider-agnostic persistence contracts**

✅ Allowed  
- Persistence options
- Naming & tenancy conventions
- Provider-neutral contracts (if needed)

❌ Forbidden  
- DbContext
- EF migrations
- Npgsql
- Concrete repositories

---

### Infrastructure.Persistence.Postgres
**EF Core + PostgreSQL implementation**

✅ Allowed  
- DbContext
- EF Row entities
- Fluent configurations
- Migrations
- Repository implementations
- Domain ↔ DB mapping
- Npgsql configuration

❌ Forbidden  
- Business logic
- API concerns

---

## 3. Domain vs Database Model (CRITICAL)

### Domain Entity
- Lives in feature projects
- Represents business meaning
- Uses value objects
- No persistence annotations

Example:
Content/Domain/ContentItem.cs

### DB Entity (Row Model)
- Lives in `Infrastructure.Persistence.Postgres`
- Represents table shape
- Must end with `Row`

Example:
Infrastructure.Persistence.Postgres/Entities/ContentItemRow.cs

### Mapping Rule
**All Domain ↔ DB mapping must happen in Infrastructure.Persistence.Postgres.**

Feature projects must never know database shapes.

---

## 4. Multi-Tenancy Rules (MANDATORY)

- Every persisted entity **must include TenantId**
- Every table **must include tenant_id**
- All uniqueness constraints are tenant-scoped
- DbContext **must enforce tenant isolation via global query filters**

Tenant resolution:
- MVP: `X-Tenant-Id` header
- Admin routes are tenant-agnostic

---

## 5. Tenancy Management (SuperAdmin Only)

- Tenant CRUD lives in **Tenancy feature project**
- `ITenantRepository` → Tenancy project
- `EfTenantRepository` → Persistence.Postgres

Security:
- Tenant APIs require **SuperAdmin**
- Must NOT require `X-Tenant-Id`

---

## 6. Security & Authorization

- Permissions are enforced via `IPolicyEvaluator`
- API enforces permissions
- Feature code declares required permission
- No direct auth logic inside domain or infrastructure

MVP:
- SuperAdmin resolved via `X-SuperAdmin: true`

Future:
- JWT + claims (`scope`, `role`)
- Same policy interface

---

## 7. Persistence Rules

- Feature projects define repository **interfaces**
- Infrastructure implements repositories
- API wires implementations via DI

Never allowed:
- Returning EF entities outside Infrastructure
- Exposing `IQueryable`
- Executing SQL outside Infrastructure

---

## 8. Events & Workflow

- Domain events originate in feature projects
- Event bus abstraction lives in Kernel
- Handlers live outside domain
- MVP uses in-memory event bus

Workflow rules:
- Workflow is a separate feature
- Publishing must depend on workflow approval

---

## 9. Enforcement

These rules are enforced via:
- NetArchTest boundary tests
- Code reviews
- Copilot instructions

If a change violates architecture:
**Fix the design, not the rule.**

---

## 10. Design Philosophy

ContentOS prioritizes:
- Clarity over convenience
- Boundaries over shortcuts
- Long-term scalability over short-term speed

**Architecture is a feature.**