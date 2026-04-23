/*
Pre-Deployment Script
Wipe user-created checklist data when OwnerId is not yet enforced as NOT NULL.
This allows the DACPAC to safely alter CheckSet.OwnerId from NULL to NOT NULL.
Safe to re-run: no-ops once OwnerId is already NOT NULL.
*/

IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[CheckSet]') AND name = N'OwnerId' AND is_nullable = 1
)
BEGIN
    -- OwnerId is still nullable — wipe dependent data before DACPAC enforces NOT NULL.
    IF OBJECT_ID(N'[dbo].[CheckAction]',   'U') IS NOT NULL DELETE FROM [dbo].[CheckAction];
    IF OBJECT_ID(N'[dbo].[CheckCategory]', 'U') IS NOT NULL DELETE FROM [dbo].[CheckCategory];
    IF OBJECT_ID(N'[dbo].[CheckList]',     'U') IS NOT NULL DELETE FROM [dbo].[CheckList];
    IF OBJECT_ID(N'[dbo].[CheckSetShare]', 'U') IS NOT NULL DELETE FROM [dbo].[CheckSetShare];
    IF OBJECT_ID(N'[dbo].[CheckSet]',      'U') IS NOT NULL DELETE FROM [dbo].[CheckSet];
    IF OBJECT_ID(N'[dbo].[AppUser]',       'U') IS NOT NULL DELETE FROM [dbo].[AppUser];
END
