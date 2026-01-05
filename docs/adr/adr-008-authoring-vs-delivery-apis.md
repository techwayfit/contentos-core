# ADR-008: Separation of Authoring and Delivery APIs

**Status:** Accepted  
**Date:** 2026-01-02  

## Context
Authoring workflows and content delivery have very different performance, security, and caching needs.

## Decision
ContentOS will expose:
- Authoring APIs (drafts, workflow, preview)
- Delivery APIs (published-only, cacheable)

These APIs are logically and operationally separated.

## Consequences
- Improved delivery performance
- Clear security boundaries
- Simplifies CDN and caching strategies
