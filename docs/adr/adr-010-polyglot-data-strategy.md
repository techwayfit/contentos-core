# ADR-010: Data Storage Strategy (Polyglot Persistence)

**Status:** Accepted  
**Date:** 2026-01-02  

## Context
Different CMS workloads have different data access patterns and performance needs.

## Decision
ContentOS will use polyglot persistence:
- Relational DB for transactional data
- Blob storage for media
- Search index for keyword queries
- Vector store for semantic features
- Cache and queues for performance and async jobs

## Consequences
- Optimal performance per workload
- Increased operational complexity
- Clear data ownership boundaries required
