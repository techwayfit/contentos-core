# ADR-002: Headless-First, API-Only Core

**Status:** Accepted  
**Date:** 2026-01-02

## Context
ContentOS must support a wide variety of consumers including web UIs, mobile apps, static sites, CLIs, and external systems.

## Decision
ContentOS Core will be **headless-first and API-only**:
- All functionality is exposed via APIs and domain events
- No UI-specific or privileged database logic exists in Core
- UX layers are treated as external clients

## Consequences
- UI becomes optional and replaceable
- Platform supports future and unknown consumers
- Strong API and contract discipline is required
