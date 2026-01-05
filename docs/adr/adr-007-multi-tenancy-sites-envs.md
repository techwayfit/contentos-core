# ADR-007: Multi-Tenancy, Sites, and Environments

**Status:** Accepted  
**Date:** 2026-01-02  

## Context
Enterprise CMS deployments require isolation between tenants, multiple sites per tenant, and environment separation.

## Decision
ContentOS will support:
- Tenant → Site → Environment hierarchy
- Environment isolation (dev/stage/prod)
- Policy and configuration scoped at each level

## Consequences
- Enables enterprise governance
- Adds complexity to resolution and routing
- Requires consistent tenant context propagation
