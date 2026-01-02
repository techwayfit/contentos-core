# ADR-004: Policy-Driven Capability Evaluation

**Status:** Accepted  
**Date:** 2026-01-02

## Context
Different tenants and environments require different behavior for workflows, AI usage, moderation strictness, and feature availability.

## Decision
Introduce a **policy evaluation layer**:
- Policies determine capability availability and execution paths
- Policies are scoped per tenant, site, and environment
- Consumers call stable APIs; behavior varies via policy

## Consequences
- Avoids API fragmentation
- Enables feature control without code changes
- Separates "what is requested" from "how it is executed"
