/*
Pre-Deployment Script
Wipe user-created checklist data when OwnerId is not yet enforced as NOT NULL.
This allows the DACPAC to safely alter CheckSet.OwnerId from NULL to NOT NULL.
Safe to re-run: no-ops once OwnerId is already NOT NULL.
*/

IF EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[CheckList].[CheckSet]') AND name = N'OwnerId' AND is_nullable = 1
)
BEGIN
    -- OwnerId is still nullable — wipe dependent data before DACPAC enforces NOT NULL.
    IF OBJECT_ID(N'[CheckList].[CheckAction]',   'U') IS NOT NULL DELETE FROM [CheckList].[CheckAction];
    IF OBJECT_ID(N'[CheckList].[CheckCategory]', 'U') IS NOT NULL DELETE FROM [CheckList].[CheckCategory];
    IF OBJECT_ID(N'[CheckList].[CheckList]',     'U') IS NOT NULL DELETE FROM [CheckList].[CheckList];
    IF OBJECT_ID(N'[CheckList].[CheckSetShare]', 'U') IS NOT NULL DELETE FROM [CheckList].[CheckSetShare];
    IF OBJECT_ID(N'[CheckList].[CheckSet]',      'U') IS NOT NULL DELETE FROM [CheckList].[CheckSet];
    IF OBJECT_ID(N'[CheckList].[AppUser]',       'U') IS NOT NULL DELETE FROM [CheckList].[AppUser];
END
