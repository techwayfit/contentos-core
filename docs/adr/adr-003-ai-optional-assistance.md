# ADR-003: AI as Optional Assistance Layer

**Status:** Accepted  
**Date:** 2026-01-02

## Context
AI capabilities (content assist, moderation, search ranking) provide significant value but introduce cost, latency, and governance considerations.

## Decision
AI is implemented as an **optional assistance layer**:
- Deterministic baseline logic always exists
- AI augments functionality via policy-driven pipelines
- Core functionality must not depend on AI availability

## Consequences
- Platform degrades gracefully when AI is unavailable
- AI usage can be controlled per tenant or environment
- Improves reliability, explainability, and cost control
