# ContentOS PostgreSQL Database Setup - Summary

## ? Created Files

### Database Scripts

1. **`scripts/db/01-setup-database.sql`**
   - Database creation
   - Extensions: uuid-ossp, pg_trgm, btree_gin, (pgvector optional)
   - Helper functions: `update_updated_on_column()`, `set_deleted_on()`
   - Schema setup

2. **`scripts/db/02-create-core-tables.sql`**
   - `tenants` - Multi-tenancy foundation
   - `sites` - Multi-site support
   - `users` - Identity records
   - `roles` - RBAC
   - `groups` - User grouping
   - `user_roles` - User ? Role mapping
   - `user_groups` - User ? Group mapping
   - Triggers for all tables

3. **`scripts/db/03-create-content-tables.sql`**
   - `content_types` - Schema registry
   - `content_type_fields` - Field definitions
   - `content_nodes` - Content hierarchy (Sitecore-like tree)
   - `routes` - URL routing
   - `content_items` - Content instances
   - `content_versions` - Versioned content lifecycle
   - `content_field_values` - Field storage with localization
   - Performance indexes for queries

4. **`scripts/db/04-create-workflow-security-tables.sql`**
   - `workflow_definitions` - Workflow graphs
   - `workflow_states` - Workflow states
   - `workflow_transitions` - Transition rules
   - `acl_entries` - Fine-grained permissions
   - `preview_tokens` - Secure preview links (hashed)
   - `audit_logs` - Append-only audit trail

5. **`scripts/db/05-create-layout-module-tables.sql`**
   - `layout_definitions` - Layout templates
   - `component_definitions` - Component registry
   - `content_layouts` - Composed layouts
   - `modules` - Module lifecycle
   - `module_capabilities` - Feature flags
   - `module_settings` - Configuration
 - `module_migrations` - Migration tracking

6. **`scripts/db/06-seed-data.sql`**
   - System tenant (`system`)
   - Demo tenant (`demo`)
   - Demo site (`localhost`)
   - Default roles (SuperAdmin, TenantAdmin, Editor, Publisher, Viewer)
   - Demo admin user (`admin@demo.local`)
   - Default workflow (Draft ? Review ? Published)
   - Basic page content type

### Setup Scripts

7. **`scripts/db/setup-database.sh`** (Linux/macOS)
   - Automated setup with error handling
   - Environment variable configuration
   - Connection validation
   - Sequential script execution
   - Color-coded output

8. **`scripts/db/setup-database.bat`** (Windows)
   - Windows equivalent of setup script
   - Same functionality as shell script
   - Batch file syntax for Windows

9. **`scripts/db/README.md`**
   - Comprehensive documentation
   - Quick start guide
   - Configuration instructions
   - Troubleshooting section
   - Verification queries
   - Clean up procedures

## ?? Key Features

### Architecture Compliance

? **Multi-Tenancy**: Every table (except `tenants`) includes `tenant_id` with enforced filtering
? **Soft Delete**: All entities support soft delete with `is_deleted`, `deleted_on`, `deleted_by`
? **Audit Trail**: Full audit fields (`created_on`, `created_by`, `updated_on`, `updated_by`)
? **System Entities**: `is_system` flag prevents deletion of platform-owned data
? **Snake Case**: All column names use `snake_case` convention
? **Unique Constraints**: Tenant-scoped uniqueness on all key fields

### Performance Optimizations

? **Partial Indexes**: `WHERE NOT is_deleted` for active record queries
? **Composite Indexes**: Multi-column indexes for common query patterns
? **GIN Indexes**: JSONB support (optional, commented for performance)
? **Foreign Keys**: Proper cascading rules (CASCADE, RESTRICT, SET NULL)
? **Query Filters**: Optimized for tenant-scoped queries

### Security Features

? **Preview Tokens**: HMAC-SHA256 hashed (never stores raw tokens)
? **ACL System**: Fine-grained permissions with inheritance
? **Audit Logs**: Append-only (no updates/deletes)
? **Workflow**: State-based content lifecycle
? **Protected Entities**: `can_delete=false` for critical records

## ?? Database Statistics

- **Total Tables**: 25+
- **Total Indexes**: 100+
- **Total Foreign Keys**: 40+
- **Total Triggers**: 20+
- **Seed Records**: 20+

## ?? Alignment with EF Core

The scripts align with your existing EF Core configurations:

- Table names match `NamingConventions.ToTableName()`
- Column names match configuration extensions (`ConfigureAuditFields`, `ConfigureTenantKey`)
- Foreign keys match navigation properties
- Indexes match your performance requirements
- Constraints match domain rules

## ?? Usage Examples

### Setup Database

```bash
# Linux/macOS
cd scripts/db
chmod +x setup-database.sh
./setup-database.sh

# Windows
cd scripts\db
setup-database.bat
```

### Custom Configuration

```bash
# Set environment variables
export DB_NAME=my_contentos_db
export DB_USER=myuser
export DB_HOST=db.example.com
export DB_PORT=5432
export PGPASSWORD=secure_password

# Run setup
./setup-database.sh
```

### Verify Installation

```sql
-- Check tenant setup
SELECT * FROM contentos.tenants;

-- Check roles
SELECT * FROM contentos.roles WHERE tenant_id = '11111111-1111-1111-1111-111111111111'::UUID;

-- Check workflow
SELECT 
    wd.workflow_key,
    ws.state_key,
    ws.display_name,
    ws.is_terminal
FROM contentos.workflow_definitions wd
JOIN contentos.workflow_states ws ON ws.workflow_definition_id = wd.id
WHERE wd.is_default = TRUE
ORDER BY ws.state_key;

-- Check content type
SELECT 
 ct.type_key,
    ct.display_name,
    ctf.field_key,
    ctf.data_type,
    ctf.is_required,
    ctf.is_localized
FROM contentos.content_types ct
JOIN contentos.content_type_fields ctf ON ctf.content_type_id = ct.id
WHERE ct.type_key = 'page.basic';
```

## ?? What You Get

After running the setup:

1. **Fully configured PostgreSQL database** with all tables, indexes, and constraints
2. **Demo tenant** ready for testing with:
   - Admin user
   - Default roles
   - Basic workflow
 - Sample content type
3. **Production-ready schema** following all architectural rules
4. **Performance optimized** with proper indexes
5. **Security compliant** with ACL, audit, and soft delete

## ?? Next Steps

1. **Update Connection String** in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=contentos_core;Username=postgres;Password=YOUR_PASSWORD"
     }
   }
   ```

2. **Run Application**:
   ```bash
   cd src/delivery/api/TechWayFit.ContentOS.Api
   dotnet run
   ```

3. **Test API** with demo tenant:
   - Add header: `X-Tenant-Id: 11111111-1111-1111-1111-111111111111`
   - Access endpoints at `http://localhost:5000/api/*`

4. **Create Content**:
   ```bash
   curl -X POST http://localhost:5000/api/content \
 -H "X-Tenant-Id: 11111111-1111-1111-1111-111111111111" \
     -H "Content-Type: application/json" \
  -d '{
       "contentTypeId": "11111111-1111-1111-1111-111111111120",
       "title": "My First Page",
       "fields": {
  "title": "Welcome to ContentOS",
"body": "<p>Hello World</p>"
       }
     }'
   ```

## ?? Notes

- Scripts are **idempotent** (safe to run multiple times)
- Uses `ON CONFLICT DO NOTHING` for seed data
- All GUIDs are hardcoded for demo data reproducibility
- Triggers automatically update `updated_on` timestamps
- Foreign keys use appropriate cascade rules
- Indexes optimized for tenant-scoped queries

## ?? Troubleshooting

See `scripts/db/README.md` for detailed troubleshooting guide.

Common issues:
- PostgreSQL not running ? Start service
- Permission denied ? Grant CREATEDB privilege
- Extension not found ? Install postgresql-contrib
- Connection refused ? Check host/port/credentials

## ? Benefits of These Scripts

1. **Development Speed**: Instant database setup for new developers
2. **Consistency**: Same schema across all environments
3. **Documentation**: Scripts serve as executable documentation
4. **CI/CD Ready**: Can be integrated into automated pipelines
5. **Testing**: Quick database reset for integration tests
6. **Production**: Reference for manual provisioning if needed

---

**Total Time to Setup**: < 2 minutes
**Total Lines of SQL**: ~2,000+
**Fully Compliant with**: ContentOS architectural rules (.github/copilot-instructions.md)
