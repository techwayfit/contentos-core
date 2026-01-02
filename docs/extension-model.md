# ContentOS Extension Model (Forge)

This document explains how **Forge modules** integrate with ContentOS Core.

---

## Design Goals
- Core remains stable and minimal
- Modules are independently installable
- No compile-time dependency from Core to Forge
- Clear upgrade and licensing boundaries

---

## Extension Mechanisms

Forge modules integrate via:

### 1. Abstractions
- Implement interfaces from `ContentOS.Abstractions`
- Register services using DI extension methods

---

### 2. Events
- Subscribe to domain or integration events
- React without tight coupling

Example:
- ContentCreated
- WorkflowStateChanged

---

### 3. Pipelines / Hooks
- Middleware-like extension points
- Ordered execution where needed

---

### 4. Capability Discovery
- Modules declare capabilities at startup
- Core enables or disables behavior via policies

---

## What Forge Modules CAN Do
- Add domain-specific behavior (Blog, Commerce, Library)
- Extend workflows
- Introduce UI (Studio plugins)
- Add AI-powered enhancements

---

## What Forge Modules CANNOT Do
- Modify Core runtime behavior directly
- Bypass Kernel security or tenancy
- Assume internal Core implementations

---

## Dependency Rules
- Forge → Abstractions
- Forge → Contracts
- Forge → Core (via extension points)
- Core → Forge ❌ (never)

---

## Resulting Benefits
- Safe upgrades
- Marketplace-ready modules
- Clear licensing boundaries
- Long-term architectural stability
