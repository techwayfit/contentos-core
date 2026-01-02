# Copilot Context — TechWayFit ContentOS (Core)

This repository contains the **ContentOS Core runtime** (API-first CMS foundation).
Copilot: follow these rules strictly.

## Non-negotiables (read first)
1. **Core is the platform**. It must remain usable without any Forge module installed.
2. **Core must NOT reference Forge implementations** (no direct compile-time dependency).
3. **Core exposes stable contracts** via `ContentOS.Abstractions` (interfaces, events, DTOs).
4. **Modules integrate via extension points** (DI registration, pipeline hooks, events, feature flags).
5. **API-first**: all business capabilities are exposed via APIs (Studio/UX is a separate concern).
6. **Security by default**: secure headers, authn/authz, validation, rate limits, auditability.
7. **AI is optional + policy-driven**: no AI hard dependency in core business flows.
8. **Versioning discipline**: contracts are backward compatible; breaking changes require ADR.
9. Prefer **clean boundaries** over convenience (no “just call that class” across layers).
10. Favor **small, composable services** and explicit interfaces.

## Repository intent
Core provides:
- Hosting + composition model
- Cross-cutting platform services (auth, tenancy, config, observability)
- Extensibility points for modules
- Stable abstractions/contracts

Core does NOT provide:
- Domain-specific features that belong to modules (Blog, Library, Commerce, etc.)
- Studio UI (author/admin UX)
- Vendor-specific infrastructure scripts (handled elsewhere)

## Key architectural rules
### Layering
- `ContentOS.Abstractions` — stable interfaces, DTOs, events (lowest dependency)
- `ContentOS.Core` — platform services + composition root
- `ContentOS.Api` — HTTP surface area; thin controllers; delegates to application services

### Dependency direction
- `Api` → `Core` → `Abstractions`
- `Abstractions` depends on nothing else in this repo
- No reverse dependencies

### Extensibility (how modules plug in)
- Modules implement interfaces in `Abstractions`
- Modules register using DI (extension methods) and/or discovery
- Communication via:
  - domain events / integration events
  - contracts (interfaces)
  - well-defined pipelines (middlewares/handlers)

## Coding conventions
- Prefer constructor injection
- Prefer explicit interfaces (no service locator)
- Keep controllers thin (validation + mapping + calling services)
- Prefer async all the way
- Avoid static state; ensure multi-tenant safety
- Log with structured logging; never log secrets/PII

## “Do / Don’t” for Copilot
✅ DO:
- Create abstractions first when needed
- Add extension points instead of hard coupling
- Keep changes local to one layer when possible
- Add tests for boundaries and key workflows

❌ DON’T:
- Reference module assemblies/namespaces from core
- Put domain features in core if they belong to a module
- Introduce breaking contract changes without ADR
- Embed secrets, keys, or environment assumptions

## Architecture references
- ADR Index: see `../contentos-docs/adr/ADR-000-Index.md` (if repo is checked out side-by-side)
- Diagrams and principles live in `contentos-docs`

## Module Map (Source of Truth)

- TechWayFit.ContentOS.Abstractions
  → Interfaces, events, capability contracts

- TechWayFit.ContentOS.Contracts
  → DTOs, schemas, API payloads

- TechWayFit.ContentOS.Kernel
  → Tenancy, RBAC, event bus, platform primitives

- TechWayFit.ContentOS.Content
  → Content schema + CRUD (core capability)

- TechWayFit.ContentOS.Media
  → Asset management (core capability)

- TechWayFit.ContentOS.Workflow
  → Draft → Review → Publish lifecycle

- TechWayFit.ContentOS.Search
  → Indexing + query abstraction (no engine coupling)

- TechWayFit.ContentOS.AI
  → Optional, policy-driven assistance
