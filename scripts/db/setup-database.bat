@echo off
REM ============================================================================
REM ContentOS Database Setup Script (Windows)
REM Version: 1.0
REM Description: Master script to run all PostgreSQL setup scripts in order
REM ============================================================================

setlocal enabledelayedexpansion

REM Configuration (can be overridden by environment variables)
if not defined DB_NAME set DB_NAME=contentos_core
if not defined DB_USER set DB_USER=postgres
if not defined DB_HOST set DB_HOST=localhost
if not defined DB_PORT set DB_PORT=5432

set SCRIPT_DIR=%~dp0

echo ============================================================================
echo ContentOS Database Setup
echo ============================================================================
echo.
echo Database: %DB_NAME%
echo Host: %DB_HOST%
echo Port: %DB_PORT%
echo User: %DB_USER%
echo.

REM Check if psql is available
where psql >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo [ERROR] psql command not found. Please install PostgreSQL client.
    echo        PostgreSQL can be downloaded from: https://www.postgresql.org/download/
    pause
    exit /b 1
)

REM Check database connection
echo [INFO] Checking database connection...
psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d postgres -c "\q" >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo [ERROR] Cannot connect to PostgreSQL at %DB_HOST%:%DB_PORT% as user %DB_USER%
    echo [ERROR] Please check your connection settings and ensure PostgreSQL is running.
    echo.
    echo Set environment variables to configure connection:
    echo   set DB_HOST=localhost
    echo   set DB_PORT=5432
    echo   set DB_USER=postgres
    echo   set PGPASSWORD=your_password
    pause
    exit /b 1
)

echo [INFO] Connection successful!
echo.

REM Create database if it doesn't exist
echo [INFO] Creating database '%DB_NAME%' if it doesn't exist...
psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d postgres -tc "SELECT 1 FROM pg_database WHERE datname = '%DB_NAME%'" | findstr /C:"1" >nul
if %ERRORLEVEL% neq 0 (
    psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d postgres -c "CREATE DATABASE %DB_NAME%"
    echo [INFO] Database created successfully.
) else (
    echo [INFO] Database already exists.
)
echo.

REM Run all setup scripts in order
echo [INFO] Starting database setup...
echo.

set SCRIPTS=01-setup-database.sql 02-create-core-tables.sql 03-create-content-tables.sql 04-create-workflow-security-tables.sql 05-create-layout-module-tables.sql 06-seed-data.sql

for %%s in (%SCRIPTS%) do (
    set script_path=%SCRIPT_DIR%%%s
    
 if not exist "!script_path!" (
  echo [ERROR] Script not found: !script_path!
        pause
        exit /b 1
    )
    
    echo [INFO] Running %%s...
    
    psql -h %DB_HOST% -p %DB_PORT% -U %DB_USER% -d %DB_NAME% -f "!script_path!" >nul 2>nul
    if !ERRORLEVEL! equ 0 (
        echo [INFO] ? %%s completed successfully
    ) else (
        echo [ERROR] × %%s failed
        echo [ERROR] Please check the error messages above and fix any issues.
        pause
        exit /b 1
    )
    
    echo.
)

echo ============================================================================
echo Database setup completed successfully!
echo ============================================================================
echo.
echo Database Details:
echo   - Database: %DB_NAME%
echo   - Host: %DB_HOST%
echo   - Port: %DB_PORT%
echo   - User: %DB_USER%
echo.
echo Demo Credentials:
echo   - Tenant: demo (key: demo)
echo   - Site: localhost
echo   - User: admin@demo.local
echo.
echo Next Steps:
echo   1. Update your appsettings.json with the connection string
echo   2. Run your .NET application
echo   3. Access the API at http://localhost:5000 (or your configured port)
echo.
echo Connection String Example:
echo   Host=%DB_HOST%;Port=%DB_PORT%;Database=%DB_NAME%;Username=%DB_USER%;Password=YOUR_PASSWORD
echo.

pause
