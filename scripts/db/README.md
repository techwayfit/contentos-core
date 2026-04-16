# ContentOS PostgreSQL Database Setup

This directory contains PostgreSQL database setup scripts for ContentOS.

## ?? Scripts Overview

| Script | Description |
|--------|-------------|
| `01-setup-database.sql` | Creates database, extensions, functions, and triggers |
| `02-create-core-tables.sql` | Core tables: Tenant, Site, User, Role, Group |
| `03-create-content-tables.sql` | Content management: Types, Items, Versions, Nodes, Routes |
| `04-create-workflow-security-tables.sql` | Workflow, ACL, Preview Tokens, Audit Logs |
| `05-create-layout-module-tables.sql` | Layout definitions, Components, Modules |
| `06-seed-data.sql` | Initial seed data (demo tenant, roles, workflow) |

## ?? Quick Start

### Prerequisites

- PostgreSQL 14+ installed and running
- `psql` command-line client available
- Database user with `CREATEDB` privilege

### Option 1: Automated Setup (Recommended)

#### Linux/macOS:

```bash
cd scripts/db

# Make script executable
chmod +x setup-database.sh

# Run setup
./setup-database.sh
```

#### Windows:

```cmd
cd scripts\db
setup-database.bat
```

### Option 2: Manual Setup

```bash
# 1. Create database
createdb -U postgres contentos_core

# 2. Run scripts in order
psql -U postgres -d contentos_core -f 01-setup-database.sql
psql -U postgres -d contentos_core -f 02-create-core-tables.sql
psql -U postgres -d contentos_core -f 03-create-content-tables.sql
psql -U postgres -d contentos_core -f 04-create-workflow-security-tables.sql
psql -U postgres -d contentos_core -f 05-create-layout-module-tables.sql
psql -U postgres -d contentos_core -f 06-seed-data.sql
```

## ?? Configuration

Set environment variables to customize the setup:

```bash
# Linux/macOS
export DB_NAME=contentos_core
export DB_USER=postgres
export DB_HOST=localhost
export DB_PORT=5432
export PGPASSWORD=your_password

# Windows
set DB_NAME=contentos_core
set DB_USER=postgres
set DB_HOST=localhost
set DB_PORT=5432
set PGPASSWORD=your_password
```

## ?? Default Credentials

After running the seed script, you'll have:

### System Tenant
- **Tenant Key**: `system`
- **Tenant ID**: `00000000-0000-0000-0000-000000000001`
- Purpose: Platform-wide system data

### Demo Tenant
- **Tenant Key**: `demo`
- **Tenant ID**: `11111111-1111-1111-1111-111111111111`
- **Site**: `localhost`
- **Default User**: `admin@demo.local`
- **Role**: `TenantAdmin`

### Default Roles (Demo Tenant)
- `TenantAdmin` - Tenant administrator
- `Editor` - Content editor
- `Publisher` - Content publisher
- `Viewer` - Read-only viewer

### Default Workflow
- **Name**: Default Workflow
- **States**: Draft ? Review ? Published
- **Transitions**: Submit, Publish, Reject

## ?? Database Schema

### Core Tables (7)
- `tenants` - Top-level isolation boundary
- `sites` - Multi-site support
- `users` - User accounts
- `roles` - Role definitions (RBAC)
- `groups` - User grouping
- `user_roles` - User ? Role mapping
- `user_groups` - User ? Group mapping

### Content Tables (7)
- `content_types` - Content type definitions
- `content_type_fields` - Field definitions
- `content_nodes` - Content hierarchy (tree)
- `routes` - URL routing
- `content_items` - Content instances
- `content_versions` - Versioned content
- `content_field_values` - Field storage

### Workflow & Security (4)
- `workflow_definitions` - Workflow graphs
- `workflow_states` - Workflow states
- `workflow_transitions` - Allowed transitions
- `acl_entries` - Access control lists
- `preview_tokens` - Secure preview links
- `audit_logs` - Audit trail (append-only)

### Layout & Modules (8)
- `layout_definitions` - Layout templates
- `component_definitions` - Component registry
- `content_layouts` - Composed layouts
- `modules` - Module registry
- `module_capabilities` - Feature flags
- `module_settings` - Module configuration
- `module_migrations` - Migration tracking

## ?? Verification

After setup, verify the installation:

```sql
-- Check tables
SELECT schemaname, tablename 
FROM pg_tables 
WHERE schemaname = 'contentos'
ORDER BY tablename;

-- Check tenants
SELECT id, key, name, status 
FROM contentos.tenants;

-- Check users
SELECT id, email, display_name, tenant_id 
FROM contentos.users;

-- Check roles
SELECT id, name, description, is_system 
FROM contentos.roles;

-- Check workflow
SELECT wd.workflow_key, ws.state_key, ws.display_name
FROM contentos.workflow_definitions wd
JOIN contentos.workflow_states ws ON ws.workflow_definition_id = wd.id
WHERE wd.is_default = TRUE
ORDER BY ws.state_key;
```

## ?? Clean Up / Reset

**?? WARNING: This will delete all data!**

```bash
# Drop database
dropdb -U postgres contentos_core

# Recreate
createdb -U postgres contentos_core

# Re-run setup scripts
./setup-database.sh
```

Or use SQL:

```sql
-- Connect to postgres database
\c postgres

-- Terminate existing connections
SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = 'contentos_core'
  AND pid <> pg_backend_pid();

-- Drop and recreate
DROP DATABASE IF EXISTS contentos_core;
CREATE DATABASE contentos_core;
```

## ?? Connection String

Update your `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=contentos_core;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

## ?? Troubleshooting

### Cannot connect to PostgreSQL

```bash
# Check if PostgreSQL is running
# Linux/macOS
pg_isready -h localhost -p 5432

# Windows
pg_ctl status -D "C:\Program Files\PostgreSQL\14\data"

# Start PostgreSQL if stopped
# Linux
sudo systemctl start postgresql

# macOS
brew services start postgresql

# Windows
net start postgresql-x64-14
```

### Permission denied

Ensure your user has database creation privileges:

```sql
-- As postgres superuser
ALTER USER your_username CREATEDB;

-- Grant schema permissions
GRANT ALL PRIVILEGES ON SCHEMA contentos TO your_username;
```

### Script fails midway

Check the error message. Common issues:

1. **Duplicate key violations**: Database already has data. Use clean up steps above.
2. **Extension not available**: Install PostgreSQL extensions:
   ```bash
   # Ubuntu/Debian
   sudo apt-get install postgresql-contrib postgresql-14-pgvector
   
   # macOS
   brew install pgvector
   ```

3. **Syntax errors**: Ensure you're using PostgreSQL 14+:
   ```sql
   SELECT version();
   ```

## ?? Test Data

For development/testing, you can insert additional test data:

```sql
-- Insert test content type
INSERT INTO contentos.content_types (
    id, tenant_id, type_key, display_name, schema_version,
    created_on, created_by, updated_on, updated_by,
    is_deleted, is_active, can_delete, is_system
) VALUES (
    gen_random_uuid(),
    '11111111-1111-1111-1111-111111111111'::UUID,
    'blog.post',
    'Blog Post',
    1,
    NOW(), '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(), '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE, TRUE, TRUE, FALSE
);
```

## ?? Additional Resources

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [ContentOS Architecture Guide](../../docs/architecture.md)
- [Database Design](../../docs/db-design.md)
- [Repository Implementation Guide](../../docs/repository-implementation-guide.md)

## ?? Reporting Issues

If you encounter issues with the setup scripts:

1. Check PostgreSQL logs: `/var/log/postgresql/` (Linux) or Event Viewer (Windows)
2. Verify PostgreSQL version: `psql --version` (must be 14+)
3. Check connection settings
4. Review error messages in script output

## ?? Notes

- All tables use UUID primary keys
- All entities support soft delete (`is_deleted`, `deleted_on`)
- All entities include full audit trail (`created_on`, `created_by`, `updated_on`, `updated_by`)
- Multi-tenancy is enforced at the database level (all queries filter by `tenant_id`)
- Indexes are optimized for tenant-scoped queries
- Triggers automatically update `updated_on` timestamps

## ?? Migrations

For production environments, use EF Core migrations instead of running these scripts directly:

```bash
# Generate migration
dotnet ef migrations add InitialCreate --project src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres

# Update database
dotnet ef database update --project src/infrastructure/persistence/TechWayFit.ContentOS.Infrastructure.Persistence.Postgres
```

These SQL scripts are provided for:
- Development/local setup
- Understanding the schema
- Manual database provisioning
- CI/CD pipelines
- Database documentation
