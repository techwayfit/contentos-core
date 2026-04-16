# ContentOS PostgreSQL Database Scripts - Index

## ?? Directory Structure

```
scripts/db/
??? 01-setup-database.sql           # Database, extensions, functions
??? 02-create-core-tables.sql          # Tenants, users, roles, groups
??? 03-create-content-tables.sql       # Content types, items, versions
??? 04-create-workflow-security-tables.sql  # Workflow, ACL, audit
??? 05-create-layout-module-tables.sql # Layouts, components, modules
??? 06-seed-data.sql                   # Demo tenant, roles, workflow
??? setup-database.sh         # Linux/macOS setup script
??? setup-database.bat    # Windows setup script
??? README.md     # Detailed script documentation
??? SETUP-SUMMARY.md    # Quick reference

docker-compose.db.yml         # Docker Compose for PostgreSQL + pgAdmin

docs/
??? database-setup-guide.md            # **START HERE** - Complete guide
??? docker-database-setup.md  # Docker setup instructions
??? db-design.md       # Detailed schema design
??? repository-implementation-guide.md  # Repository patterns
```

## ?? Quick Start

### Option 1: Docker (Fastest)

```bash
docker-compose -f docker-compose.db.yml up -d
```

### Option 2: Automated Scripts

```bash
cd scripts/db
chmod +x setup-database.sh
./setup-database.sh
```

### Option 3: Manual

```bash
cd scripts/db
psql -U postgres -d contentos_core -f 01-setup-database.sql
# ... continue with other scripts
```

## ?? Documentation

| Document | Purpose | Audience |
|----------|---------|----------|
| **[database-setup-guide.md](../docs/database-setup-guide.md)** | Complete setup guide with all options | Everyone |
| [docker-database-setup.md](../docs/docker-database-setup.md) | Docker-specific instructions | Docker users |
| [README.md](README.md) | Detailed script documentation | Developers |
| [SETUP-SUMMARY.md](SETUP-SUMMARY.md) | Quick reference and features | Developers |
| [db-design.md](../docs/db-design.md) | Schema design and architecture | Architects |

## ?? What Gets Created

### Core Tables (7)
- `tenants` - Multi-tenancy foundation
- `sites` - Multi-site support
- `users` - Identity records
- `roles` - RBAC
- `groups` - User grouping
- `user_roles` - User-Role mapping
- `user_groups` - User-Group mapping

### Content Tables (7)
- `content_types` - Schema registry
- `content_type_fields` - Field definitions
- `content_nodes` - Content hierarchy
- `routes` - URL routing
- `content_items` - Content instances
- `content_versions` - Versioned content
- `content_field_values` - Field storage

### Workflow & Security (6)
- `workflow_definitions` - Workflow graphs
- `workflow_states` - Workflow states
- `workflow_transitions` - Transitions
- `acl_entries` - Permissions
- `preview_tokens` - Secure previews
- `audit_logs` - Audit trail

### Layout & Modules (8)
- `layout_definitions` - Layout templates
- `component_definitions` - Components
- `content_layouts` - Composed layouts
- `modules` - Module registry
- `module_capabilities` - Feature flags
- `module_settings` - Configuration
- `module_migrations` - Migration tracking

### Seed Data
- System & Demo tenants
- Default roles
- Demo admin user
- Default workflow (Draft ? Review ? Published)
- Basic page content type

## ? Key Features

? **Multi-Tenancy** - Every table tenant-scoped
? **Soft Delete** - Safe deletion with audit trail
? **Audit Trail** - Full created/updated/deleted tracking
? **Performance** - 100+ optimized indexes
? **Security** - ACL, hashed preview tokens
? **Workflow** - State-based content lifecycle
? **Idempotent** - Safe to run multiple times

## ?? Connection Strings

### Development (Docker)
```
Host=localhost;Port=5432;Database=contentos_core;Username=postgres;Password=postgres
```

### Production
```
Host=your-db-host;Port=5432;Database=contentos_core;Username=your_user;Password=your_password;SSL Mode=Require
```

## ?? Learning Path

1. **First Time**
   - Read [database-setup-guide.md](../docs/database-setup-guide.md)
   - Run setup script
   - Verify installation

2. **Understanding Schema**
   - Read [db-design.md](../docs/db-design.md)
   - Review ERD diagrams
   - Study table relationships

3. **Development**
   - Read [repository-implementation-guide.md](../docs/repository-implementation-guide.md)
   - Understand patterns
   - Implement repositories

4. **Docker Users**
   - Read [docker-database-setup.md](../docs/docker-database-setup.md)
   - Use docker-compose
   - Access pgAdmin

## ?? Verification Queries

```sql
-- Check tables
SELECT tablename FROM pg_tables 
WHERE schemaname = 'contentos' 
ORDER BY tablename;

-- Check tenants
SELECT * FROM contentos.tenants;

-- Check roles
SELECT r.name, t.key as tenant 
FROM contentos.roles r 
JOIN contentos.tenants t ON t.id = r.tenant_id;

-- Check workflow
SELECT wd.workflow_key, ws.state_key 
FROM contentos.workflow_definitions wd
JOIN contentos.workflow_states ws ON ws.workflow_definition_id = wd.id
WHERE wd.is_default = TRUE;
```

## ?? Troubleshooting

| Issue | Solution |
|-------|----------|
| Cannot connect | Check PostgreSQL is running |
| Permission denied | Grant CREATEDB privilege |
| Extension not found | Install postgresql-contrib |
| Script fails | Check PostgreSQL version (14+) |
| Duplicate keys | Drop and recreate database |

See [database-setup-guide.md](../docs/database-setup-guide.md#troubleshooting) for detailed solutions.

## ?? Statistics

- **Total Tables**: 25+
- **Total Indexes**: 100+
- **Foreign Keys**: 40+
- **Triggers**: 20+
- **Seed Records**: 20+
- **Setup Time**: < 2 minutes

## ?? Next Steps

1. ? Run setup script
2. ? Update `appsettings.json`
3. ? Run application
4. ? Test API with demo tenant
5. ? Create your first content

## ?? Support

- **PostgreSQL Issues**: Check logs in `/var/log/postgresql/`
- **Docker Issues**: Check `docker-compose logs postgres`
- **Script Issues**: Review error messages, check PostgreSQL version
- **EF Core Issues**: Ensure migrations match schema

---

**Need Help?** Start with [database-setup-guide.md](../docs/database-setup-guide.md)
