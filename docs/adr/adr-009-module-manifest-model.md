# ADR-009: Module Manifest and Installation Model

**Status:** Accepted  
**Date:** 2026-01-02  

## Context
ContentOS must allow safe installation, upgrade, and removal of feature modules.

## Decision
All modules must provide:
- A manifest describing schemas, APIs, permissions, UI extensions
- Explicit migrations and rollback rules
- Version compatibility declarations

## Consequences
- Predictable module lifecycle
- Safe upgrades and removals
- Enables future marketplace model
