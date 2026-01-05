# ADR-012: Versioning Strategy for APIs and Modules

**Status:** Accepted  
**Date:** 2026-01-02  

## Context
ContentOS APIs and modules will evolve independently over time.

## Decision
- Semantic Versioning (SemVer) for APIs and modules
- Backward-compatible changes preferred
- Breaking changes require major version bump
- Contracts treated as public APIs

## Consequences
- Predictable upgrades
- Requires strong discipline in contract changes
- Encourages additive evolution
