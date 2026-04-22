-- ------------------------------------------------------------------------------------------------------------------------
-- Grant App Service Managed Identity access to the database
-- ------------------------------------------------------------------------------------------------------------------------
-- This script grants the App Service system-assigned managed identity the necessary
-- database roles to read, write, and execute stored procedures.
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

-- Grant db_datareader role
IF NOT EXISTS (SELECT 1 FROM sys.database_role_members rm
    JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON rm.member_principal_id = m.principal_id
    WHERE r.name = 'db_datareader' AND m.name = '$(AppIdentityName)')
BEGIN
    PRINT 'Adding [$(AppIdentityName)] to db_datareader...';
    ALTER ROLE db_datareader ADD MEMBER [$(AppIdentityName)];
END
ELSE
BEGIN
    PRINT '[$(AppIdentityName)] is already a member of db_datareader.';
END

-- Grant db_datawriter role
IF NOT EXISTS (SELECT 1 FROM sys.database_role_members rm
    JOIN sys.database_principals r ON rm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON rm.member_principal_id = m.principal_id
    WHERE r.name = 'db_datawriter' AND m.name = '$(AppIdentityName)')
BEGIN
    PRINT 'Adding [$(AppIdentityName)] to db_datawriter...';
    ALTER ROLE db_datawriter ADD MEMBER [$(AppIdentityName)];
END
ELSE
BEGIN
    PRINT '[$(AppIdentityName)] is already a member of db_datawriter.';
END

-- Grant EXECUTE on dbo schema
PRINT 'Granting EXECUTE on schema [dbo] to [$(AppIdentityName)]...';
GRANT EXECUTE ON SCHEMA::[dbo] TO [$(AppIdentityName)];

PRINT 'Managed identity rights granted successfully for [$(AppIdentityName)].';
GO
