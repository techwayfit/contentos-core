-- ============================================================================
-- ContentOS Workflow & Security Tables
-- Version: 1.0
-- Description: Workflow definitions, states, transitions, ACL, preview tokens
-- ============================================================================

SET search_path TO contentos, public;

-- ============================================================================
-- WORKFLOW_DEFINITION (Workflow graphs)
-- ============================================================================

CREATE TABLE IF NOT EXISTS workflow_definitions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    workflow_key VARCHAR(200) NOT NULL,
    display_name VARCHAR(200) NOT NULL,
    is_default BOOLEAN NOT NULL DEFAULT FALSE,
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
    CONSTRAINT fk_workflow_definitions_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT uq_workflow_definitions_tenant_key UNIQUE (tenant_id, workflow_key)
);

CREATE INDEX idx_workflow_definitions_tenant ON workflow_definitions(tenant_id) WHERE NOT is_deleted;
CREATE INDEX idx_workflow_definitions_default ON workflow_definitions(tenant_id, is_default) 
    WHERE is_default = TRUE AND NOT is_deleted;
CREATE INDEX idx_workflow_definitions_system ON workflow_definitions(is_system) 
    WHERE is_system = TRUE AND NOT is_deleted;

COMMENT ON TABLE workflow_definitions IS 'Workflow graph definitions (e.g., Draft → Review → Publish)';

-- ============================================================================
-- WORKFLOW_STATE (States within workflow)
-- ============================================================================

CREATE TABLE IF NOT EXISTS workflow_states (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    workflow_definition_id UUID NOT NULL,
    state_key VARCHAR(200) NOT NULL,
    display_name VARCHAR(200) NOT NULL,
    is_terminal BOOLEAN NOT NULL DEFAULT FALSE,
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
    CONSTRAINT fk_workflow_states_tenant FOREIGN KEY (tenant_id) 
    REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_workflow_states_definition FOREIGN KEY (workflow_definition_id) 
        REFERENCES workflow_definitions(id) ON DELETE CASCADE,
    CONSTRAINT uq_workflow_states_tenant_definition_key UNIQUE (tenant_id, workflow_definition_id, state_key)
);

CREATE INDEX idx_workflow_states_definition ON workflow_states(workflow_definition_id) WHERE NOT is_deleted;
CREATE INDEX idx_workflow_states_tenant_definition ON workflow_states(tenant_id, workflow_definition_id) 
    WHERE NOT is_deleted;

COMMENT ON TABLE workflow_states IS 'Workflow states within a definition';

-- ============================================================================
-- WORKFLOW_TRANSITION (Allowed transitions)
-- ============================================================================

CREATE TABLE IF NOT EXISTS workflow_transitions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  tenant_id UUID NOT NULL,
    workflow_definition_id UUID NOT NULL,
    from_state_id UUID NOT NULL,
    to_state_id UUID NOT NULL,
    required_action VARCHAR(200) NOT NULL,
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
    CONSTRAINT fk_workflow_transitions_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_workflow_transitions_definition FOREIGN KEY (workflow_definition_id) 
        REFERENCES workflow_definitions(id) ON DELETE CASCADE,
    CONSTRAINT fk_workflow_transitions_from FOREIGN KEY (from_state_id) 
      REFERENCES workflow_states(id) ON DELETE CASCADE,
    CONSTRAINT fk_workflow_transitions_to FOREIGN KEY (to_state_id) 
 REFERENCES workflow_states(id) ON DELETE CASCADE,
    CONSTRAINT uq_workflow_transitions_tenant_definition_from_to 
        UNIQUE (tenant_id, workflow_definition_id, from_state_id, to_state_id)
);

CREATE INDEX idx_workflow_transitions_definition ON workflow_transitions(workflow_definition_id) 
    WHERE NOT is_deleted;
CREATE INDEX idx_workflow_transitions_from ON workflow_transitions(tenant_id, workflow_definition_id, from_state_id) 
    WHERE NOT is_deleted;

COMMENT ON TABLE workflow_transitions IS 'Allowed transitions between states with required permissions';

-- ============================================================================
-- ACL_ENTRY (Fine-grained permissions)
-- ============================================================================

CREATE TABLE IF NOT EXISTS acl_entries (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    scope_type VARCHAR(50) NOT NULL, -- Tenant|Site|Node|ContentType
    scope_id UUID NOT NULL,
    principal_type VARCHAR(50) NOT NULL, -- User|Role|Group
    principal_id UUID NOT NULL,
    effect VARCHAR(20) NOT NULL, -- Allow|Deny
    actions_csv TEXT NOT NULL, -- Read,Edit,Publish,...
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
    CONSTRAINT fk_acl_entries_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE
);

-- Scope-first indexes (active ACLs only)
CREATE INDEX idx_acl_entries_scope ON acl_entries(tenant_id, scope_type, scope_id) 
    WHERE NOT is_deleted AND is_active;
CREATE INDEX idx_acl_entries_scope_principal ON acl_entries(tenant_id, scope_type, scope_id, principal_type, principal_id) 
    WHERE NOT is_deleted AND is_active;

-- Principal-first indexes
CREATE INDEX idx_acl_entries_principal ON acl_entries(tenant_id, principal_type, principal_id) 
    WHERE NOT is_deleted AND is_active;

-- By effect (for optimization)
CREATE INDEX idx_acl_entries_allow ON acl_entries(tenant_id, scope_type, scope_id, principal_type, principal_id) 
    WHERE effect='Allow' AND NOT is_deleted AND is_active;
CREATE INDEX idx_acl_entries_deny ON acl_entries(tenant_id, scope_type, scope_id, principal_type, principal_id) 
    WHERE effect='Deny' AND NOT is_deleted AND is_active;

-- System ACLs (protected)
CREATE INDEX idx_acl_entries_system ON acl_entries(tenant_id, is_system) 
    WHERE is_system = TRUE AND NOT is_deleted;

COMMENT ON TABLE acl_entries IS 'Fine-grained permissions on scopes with inheritance';
COMMENT ON COLUMN acl_entries.scope_type IS 'Tenant|Site|Node|ContentType';
COMMENT ON COLUMN acl_entries.principal_type IS 'User|Role|Group';
COMMENT ON COLUMN acl_entries.effect IS 'Allow|Deny';

-- ============================================================================
-- PREVIEW_TOKEN (Secure preview links)
-- ============================================================================

CREATE TABLE IF NOT EXISTS preview_tokens (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
 tenant_id UUID NOT NULL,
    site_id UUID NOT NULL,
    node_id UUID NOT NULL,
    content_version_id UUID NOT NULL,
    token_hash VARCHAR(256) NOT NULL, -- HMAC-SHA256 hash
 expires_at TIMESTAMPTZ NOT NULL,
 issued_to_email VARCHAR(320),
    one_time_use BOOLEAN NOT NULL DEFAULT FALSE,
  used_at TIMESTAMPTZ,
    created_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_by UUID NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    CONSTRAINT fk_preview_tokens_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE,
    CONSTRAINT fk_preview_tokens_site FOREIGN KEY (site_id) 
      REFERENCES sites(id) ON DELETE CASCADE,
    CONSTRAINT fk_preview_tokens_node FOREIGN KEY (node_id) 
        REFERENCES content_nodes(id) ON DELETE CASCADE,
    CONSTRAINT fk_preview_tokens_version FOREIGN KEY (content_version_id) 
        REFERENCES content_versions(id) ON DELETE CASCADE,
    CONSTRAINT uq_preview_tokens_tenant_hash UNIQUE (tenant_id, token_hash)
);

CREATE INDEX idx_preview_tokens_tenant_hash ON preview_tokens(tenant_id, token_hash) 
    WHERE is_active = TRUE;
CREATE INDEX idx_preview_tokens_expires ON preview_tokens(tenant_id, expires_at) 
    WHERE is_active = TRUE;
CREATE INDEX idx_preview_tokens_version ON preview_tokens(tenant_id, content_version_id) 
    WHERE is_active = TRUE;
CREATE INDEX idx_preview_tokens_node ON preview_tokens(tenant_id, site_id, node_id) 
    WHERE is_active = TRUE;
-- Index for cleanup of expired tokens (simple index on expires_at for sorting/filtering)
CREATE INDEX idx_preview_tokens_cleanup ON preview_tokens(expires_at);

COMMENT ON TABLE preview_tokens IS 'Secure preview links (time-bound, optional one-time)';
COMMENT ON COLUMN preview_tokens.token_hash IS 'HMAC-SHA256 hash of the random token (never store raw token)';

-- ============================================================================
-- AUDIT_LOG (Append-only audit trail)
-- ============================================================================

CREATE TABLE IF NOT EXISTS audit_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenant_id UUID NOT NULL,
    actor_user_id UUID, -- Nullable, preserves audit even if user deleted
    action_key VARCHAR(200) NOT NULL, -- e.g., content.published, acl.modified
    entity_type VARCHAR(100) NOT NULL,
    entity_id UUID NOT NULL,
    details_json JSONB NOT NULL DEFAULT '{}',
    created_on TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_audit_logs_tenant FOREIGN KEY (tenant_id) 
        REFERENCES tenants(id) ON DELETE CASCADE
);

-- Primary audit queries
CREATE INDEX idx_audit_logs_tenant_created ON audit_logs(tenant_id, created_on DESC);
CREATE INDEX idx_audit_logs_actor ON audit_logs(tenant_id, actor_user_id, created_on DESC) 
    WHERE actor_user_id IS NOT NULL;
CREATE INDEX idx_audit_logs_entity ON audit_logs(tenant_id, entity_type, entity_id, created_on DESC);
CREATE INDEX idx_audit_logs_action ON audit_logs(tenant_id, action_key, created_on DESC);

COMMENT ON TABLE audit_logs IS 'Append-only audit trail (no updates or deletes)';

-- ============================================================================
-- TRIGGERS
-- ============================================================================

CREATE TRIGGER trigger_workflow_definitions_updated_on BEFORE UPDATE ON workflow_definitions
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_workflow_states_updated_on BEFORE UPDATE ON workflow_states
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_workflow_transitions_updated_on BEFORE UPDATE ON workflow_transitions
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

CREATE TRIGGER trigger_acl_entries_updated_on BEFORE UPDATE ON acl_entries
    FOR EACH ROW EXECUTE FUNCTION update_updated_on_column();

-- Note: audit_logs is append-only, no update trigger needed
-- Note: preview_tokens doesn't have updated_on, no trigger needed
