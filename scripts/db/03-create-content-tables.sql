-- ============================================================================
-- ContentOS Content Management Tables
-- Version: 1.0
-- Description: Content types, items, versions, fields, nodes, routes
-- ============================================================================

SET search_path TO contentos, public;

-- ============================================================================
-- CONTENT_TYPE (Schema registry)
-- ============================================================================

CREATE TABLE IF NOT EXISTS content_types (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    type_key VARCHAR(200) NOT NULL, -- e.g., page.article
display_name VARCHAR(200) NOT NULL,
    schema_version INTEGER NOT NULL DEFAULT 1,
    settings_json JSONB NOT NULL DEFAULT '{}',
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
    CONSTRAINT fk_content_types_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT uq_content_types_tenant_key UNIQUE (tenant_id, type_key)
);

CREATE INDEX idx_content_types_tenant ON content_types(tenant_id) WHERE NOT is_deleted;
CREATE INDEX idx_content_types_system ON content_types(is_system) WHERE is_system = TRUE;

COMMENT ON TABLE content_types IS 'Content type definitions (Contentful-like schema registry)';

-- ============================================================================
-- CONTENT_TYPE_FIELD (Field definitions)
-- ============================================================================

CREATE TABLE IF NOT EXISTS content_type_fields (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
 tenant_id UUID NOT NULL,
    content_type_id UUID NOT NULL,
    field_key VARCHAR(200) NOT NULL,
    data_type VARCHAR(50) NOT NULL, -- string|richtext|number|bool|datetime|ref|json
    is_required BOOLEAN NOT NULL DEFAULT FALSE,
    is_localized BOOLEAN NOT NULL DEFAULT FALSE,
    constraints_json JSONB NOT NULL DEFAULT '{}',
    sort_order INTEGER NOT NULL DEFAULT 0,
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
  CONSTRAINT fk_content_type_fields_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_content_type_fields_type FOREIGN KEY (content_type_id) 
      REFERENCES content_types(id) ON DELETE CASCADE,
    CONSTRAINT uq_content_type_fields_tenant_type_key UNIQUE (tenant_id, content_type_id, field_key)
);

CREATE INDEX idx_content_type_fields_type ON content_type_fields(content_type_id) WHERE NOT is_deleted;
CREATE INDEX idx_content_type_fields_tenant_type ON content_type_fields(tenant_id, content_type_id, sort_order) 
    WHERE NOT is_deleted;

COMMENT ON TABLE content_type_fields IS 'Field definitions for each content type';

-- ============================================================================
-- CONTENT_NODE (Content hierarchy/tree)
-- ============================================================================

CREATE TABLE IF NOT EXISTS content_nodes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    site_id UUID NOT NULL,
    parent_id UUID,
    node_type VARCHAR(20) NOT NULL, -- Folder|Item|Link|Mount
    content_item_id UUID,
    title VARCHAR(500) NOT NULL,
    path VARCHAR(2000) NOT NULL,
    sort_order INTEGER NOT NULL DEFAULT 0,
    inherit_acl BOOLEAN NOT NULL DEFAULT TRUE,
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
    CONSTRAINT fk_content_nodes_tenant FOREIGN KEY (tenant_id) 
  REFERENCES tenants(id) ON DELETE CASCADE,
 CONSTRAINT fk_content_nodes_site FOREIGN KEY (site_id) 
        REFERENCES sites(id) ON DELETE CASCADE,
    CONSTRAINT fk_content_nodes_parent FOREIGN KEY (parent_id) 
        REFERENCES content_nodes(id) ON DELETE RESTRICT,
    CONSTRAINT uq_content_nodes_tenant_site_path UNIQUE (tenant_id, site_id, path)
);

CREATE INDEX idx_content_nodes_tenant_site ON content_nodes(tenant_id, site_id) WHERE NOT is_deleted;
CREATE INDEX idx_content_nodes_parent ON content_nodes(tenant_id, site_id, parent_id, sort_order) 
    WHERE NOT is_deleted AND is_active;
CREATE INDEX idx_content_nodes_item ON content_nodes(tenant_id, site_id, content_item_id) 
 WHERE content_item_id IS NOT NULL AND NOT is_deleted;
CREATE INDEX idx_content_nodes_path ON content_nodes(tenant_id, site_id, path) 
    WHERE NOT is_deleted;

COMMENT ON TABLE content_nodes IS 'Content hierarchy (tree structure like Sitecore)';
COMMENT ON COLUMN content_nodes.node_type IS 'Folder|Item|Link|Mount';

-- ============================================================================
-- ROUTE (Delivery routing)
-- ============================================================================

CREATE TABLE IF NOT EXISTS routes (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    site_id UUID NOT NULL,
    node_id UUID NOT NULL,
    route_path VARCHAR(2000) NOT NULL,
    is_primary BOOLEAN NOT NULL DEFAULT TRUE,
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
    CONSTRAINT fk_routes_tenant FOREIGN KEY (tenant_id) 
REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_routes_site FOREIGN KEY (site_id) 
 REFERENCES sites(id) ON DELETE CASCADE,
    CONSTRAINT fk_routes_node FOREIGN KEY (node_id) 
        REFERENCES content_nodes(id) ON DELETE CASCADE,
    CONSTRAINT uq_routes_tenant_site_path UNIQUE (tenant_id, site_id, route_path)
);

CREATE INDEX idx_routes_tenant_site ON routes(tenant_id, site_id, route_path) WHERE NOT is_deleted;
CREATE INDEX idx_routes_node ON routes(tenant_id, site_id, node_id) WHERE NOT is_deleted;
CREATE INDEX idx_routes_primary ON routes(tenant_id, site_id, route_path) 
    WHERE is_primary = TRUE AND NOT is_deleted;

COMMENT ON TABLE routes IS 'Delivery routing (friendly URLs to nodes)';

-- ============================================================================
-- CONTENT_ITEM (Content instances)
-- ============================================================================

CREATE TABLE IF NOT EXISTS content_items (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    site_id UUID NOT NULL,
    content_type_id UUID NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'active', -- active|archived
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
    CONSTRAINT fk_content_items_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_content_items_site FOREIGN KEY (site_id) 
  REFERENCES sites(id) ON DELETE CASCADE,
    CONSTRAINT fk_content_items_type FOREIGN KEY (content_type_id) 
        REFERENCES content_types(id) ON DELETE RESTRICT
);

CREATE INDEX idx_content_items_tenant_site ON content_items(tenant_id, site_id, content_type_id) 
    WHERE NOT is_deleted AND is_active;
CREATE INDEX idx_content_items_status ON content_items(tenant_id, site_id, status) 
    WHERE NOT is_deleted;
CREATE INDEX idx_content_items_deleted ON content_items(tenant_id, deleted_on) 
    WHERE deleted_on IS NOT NULL;

COMMENT ON TABLE content_items IS 'Content instances (stable identity, fields in versions)';

-- ============================================================================
-- CONTENT_VERSION (Versioned lifecycle)
-- ============================================================================

CREATE TABLE IF NOT EXISTS content_versions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    content_item_id UUID NOT NULL,
    version_number INTEGER NOT NULL,
    lifecycle VARCHAR(50) NOT NULL, -- draft|review|published|archived
    workflow_state_id UUID,
    published_at TIMESTAMPTZ,
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
 CONSTRAINT fk_content_versions_tenant FOREIGN KEY (tenant_id) 
     REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_content_versions_item FOREIGN KEY (content_item_id) 
        REFERENCES content_items(id) ON DELETE CASCADE,
    CONSTRAINT uq_content_versions_tenant_item_number UNIQUE (tenant_id, content_item_id, version_number)
);

CREATE INDEX idx_content_versions_item ON content_versions(tenant_id, content_item_id, version_number DESC) 
    WHERE NOT is_deleted;
CREATE INDEX idx_content_versions_published ON content_versions(tenant_id, content_item_id, published_at DESC) 
  WHERE published_at IS NOT NULL AND NOT is_deleted;
CREATE INDEX idx_content_versions_lifecycle ON content_versions(tenant_id, lifecycle) 
  WHERE NOT is_deleted;
CREATE INDEX idx_content_versions_workflow ON content_versions(tenant_id, workflow_state_id) 
    WHERE workflow_state_id IS NOT NULL AND NOT is_deleted;

COMMENT ON TABLE content_versions IS 'Versioned content lifecycle (draft|review|published|archived)';

-- ============================================================================
-- CONTENT_FIELD_VALUE (Field storage)
-- ============================================================================

CREATE TABLE IF NOT EXISTS content_field_values (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    content_version_id UUID NOT NULL,
    field_key VARCHAR(200) NOT NULL,
    locale VARCHAR(10) NOT NULL DEFAULT '',
    value_json JSONB NOT NULL,
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
    CONSTRAINT fk_content_field_values_tenant FOREIGN KEY (tenant_id) 
    REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_content_field_values_version FOREIGN KEY (content_version_id) 
        REFERENCES content_versions(id) ON DELETE CASCADE,
    CONSTRAINT uq_content_field_values_tenant_version_field_locale 
        UNIQUE (tenant_id, content_version_id, field_key, locale)
);

CREATE INDEX idx_content_field_values_version ON content_field_values(tenant_id, content_version_id) 
    WHERE NOT is_deleted;
CREATE INDEX idx_content_field_values_version_field ON content_field_values(tenant_id, content_version_id, field_key, locale) 
    WHERE NOT is_deleted;
-- Optional: GIN index for JSON queries (only if frequently querying inside JSON)
-- CREATE INDEX idx_content_field_values_json ON content_field_values USING GIN (value_json jsonb_path_ops) 
--  WHERE NOT is_deleted;

COMMENT ON TABLE content_field_values IS 'Content field values per version (supports localization)';

-- ============================================================================
-- TRIGGERS
-- ============================================================================

CREATE TRIGGER trigger_content_types_updated_on BEFORE UPDATE ON content_types
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_content_type_fields_updated_on BEFORE UPDATE ON content_type_fields
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_content_nodes_updated_on BEFORE UPDATE ON content_nodes
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_routes_updated_on BEFORE UPDATE ON routes
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_content_items_updated_on BEFORE UPDATE ON content_items
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_content_versions_updated_on BEFORE UPDATE ON content_versions
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_content_field_values_updated_on BEFORE UPDATE ON content_field_values
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();
