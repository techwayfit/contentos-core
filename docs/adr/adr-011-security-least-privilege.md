# ADR-011: Security Model and Least-Privilege Access

**Status:** Accepted  
**Date:** 2026-01-02  

## Context
ContentOS must meet enterprise security and compliance expectations.

## Decision
Security principles:
- Tenant isolation at every layer
- Role-based access control (RBAC)
- Policy-based extensions (ABAC-ready)
- Least-privilege by default

## Consequences
- Strong security posture
- More upfront design effort
- Policies become a critical platform component
