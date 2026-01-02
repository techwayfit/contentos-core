# ADR-005: Licensing Outside Core Platform Logic

**Status:** Accepted  
**Date:** 2026-01-02

## Context
ContentOS must remain open and predictable while still supporting commercial value and licensed features.

## Decision
- ContentOS Core must not contain licensing or billing logic
- Monetisation occurs at:
  - Module level (e.g., Forge)
  - Capability provider level (e.g., AI services)
- Core evaluates capability availability, not pricing or plans

## Consequences
- Core remains open, trusted, and OSS-friendly
- Commercial features evolve independently
- Avoids long-term architectural and community friction
