#!/bin/bash

# ============================================================================
# ContentOS Database Setup Script
# Version: 1.0
# Description: Master script to run all PostgreSQL setup scripts in order
# ============================================================================

set -e  # Exit on error
set -u  # Exit on undefined variable

# Configuration
DB_NAME="${DB_NAME:-contentos_core}"
DB_USER="${DB_USER:-postgres}"
DB_HOST="${DB_HOST:-localhost}"
DB_PORT="${DB_PORT:-5432}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if psql is installed
if ! command -v psql &> /dev/null; then
    log_error "psql command not found. Please install PostgreSQL client."
    exit 1
fi

# Check database connection
log_info "Checking database connection..."
if ! psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d postgres -c '\q' 2>/dev/null; then
    log_error "Cannot connect to PostgreSQL at $DB_HOST:$DB_PORT as user $DB_USER"
    log_error "Please check your connection settings and credentials."
    exit 1
fi

log_info "Connection successful!"

# Create database if it doesn't exist
log_info "Creating database '$DB_NAME' if it doesn't exist..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d postgres -tc "SELECT 1 FROM pg_database WHERE datname = '$DB_NAME'" | grep -q 1 || \
    psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d postgres -c "CREATE DATABASE $DB_NAME"

# Run all setup scripts in order
SCRIPTS=(
    "01-setup-database.sql"
    "02-create-core-tables.sql"
    "03-create-content-tables.sql"
  "04-create-workflow-security-tables.sql"
    "05-create-layout-module-tables.sql"
    "06-seed-data.sql"
)

log_info "Starting database setup..."
echo ""

for script in "${SCRIPTS[@]}"; do
  script_path="$SCRIPT_DIR/$script"
    
    if [ ! -f "$script_path" ]; then
        log_error "Script not found: $script_path"
        exit 1
    fi
    
    log_info "Running $script..."
    
    if psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f "$script_path" > /dev/null 2>&1; then
        log_info "? $script completed successfully"
else
        log_error "? $script failed"
        log_error "Please check the error messages above and fix any issues."
    exit 1
    fi
    
    echo ""
done

log_info "Database setup completed successfully!"
echo ""
log_info "Database Details:"
echo "  - Database: $DB_NAME"
echo "  - Host: $DB_HOST"
echo "  - Port: $DB_PORT"
echo "  - User: $DB_USER"
echo ""
log_info "Demo Credentials:"
echo "  - Tenant: demo (key: demo)"
echo "  - Site: localhost"
echo "  - User: admin@demo.local"
echo ""
log_info "Next Steps:"
echo "  1. Update your appsettings.json with the connection string"
echo "  2. Run your .NET application"
echo "  3. Access the API at http://localhost:5000 (or your configured port)"
