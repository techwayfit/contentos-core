# Copilot Instructions — ContentOS Core

You are generating code for an enterprise-grade, API-first CMS platform core.
Follow these constraints:

## Architecture constraints
- Core MUST NOT reference Forge module implementations.
- Add new extension points via interfaces/events/pipelines in `ContentOS.Abstractions`.
- Keep contracts backward compatible. If a breaking change is unavoidable, add an ADR entry (do not proceed silently).

## Code style
- Use C#/.NET idioms: async/await, DI, small services, explicit interfaces.
- Controllers must be thin: validate → call application service → return response.
- Prefer records for immutable DTOs (when appropriate).
- Use structured logging and avoid logging PII/secrets.

## Security defaults
- Validate input (server-side).
- Use authorization checks at API boundaries.
- Deny-by-default for privileged operations.
- Be careful with serialization and over-posting.

## Multi-tenancy & stability
- No static mutable state.
- Ensure services are safe for concurrent requests.
- Keep module-specific behavior behind extension points.

## Deliverables
When adding a feature:
1) update abstractions if needed
2) implement core service
3) add API endpoint (if relevant)
4) add tests (unit + boundary/architecture tests when relevant)
5) update docs pointers (README links)
