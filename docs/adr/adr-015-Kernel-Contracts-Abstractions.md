
# ADR-015: Separation of Kernel, Contracts, and Abstractions

## Status
Accepted

## Date
2026-01-02

## Context

ContentOS is designed as:
- API-first
- Modular (Forge installable modules)
- Multi-tenant by default
- Extensible with AI and licensed capabilities

Early iterations showed risk of:
- Domain leakage into APIs
- Infrastructure coupling with business logic
- Unstable module extension points

A strict separation was required to ensure:
- Long-term maintainability
- Safe OSS + licensed evolution
- Clear ownership boundaries

---

## Decision

We introduce **three explicit Core layers**:

1. **Kernel** – Domain truth
2. **Contracts** – Public communication surface
3. **Abstractions** – Behavioral dependency inversion layer

Each layer has **non-overlapping responsibilities** and **strict dependency rules**.

---

## Definitions

### Kernel
- Contains domain entities, value objects, and invariants
- Expresses business meaning only
- Has zero dependencies

### Contracts
- Defines DTOs, events, and extension interfaces
- Serves API consumers and Forge modules
- Is versioned and backward-compatible where possible

### Abstractions
- Defines repository and service interfaces
- Enables infrastructure swapping
- Enforces dependency inversion

---

## Dependency Rules

- Kernel must not reference any other project
- Contracts must not contain business logic
- Abstractions must not contain implementations
- Infrastructure depends on Abstractions, never the reverse

Violations are considered **architecture defects**.

---

## Consequences (Positive)

- Clear extension model for Forge
- Clean multi-tenant enforcement
- Infrastructure independence
- Safer public APIs
- AI remains optional and policy-driven
- Improved testability

---

## Consequences (Trade-offs)

- More projects to understand
- Slightly higher upfront discipline
- Requires ADR familiarity for contributors

These trade-offs are accepted for enterprise longevity.

---

## Enforcement

- Architecture reviews
- Dependency checks
- Copilot instruction files
- Mandatory ADR for boundary exceptions

---

## Related Documents

- `architecture.md`
- `module-boundaries.md`
- `extension-model.md`
- ADR-001 to ADR-005