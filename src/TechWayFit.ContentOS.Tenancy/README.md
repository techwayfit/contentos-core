# TechWayFit.ContentOS.Tenancy

## Overview
Tenant management feature for ContentOS. Handles CRUD operations for tenants - the top-level isolation boundary in the multi-tenant architecture.

## Architecture
- **Domain**: Tenant entity with business rules
- **Ports**: ITenantRepository interface for persistence
- **Use-cases**: CreateTenant, UpdateTenant, GetTenant, ListTenants
- **Security**: SuperAdmin scope required for all operations

## Structure
```
Domain/
  Tenant.cs              # Tenant domain entity
  TenantStatus.cs        # Active/Disabled enum
Ports/
  ITenantRepository.cs   # Repository contract
Application/
  CreateTenantUseCase.cs
  UpdateTenantUseCase.cs
  GetTenantUseCase.cs
  ListTenantsUseCase.cs
```

## Important Notes
- Tenant management is **SuperAdmin only** - requires `platform:superadmin` permission
- Tenant key must be **globally unique** (enforced at database level)
- This is a separate feature from tenant *runtime context* (which lives in Kernel)
