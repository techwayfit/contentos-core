-- ============================================================================
-- ContentOS Core Tables
-- Version: 1.0
-- Description: Tenant, Site, User, Role, Group management
-- ============================================================================

SET search_path TO contentos, public;

-- ============================================================================
-- TENANT (Top-level isolation boundary)
-- ============================================================================

CREATE TABLE IF NOT EXISTS tenants (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    key VARCHAR(100) NOT NULL,
    name VARCHAR(200) NOT NULL,
    status INTEGER NOT NULL DEFAULT 0, -- 0=Active, 1=Suspended, 2=Inactive
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ,
    created_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
 created_by UUID NOT NULL,
    updated_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_by UUID NOT NULL,
    deleted_on TIMESTAMPTZ,
    deleted_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    can_delete BOOLEAN NOT NULL DEFAULT TRUE,
    is_system BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT uq_tenants_key UNIQUE (key)
);

CREATE INDEX idx_tenants_status ON tenants(status) WHERE NOT is_deleted;
CREATE INDEX idx_tenants_active ON tenants(is_active, is_deleted) WHERE NOT is_deleted;

COMMENT ON TABLE tenants IS 'Top-level isolation boundary for multi-tenancy';
COMMENT ON COLUMN tenants.key IS 'Unique tenant slug (e.g., techwayfit, acme-corp)';
COMMENT ON COLUMN tenants.status IS '0=Active, 1=Suspended, 2=Inactive';

-- ============================================================================
-- SITE (Multi-site within tenant)
-- ============================================================================

CREATE TABLE IF NOT EXISTS sites (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    name VARCHAR(200) NOT NULL,
    host_name VARCHAR(200) NOT NULL,
    default_locale VARCHAR(10) NOT NULL DEFAULT 'en-US',
    created_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
  updated_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_by UUID NOT NULL,
    deleted_on TIMESTAMPTZ,
    deleted_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
can_delete BOOLEAN NOT NULL DEFAULT TRUE,
    is_system BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT fk_sites_tenant FOREIGN KEY (tenant_id) 
  REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT uq_sites_tenant_hostname UNIQUE (tenant_id, host_name)
);

CREATE INDEX idx_sites_tenant ON sites(tenant_id) WHERE NOT is_deleted;
CREATE INDEX idx_sites_hostname ON sites(host_name) WHERE NOT is_deleted;
CREATE INDEX idx_sites_active ON sites(tenant_id, is_active) WHERE NOT is_deleted;

COMMENT ON TABLE sites IS 'Multi-site support within a tenant (hostnames, locales, delivery scope)';
COMMENT ON COLUMN sites.host_name IS 'Site hostname (e.g., www.example.com)';

-- ============================================================================
-- USER (Identity records)
-- ============================================================================

CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    email VARCHAR(320) NOT NULL,
    display_name VARCHAR(200) NOT NULL,
    status INTEGER NOT NULL DEFAULT 0, -- 0=Active, 1=Inactive, 2=Locked
    created_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
  updated_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_by UUID NOT NULL,
    deleted_on TIMESTAMPTZ,
    deleted_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    can_delete BOOLEAN NOT NULL DEFAULT TRUE,
    is_system BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT fk_users_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT uq_users_tenant_email UNIQUE (tenant_id, email)
);

CREATE INDEX idx_users_tenant ON users(tenant_id) WHERE NOT is_deleted;
CREATE INDEX idx_users_email ON users(tenant_id, email) WHERE NOT is_deleted;
CREATE INDEX idx_users_status ON users(tenant_id, status) WHERE NOT is_deleted;

COMMENT ON TABLE users IS 'Core identity records (auth handled via IdP)';
COMMENT ON COLUMN users.email IS 'User email address (unique per tenant)';

-- ============================================================================
-- ROLE (RBAC)
-- ============================================================================

CREATE TABLE IF NOT EXISTS roles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    created_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    updated_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_by UUID NOT NULL,
 deleted_on TIMESTAMPTZ,
    deleted_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
  is_active BOOLEAN NOT NULL DEFAULT TRUE,
    can_delete BOOLEAN NOT NULL DEFAULT TRUE,
    is_system BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT fk_roles_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT uq_roles_tenant_name UNIQUE (tenant_id, name)
);

CREATE INDEX idx_roles_tenant ON roles(tenant_id) WHERE NOT is_deleted;
CREATE INDEX idx_roles_system ON roles(is_system) WHERE is_system = TRUE AND NOT is_deleted;

COMMENT ON TABLE roles IS 'Role-based access control (RBAC) definitions';

-- ============================================================================
-- GROUP (Directory-style grouping)
-- ============================================================================

CREATE TABLE IF NOT EXISTS groups (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    created_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    updated_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_by UUID NOT NULL,
    deleted_on TIMESTAMPTZ,
    deleted_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    can_delete BOOLEAN NOT NULL DEFAULT TRUE,
    is_system BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT fk_groups_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT uq_groups_tenant_name UNIQUE (tenant_id, name)
);

CREATE INDEX idx_groups_tenant ON groups(tenant_id) WHERE NOT is_deleted;

COMMENT ON TABLE groups IS 'User grouping for team-based permissions';

-- ============================================================================
-- USER_ROLE (Many-to-many: Users ? Roles)
-- ============================================================================

CREATE TABLE IF NOT EXISTS user_roles (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    user_id UUID NOT NULL,
    role_id UUID NOT NULL,
    created_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    updated_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_by UUID NOT NULL,
    deleted_on TIMESTAMPTZ,
    deleted_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
 is_active BOOLEAN NOT NULL DEFAULT TRUE,
 can_delete BOOLEAN NOT NULL DEFAULT TRUE,
    is_system BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT fk_user_roles_tenant FOREIGN KEY (tenant_id) 
    REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) 
        REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) 
  REFERENCES roles(id) ON DELETE CASCADE,
    CONSTRAINT uq_user_roles_tenant_user_role UNIQUE (tenant_id, user_id, role_id)
);

CREATE INDEX idx_user_roles_user ON user_roles(tenant_id, user_id) WHERE NOT is_deleted;
CREATE INDEX idx_user_roles_role ON user_roles(tenant_id, role_id) WHERE NOT is_deleted;

COMMENT ON TABLE user_roles IS 'Many-to-many mapping of users to roles';

-- ============================================================================
-- USER_GROUP (Many-to-many: Users ? Groups)
-- ============================================================================

CREATE TABLE IF NOT EXISTS user_groups (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
 user_id UUID NOT NULL,
  group_id UUID NOT NULL,
    created_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    updated_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_by UUID NOT NULL,
    deleted_on TIMESTAMPTZ,
    deleted_by UUID,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    can_delete BOOLEAN NOT NULL DEFAULT TRUE,
 is_system BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT fk_user_groups_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
  CONSTRAINT fk_user_groups_user FOREIGN KEY (user_id) 
 REFERENCES users(id) ON DELETE CASCADE,
    CONSTRAINT fk_user_groups_group FOREIGN KEY (group_id) 
     REFERENCES groups(id) ON DELETE CASCADE,
    CONSTRAINT uq_user_groups_tenant_user_group UNIQUE (tenant_id, user_id, group_id)
);

CREATE INDEX idx_user_groups_user ON user_groups(tenant_id, user_id) WHERE NOT is_deleted;
CREATE INDEX idx_user_groups_group ON user_groups(tenant_id, group_id) WHERE NOT is_deleted;

COMMENT ON TABLE user_groups IS 'Many-to-many mapping of users to groups';

-- ============================================================================
-- TRIGGERS
-- ============================================================================

CREATE TRIGGER trigger_tenants_updated_on BEFORE UPDATE ON tenants
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_sites_updated_on BEFORE UPDATE ON sites
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_users_updated_on BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_roles_updated_on BEFORE UPDATE ON roles
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_groups_updated_on BEFORE UPDATE ON groups
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();
