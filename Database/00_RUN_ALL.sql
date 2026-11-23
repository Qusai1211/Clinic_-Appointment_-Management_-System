-- ============================================================================
-- Clinic Appointment Management System
-- Tier 1: Database - 00_RUN_ALL.sql
-- Master Deployment Script for Oracle Database 19c / 21c / XE
--
-- Instructions:
-- 1. Connect to Oracle using SQL*Plus or SQL Developer as the application user:
--    sqlplus my_user/123456789@localhost:1521/finalpdb
-- 2. Execute this script:
--    @00_RUN_ALL.sql
-- ============================================================================

SET ECHO ON;
SET DEFINE OFF;
SET SERVEROUTPUT ON;
WHENEVER SQLERROR CONTINUE;

PROMPT [1/4] Executing 01_SCHEMA.sql...
@@01_SCHEMA.sql

PROMPT [2/4] Executing 02_VIEWS.sql...
@@02_VIEWS.sql

PROMPT [3/4] Executing 03_PACKAGES.sql...
@@03_PACKAGES.sql

PROMPT [4/4] Executing 04_SEED_DATA.sql...
@@04_SEED_DATA.sql

PROMPT Database initialization and deployment completed successfully!
