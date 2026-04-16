-- ============================================================================
-- ContentOS PostgreSQL Database Setup Script
-- Version: 1.0
-- Description: Creates database, extensions, and base configuration
-- ============================================================================

-- Create database (run as postgres superuser)
-- Note: Run this separately if database doesn't exist
-- CREATE DATABASE contentos_core;

-- Connect to the database
\c contentos_core;

-- ============================================================================
-- EXTENSIONS
-- ============================================================================

-- Enable UUID generation
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Enable full-text search
CREATE EXTENSION IF NOT EXISTS "pg_trgm";

-- Enable vector operations for AI embeddings (if available)
-- Uncomment if pgvector is installed
-- CREATE EXTENSION IF NOT EXISTS "vector";

-- Enable JSON operations
CREATE EXTENSION IF NOT EXISTS "btree_gin";

-- ============================================================================
-- SCHEMAS
-- ============================================================================

-- Create application schema (optional, for namespace separation)
CREATE SCHEMA IF NOT EXISTS contentos;

-- Set default schema
SET search_path TO contentos, public;

-- ============================================================================
-- FUNCTIONS
-- ============================================================================

-- Function to update updated_on timestamp automatically
CREATE OR REPLACE FUNCTION update_updated_on_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_on = NOW();
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Function to set soft delete timestamp
CREATE OR REPLACE FUNCTION set_deleted_on()
RETURNS TRIGGER AS $$
BEGIN
 IF NEW.is_deleted = TRUE AND OLD.is_deleted = FALSE THEN
        NEW.deleted_on = NOW();
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- ============================================================================
-- COMMENTS
-- ============================================================================

COMMENT ON DATABASE contentos_core IS 'ContentOS - Enterprise Multi-Tenant CMS Database';
COMMENT ON SCHEMA contentos IS 'ContentOS application schema';
COMMENT ON FUNCTION update_updated_on_column() IS 'Automatically updates updated_on timestamp on row modification';
COMMENT ON FUNCTION set_deleted_on() IS 'Automatically sets deleted_on timestamp when is_deleted is set to true';
