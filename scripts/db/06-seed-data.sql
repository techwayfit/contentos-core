-- ============================================================================
-- ContentOS Seed Data
-- Version: 1.0
-- Description: Initial data for system setup (SuperAdmin tenant, default roles)
-- ============================================================================

SET search_path TO contentos, public;

-- ============================================================================
-- SEED TENANT (System Tenant)
-- ============================================================================

-- Insert system tenant (for platform-wide data)
INSERT INTO tenants (
 id,
    key,
    name,
    status,
    created_at,
    updated_at,
    created_on,
    created_by,
    updated_on,
    updated_by,
    is_deleted,
    is_active,
    can_delete,
    is_system
) VALUES (
    '00000000-0000-0000-0000-000000000001'::UUID,
    'system',
    'System Tenant',
    0,
    NOW(),
NOW(),
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
 '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE, -- System tenant cannot be deleted
    TRUE
) ON CONFLICT (id) DO NOTHING;

-- Insert demo tenant
INSERT INTO tenants (
    id,
    key,
    name,
    status,
    created_at,
    updated_at,
    created_on,
    created_by,
    updated_on,
    updated_by,
    is_deleted,
 is_active,
    can_delete,
    is_system
) VALUES (
    '11111111-1111-1111-1111-111111111111'::UUID,
    'demo',
    'Demo Tenant',
    0,
    NOW(),
    NOW(),
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
 NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
TRUE,
    TRUE,
    FALSE
) ON CONFLICT (id) DO NOTHING;

-- ============================================================================
-- SEED SITE (Demo site)
-- ============================================================================

INSERT INTO sites (
    id,
    tenant_id,
    name,
    host_name,
    default_locale,
    created_on,
    created_by,
    updated_on,
    updated_by,
 is_deleted,
    is_active,
    can_delete,
    is_system
) VALUES (
  '11111111-1111-1111-1111-111111111112'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    'Demo Site',
    'localhost',
    'en-US',
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    TRUE,
    FALSE
) ON CONFLICT (id) DO NOTHING;

-- ============================================================================
-- SEED ROLES (System-wide default roles)
-- ============================================================================

-- SuperAdmin role (system-wide)
INSERT INTO roles (
    id,
    tenant_id,
 name,
    description,
    created_on,
    created_by,
    updated_on,
    updated_by,
    is_deleted,
    is_active,
    can_delete,
    is_system
) VALUES (
    '00000000-0000-0000-0000-000000000010'::UUID,
 '00000000-0000-0000-0000-000000000001'::UUID,
    'SuperAdmin',
    'Platform super administrator with full access',
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
  FALSE,
TRUE,
    FALSE,
    TRUE
) ON CONFLICT (id) DO NOTHING;

-- Demo tenant roles
INSERT INTO roles (
    id,
 tenant_id,
    name,
    description,
    created_on,
    created_by,
    updated_on,
    updated_by,
    is_deleted,
    is_active,
 can_delete,
    is_system
) VALUES 
(
    '11111111-1111-1111-1111-111111111113'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    'TenantAdmin',
    'Tenant administrator',
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
    TRUE
),
(
 '11111111-1111-1111-1111-111111111114'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    'Editor',
    'Content editor',
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
  '00000000-0000-0000-0000-000000000000'::UUID,
 FALSE,
    TRUE,
    TRUE,
    FALSE
),
(
    '11111111-1111-1111-1111-111111111115'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    'Publisher',
    'Content publisher',
  NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    TRUE,
 FALSE
),
(
    '11111111-1111-1111-1111-111111111116'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    'Viewer',
    'Read-only viewer',
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    TRUE,
    FALSE
)
ON CONFLICT (id) DO NOTHING;

-- ============================================================================
-- SEED USER (Demo admin user)
-- ============================================================================

INSERT INTO users (
    id,
    tenant_id,
    email,
    display_name,
    status,
    created_on,
    created_by,
    updated_on,
    updated_by,
    is_deleted,
    is_active,
    can_delete,
    is_system
) VALUES (
    '11111111-1111-1111-1111-111111111117'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    'admin@demo.local',
    'Demo Admin',
    0,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
 FALSE,
    TRUE
) ON CONFLICT (id) DO NOTHING;

-- ============================================================================
-- SEED USER_ROLE (Assign admin role)
-- ============================================================================

INSERT INTO user_roles (
 id,
    tenant_id,
    user_id,
    role_id,
    created_on,
    created_by,
    updated_on,
    updated_by,
    is_deleted,
    is_active,
    can_delete,
    is_system
) VALUES (
    '11111111-1111-1111-1111-111111111118'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    '11111111-1111-1111-1111-111111111117'::UUID,
    '11111111-1111-1111-1111-111111111113'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
    TRUE
) ON CONFLICT (id) DO NOTHING;

-- ============================================================================
-- SEED WORKFLOW (Default workflow)
-- ============================================================================

INSERT INTO workflow_definitions (
    id,
    tenant_id,
    workflow_key,
    display_name,
    is_default,
    created_on,
    created_by,
    updated_on,
    updated_by,
    is_deleted,
    is_active,
    can_delete,
    is_system
) VALUES (
    '11111111-1111-1111-1111-111111111119'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    'default',
    'Default Workflow',
    TRUE,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
    TRUE
) ON CONFLICT (id) DO NOTHING;

-- Workflow States
INSERT INTO workflow_states (
    id,
    tenant_id,
    workflow_definition_id,
    state_key,
    display_name,
    is_terminal,
    created_on,
    created_by,
    updated_on,
    updated_by,
    is_deleted,
    is_active,
    can_delete,
    is_system
) VALUES 
(
    '11111111-1111-1111-1111-11111111111A'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    '11111111-1111-1111-1111-111111111119'::UUID,
    'draft',
    'Draft',
    FALSE,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
 '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
    TRUE
),
(
    '11111111-1111-1111-1111-11111111111B'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    '11111111-1111-1111-1111-111111111119'::UUID,
    'review',
    'In Review',
    FALSE,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
  NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
    TRUE
),
(
    '11111111-1111-1111-1111-11111111111C'::UUID,
  '11111111-1111-1111-1111-111111111111'::UUID,
    '11111111-1111-1111-1111-111111111119'::UUID,
  'published',
    'Published',
    TRUE,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
    TRUE
)
ON CONFLICT (id) DO NOTHING;

-- Workflow Transitions
INSERT INTO workflow_transitions (
    id,
    tenant_id,
    workflow_definition_id,
    from_state_id,
    to_state_id,
    required_action,
    created_on,
    created_by,
    updated_on,
    updated_by,
    is_deleted,
    is_active,
    can_delete,
    is_system
) VALUES 
(
  '11111111-1111-1111-1111-11111111111D'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    '11111111-1111-1111-1111-111111111119'::UUID,
 '11111111-1111-1111-1111-11111111111A'::UUID,
    '11111111-1111-1111-1111-11111111111B'::UUID,
    'Submit',
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
    TRUE
),
(
    '11111111-1111-1111-1111-11111111111E'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    '11111111-1111-1111-1111-111111111119'::UUID,
    '11111111-1111-1111-1111-11111111111B'::UUID,
    '11111111-1111-1111-1111-11111111111C'::UUID,
    'Publish',
    NOW(),
 '00000000-0000-0000-0000-000000000000'::UUID,
NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
    TRUE
),
(
    '11111111-1111-1111-1111-11111111111F'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    '11111111-1111-1111-1111-111111111119'::UUID,
    '11111111-1111-1111-1111-11111111111B'::UUID,
    '11111111-1111-1111-1111-11111111111A'::UUID,
    'Reject',
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
    TRUE
)
ON CONFLICT (id) DO NOTHING;

-- ============================================================================
-- SEED CONTENT_TYPE (Basic page type)
-- ============================================================================

INSERT INTO content_types (
 id,
    tenant_id,
    type_key,
    display_name,
    schema_version,
    settings_json,
    created_on,
 created_by,
    updated_on,
    updated_by,
    is_deleted,
    is_active,
    can_delete,
    is_system
) VALUES (
    '11111111-1111-1111-1111-111111111120'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    'page.basic',
    'Basic Page',
    1,
    '{"description": "Basic page content type"}'::JSONB,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
 TRUE
) ON CONFLICT (id) DO NOTHING;

-- Basic page fields
INSERT INTO content_type_fields (
    id,
    tenant_id,
    content_type_id,
    field_key,
    data_type,
    is_required,
    is_localized,
    constraints_json,
    sort_order,
    created_on,
    created_by,
    updated_on,
    updated_by,
    is_deleted,
    is_active,
    can_delete,
    is_system
) VALUES 
(
    '11111111-1111-1111-1111-111111111121'::UUID,
    '11111111-1111-1111-1111-111111111111'::UUID,
    '11111111-1111-1111-1111-111111111120'::UUID,
    'title',
    'string',
    TRUE,
TRUE,
    '{"minLength": 1, "maxLength": 500}'::JSONB,
    1,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
    TRUE
),
(
    '11111111-1111-1111-1111-111111111122'::UUID,
 '11111111-1111-1111-1111-111111111111'::UUID,
    '11111111-1111-1111-1111-111111111120'::UUID,
    'body',
    'richtext',
    FALSE,
    TRUE,
    '{}'::JSONB,
    2,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    NOW(),
    '00000000-0000-0000-0000-000000000000'::UUID,
    FALSE,
    TRUE,
    FALSE,
    TRUE
)
ON CONFLICT (id) DO NOTHING;

COMMENT ON SCRIPT IS 'ContentOS seed data - creates system tenant, demo tenant, default roles, and basic workflow';
