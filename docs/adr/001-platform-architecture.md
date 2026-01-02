# ADR-001: Platform Architecture - Headless, API-First CMS

## Status
Accepted

## Context
We need to build an enterprise-grade content management platform that serves as a headless CMS with multi-tenant capabilities. The platform must be API-first with no mandatory UI, enabling flexibility for various frontend implementations.

## Decision
Build TechWayFit ContentOS Core as a modular, headless-first platform with the following characteristics:

1. **Multi-tenant by design**: Support for tenant → site → environment hierarchy
2. **API-first**: All functionality exposed through REST APIs, no UI in core
3. **Event-driven**: Domain events for loose coupling between modules
4. **Deterministic baseline**: Core features work without AI dependencies
5. **AI-optional**: AI capabilities as an optional assistance layer, not a requirement
6. **Modular architecture**: Clear separation of concerns across projects:
   - Abstractions: Core interfaces and contracts
   - Contracts: DTOs, schemas, domain events
   - Kernel: Multi-tenancy, RBAC, event bus
   - Content: Schema definition and CRUD
   - Workflow: Draft/review/publish lifecycle
   - Media: Digital asset management (DAM-lite)
   - Search: Keyword-based content discovery
   - AI: Optional AI orchestration (provider-agnostic)
   - Api: REST API gateway

## Consequences

### Positive Consequences
- Clear separation of concerns enables independent module evolution
- Headless architecture allows frontend flexibility
- Multi-tenant design enables SaaS deployments
- Event-driven architecture enables extensibility
- AI-optional approach ensures platform works deterministically
- No licensing/billing logic keeps focus on core CMS capabilities

### Negative Consequences
- Requires separate UI implementation
- More complex initial setup compared to monolithic CMS
- Developers must understand event-driven patterns

## Alternatives Considered
1. **Monolithic CMS with built-in UI**: Rejected due to lack of flexibility
2. **Mandatory AI integration**: Rejected to ensure deterministic behavior
3. **Single-tenant architecture**: Rejected as it limits SaaS capabilities

## References
- Headless CMS principles
- Multi-tenant architecture patterns
- Event-driven architecture patterns
