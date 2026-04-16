-- ============================================================================
-- ContentOS Layout & Module Tables
-- Version: 1.0
-- Description: Layout definitions, components, module management
-- ============================================================================

SET search_path TO contentos, public;

-- ============================================================================
-- LAYOUT_DEFINITION (Reusable layout templates)
-- ============================================================================

CREATE TABLE IF NOT EXISTS layout_definitions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    layout_key VARCHAR(200) NOT NULL,
    display_name VARCHAR(200) NOT NULL,
    regions_rules_json JSONB NOT NULL DEFAULT '{}',
    version INTEGER NOT NULL DEFAULT 1,
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
    CONSTRAINT fk_layout_definitions_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT uq_layout_definitions_tenant_key_version UNIQUE (tenant_id, layout_key, version)
);

CREATE INDEX idx_layout_definitions_tenant ON layout_definitions(tenant_id) WHERE NOT is_deleted;
CREATE INDEX idx_layout_definitions_key ON layout_definitions(tenant_id, layout_key) WHERE NOT is_deleted;

COMMENT ON TABLE layout_definitions IS 'Reusable layout template rules (regions + allowed components)';

-- ============================================================================
-- COMPONENT_DEFINITION (Component registry)
-- ============================================================================

CREATE TABLE IF NOT EXISTS component_definitions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    component_key VARCHAR(200) NOT NULL,
    display_name VARCHAR(200) NOT NULL,
    props_schema_json JSONB NOT NULL DEFAULT '{}',
    owner_module VARCHAR(100) NOT NULL,
    version INTEGER NOT NULL DEFAULT 1,
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
    CONSTRAINT fk_component_definitions_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT uq_component_definitions_tenant_key_version UNIQUE (tenant_id, component_key, version)
);

CREATE INDEX idx_component_definitions_tenant ON component_definitions(tenant_id) WHERE NOT is_deleted;
CREATE INDEX idx_component_definitions_module ON component_definitions(tenant_id, owner_module) 
    WHERE NOT is_deleted AND is_active;
CREATE INDEX idx_component_definitions_system ON component_definitions(is_system) 
    WHERE is_system = TRUE AND NOT is_deleted;

COMMENT ON TABLE component_definitions IS 'Component registry (module-owned components + prop schema)';

-- ============================================================================
-- CONTENT_LAYOUT (Composed layout per version)
-- ============================================================================

CREATE TABLE IF NOT EXISTS content_layouts (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    content_version_id UUID NOT NULL,
    layout_definition_id UUID,
    composition_json JSONB NOT NULL DEFAULT '{}',
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
    CONSTRAINT fk_content_layouts_tenant FOREIGN KEY (tenant_id) 
   REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_content_layouts_version FOREIGN KEY (content_version_id) 
  REFERENCES content_versions(id) ON DELETE CASCADE,
    CONSTRAINT fk_content_layouts_definition FOREIGN KEY (layout_definition_id) 
        REFERENCES layout_definitions(id) ON DELETE SET NULL,
    CONSTRAINT uq_content_layouts_tenant_version UNIQUE (tenant_id, content_version_id)
);

CREATE INDEX idx_content_layouts_version ON content_layouts(content_version_id) WHERE NOT is_deleted;
CREATE INDEX idx_content_layouts_definition ON content_layouts(tenant_id, layout_definition_id) 
    WHERE layout_definition_id IS NOT NULL AND NOT is_deleted;

COMMENT ON TABLE content_layouts IS 'Composed layout JSON per content version';

-- ============================================================================
-- MODULE (Module registry)
-- ============================================================================

CREATE TABLE IF NOT EXISTS modules (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    module_key VARCHAR(200) NOT NULL,
    display_name VARCHAR(200) NOT NULL,
    description TEXT,
    version VARCHAR(50) NOT NULL,
    publisher VARCHAR(200),
    license_type VARCHAR(100),
    category VARCHAR(100),
    installation_status VARCHAR(50) NOT NULL DEFAULT 'NotInstalled', 
    -- NotInstalled|Installing|Installed|Failed|Upgrading|Uninstalling
    installed_on TIMESTAMPTZ,
    installed_by UUID,
    dependencies_json JSONB NOT NULL DEFAULT '[]',
    schema_version INTEGER NOT NULL DEFAULT 1,
    api_version VARCHAR(50),
    requires_migration BOOLEAN NOT NULL DEFAULT FALSE,
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
  CONSTRAINT fk_modules_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT uq_modules_tenant_key UNIQUE (tenant_id, module_key)
);

CREATE INDEX idx_modules_tenant ON modules(tenant_id) WHERE NOT is_deleted;
CREATE INDEX idx_modules_status ON modules(tenant_id, installation_status) WHERE NOT is_deleted;
CREATE INDEX idx_modules_system ON modules(is_system) WHERE is_system = TRUE AND NOT is_deleted;

COMMENT ON TABLE modules IS 'Module registry and lifecycle management';

-- ============================================================================
-- MODULE_CAPABILITY (Feature flags per module)
-- ============================================================================

CREATE TABLE IF NOT EXISTS module_capabilities (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    module_id UUID NOT NULL,
    capability_key VARCHAR(200) NOT NULL,
    display_name VARCHAR(200) NOT NULL,
    is_enabled BOOLEAN NOT NULL DEFAULT TRUE,
    requires_license BOOLEAN NOT NULL DEFAULT FALSE,
    license_tier VARCHAR(50), -- Free|Pro|Enterprise
    created_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    updated_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_by UUID NOT NULL,
    CONSTRAINT fk_module_capabilities_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_module_capabilities_module FOREIGN KEY (module_id) 
        REFERENCES modules(id) ON DELETE CASCADE,
    CONSTRAINT uq_module_capabilities_tenant_module_key UNIQUE (tenant_id, module_id, capability_key)
);

CREATE INDEX idx_module_capabilities_module ON module_capabilities(tenant_id, module_id);
CREATE INDEX idx_module_capabilities_enabled ON module_capabilities(tenant_id, module_id, is_enabled) 
    WHERE is_enabled = TRUE;

COMMENT ON TABLE module_capabilities IS 'Feature flags and capabilities per module';

-- ============================================================================
-- MODULE_SETTING (Module configuration)
-- ============================================================================

CREATE TABLE IF NOT EXISTS module_settings (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    module_id UUID NOT NULL,
    site_id UUID, -- Nullable for site-specific settings
    setting_key VARCHAR(200) NOT NULL,
    setting_value JSONB NOT NULL,
    data_type VARCHAR(50) NOT NULL, -- string|number|boolean|json
  is_encrypted BOOLEAN NOT NULL DEFAULT FALSE,
  is_tenant_overridable BOOLEAN NOT NULL DEFAULT TRUE,
    created_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    updated_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_by UUID NOT NULL,
    CONSTRAINT fk_module_settings_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_module_settings_module FOREIGN KEY (module_id) 
        REFERENCES modules(id) ON DELETE CASCADE,
    CONSTRAINT fk_module_settings_site FOREIGN KEY (site_id) 
        REFERENCES sites(id) ON DELETE CASCADE,
    CONSTRAINT uq_module_settings_tenant_module_site_key UNIQUE (tenant_id, module_id, site_id, setting_key)
);

CREATE INDEX idx_module_settings_module ON module_settings(tenant_id, module_id);
CREATE INDEX idx_module_settings_site ON module_settings(tenant_id, module_id, site_id) 
    WHERE site_id IS NOT NULL;

COMMENT ON TABLE module_settings IS 'Module-specific configuration per tenant/site';

-- ============================================================================
-- MODULE_MIGRATION (Schema migration tracking)
-- ============================================================================

CREATE TABLE IF NOT EXISTS module_migrations (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    module_id UUID NOT NULL,
    migration_name VARCHAR(200) NOT NULL,
    from_version VARCHAR(50),
    to_version VARCHAR(50) NOT NULL,
    applied_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    applied_by UUID,
 status VARCHAR(50) NOT NULL, -- Pending|Running|Completed|Failed|RolledBack
    error_message TEXT,
    up_script TEXT,
    down_script TEXT,
    execution_time_ms BIGINT,
    CONSTRAINT fk_module_migrations_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_module_migrations_module FOREIGN KEY (module_id) 
        REFERENCES modules(id) ON DELETE CASCADE,
    CONSTRAINT uq_module_migrations_tenant_module_name UNIQUE (tenant_id, module_id, migration_name)
);

CREATE INDEX idx_module_migrations_module ON module_migrations(tenant_id, module_id, applied_on DESC);
CREATE INDEX idx_module_migrations_status ON module_migrations(tenant_id, module_id, status);

COMMENT ON TABLE module_migrations IS 'Tracks module schema migrations';

-- ============================================================================
-- TRIGGERS
-- ============================================================================

CREATE TRIGGER trigger_layout_definitions_updated_on BEFORE UPDATE ON layout_definitions
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_component_definitions_updated_on BEFORE UPDATE ON component_definitions
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_content_layouts_updated_on BEFORE UPDATE ON content_layouts
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_modules_updated_on BEFORE UPDATE ON modules
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_module_capabilities_updated_on BEFORE UPDATE ON module_capabilities
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_module_settings_updated_on BEFORE UPDATE ON module_settings
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();
