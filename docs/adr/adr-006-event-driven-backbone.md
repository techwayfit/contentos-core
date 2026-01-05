# ADR-006: Event-Driven Architecture Backbone

**Status:** Accepted  
**Date:** 2026-01-02  

## Context
ContentOS must support extensibility, async processing, and decoupled modules while remaining scalable and reliable.

## Decision
ContentOS will adopt an event-driven architecture:
- Core emits domain events for key actions
- Modules and services react asynchronously
- Events are first-class platform contracts

Examples:
- content.created
- content.published
- media.uploaded
- module.installed

## Consequences
- Loose coupling between services and modules
- Enables async workloads (search indexing, AI jobs)
- Requires strict event versioning and contracts
