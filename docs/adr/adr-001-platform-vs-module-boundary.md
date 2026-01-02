# ADR-001: Platform vs Module Boundary

**Status:** Accepted  
**Date:** 2026-01-02

## Context
TechWayFit is evolving from a Blog Management System into an enterprise-grade CMS (ContentOS) that supports multiple installable capabilities such as blogging, libraries, and commerce. Clear boundaries are required to ensure scalability, extensibility, and long-term maintainability.

## Decision
The system is divided into three distinct layers:
- **ContentOS Core**: Always-installed platform services
- **Modules** (e.g., Forge): Installable, removable feature packages
- **UX (Studio)**: Optional client layer consuming APIs

ContentOS Core must not contain module-specific business logic.

## Consequences
- Core remains stable, trusted, and extensible
- Modules can evolve, be licensed, and versioned independently
- Clear architectural boundaries reduce long-term coupling
