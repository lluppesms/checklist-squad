-- ------------------------------------------------------------------------------------------------------------------------
-- Grant App Service Managed Identity access to the database
-- ------------------------------------------------------------------------------------------------------------------------
-- This script grants the App Service system-assigned managed identity schema-scoped
-- permissions on the [CheckList] schema. Using schema-scoped grants instead of
-- db_datareader / db_datawriter ensures this identity cannot read or modify tables
-- belonging to other applications sharing the same database.
-- The identity name must be passed as a SQLCMD variable: $(AppIdentityName)
-- ------------------------------------------------------------------------------------------------------------------------
-- Usage (manual):
--   Invoke-Sqlcmd -ServerInstance "yourserver.database.windows.net" -Database "YourDb" `
--     -AccessToken $token -Variable "AppIdentityName=lsq-checklist1-dev" -InputFile "Add-App-User-Rights.sql"
-- ------------------------------------------------------------------------------------------------------------------------

PRINT 'Granting database access to managed identity: $(AppIdentityName)';

-- Create the user from the external (Entra ID) provider if it does not already exist
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = '$(AppIdentityName)')
BEGIN
    PRINT 'Creating user [$(AppIdentityName)] from external provider...';
    CREATE USER [$(AppIdentityName)] FROM EXTERNAL PROVIDER;
    PRINT 'User [$(AppIdentityName)] created successfully.';
END
ELSE
BEGIN
    PRINT 'User [$(AppIdentityName)] already exists — skipping creation.';
END

-- Grant schema-scoped DML permissions (SELECT, INSERT, UPDATE, DELETE)
-- This replaces db_datareader + db_datawriter with least-privilege access
PRINT 'Granting SELECT, INSERT, UPDATE, DELETE on schema [CheckList] to [$(AppIdentityName)]...';
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[CheckList] TO [$(AppIdentityName)];

-- Grant EXECUTE on the CheckList schema (for stored procedures)
PRINT 'Granting EXECUTE on schema [CheckList] to [$(AppIdentityName)]...';
GRANT EXECUTE ON SCHEMA::[CheckList] TO [$(AppIdentityName)];

PRINT 'Managed identity rights granted successfully for [$(AppIdentityName)].';
GO
