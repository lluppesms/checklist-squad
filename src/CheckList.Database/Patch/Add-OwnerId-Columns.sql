-- Create AppUser table if it doesn't exist (new table added with Entra ID auth).
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AppUser]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[AppUser]
    (
        [UserId]            NVARCHAR(256) NOT NULL,
        [DisplayName]       NVARCHAR(256) NOT NULL,
        [Email]             NVARCHAR(256) NULL,
        [CreateDateTime]    DATETIME      NOT NULL CONSTRAINT [DF_AppUser_CreateDateTime]    DEFAULT (GETDATE()),
        [LastLoginDateTime] DATETIME      NOT NULL CONSTRAINT [DF_AppUser_LastLoginDateTime] DEFAULT (GETDATE()),
        CONSTRAINT [PK_AppUser] PRIMARY KEY CLUSTERED ([UserId] ASC)
    );
END
GO

-- Create CheckSetShare table if it doesn't exist (new table added with Entra ID auth).
IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CheckSetShare]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[CheckSetShare]
    (
        [ShareId]          INT           IDENTITY(1,1) NOT NULL,
        [CheckSetId]       INT           NOT NULL,
        [SharedWithUserId] NVARCHAR(256) NOT NULL,
        [Role]             NVARCHAR(50)  NOT NULL CONSTRAINT [DF_CheckSetShare_Role]           DEFAULT (N'user'),
        [CreateDateTime]   DATETIME      NOT NULL CONSTRAINT [DF_CheckSetShare_CreateDateTime] DEFAULT (GETDATE()),
        [CreateUserName]   NVARCHAR(255) NOT NULL CONSTRAINT [DF_CheckSetShare_CreateUserName] DEFAULT (N'UNKNOWN'),
        CONSTRAINT [PK_CheckSetShare]      PRIMARY KEY CLUSTERED ([ShareId] ASC),
        CONSTRAINT [FK_CheckSetShare_CheckSet] FOREIGN KEY ([CheckSetId])       REFERENCES [dbo].[CheckSet]  ([SetId])  ON DELETE CASCADE,
        CONSTRAINT [FK_CheckSetShare_AppUser]  FOREIGN KEY ([SharedWithUserId]) REFERENCES [dbo].[AppUser]   ([UserId]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_CheckSetShare_SetUser]  UNIQUE ([CheckSetId], [SharedWithUserId])
    );
END
GO

-- Fix FK_CheckSetShare_AppUser if it was previously created with CASCADE (causes multiple cascade path error).
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CheckSetShare_AppUser')
BEGIN
    ALTER TABLE [dbo].[CheckSetShare] DROP CONSTRAINT [FK_CheckSetShare_AppUser];
    ALTER TABLE [dbo].[CheckSetShare] ADD CONSTRAINT [FK_CheckSetShare_AppUser] FOREIGN KEY ([SharedWithUserId]) REFERENCES [dbo].[AppUser] ([UserId]) ON DELETE NO ACTION;
END
GO

-- Wipe all user-created checklist data so OwnerId can be made NOT NULL with a FK to AppUser.
-- Order respects FK cascade: deepest children first.
DELETE FROM [dbo].[CheckAction];
DELETE FROM [dbo].[CheckCategory];
DELETE FROM [dbo].[CheckList];
DELETE FROM [dbo].[CheckSetShare];
DELETE FROM [dbo].[CheckSet];
DELETE FROM [dbo].[AppUser];
GO

-- Ensure OwnerId column exists on CheckSet, then enforce NOT NULL + FK to AppUser.
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[CheckSet]') AND name = N'OwnerId'
)
BEGIN
    ALTER TABLE [dbo].[CheckSet] ADD [OwnerId] NVARCHAR(256) NULL;
END
GO

-- Drop FK if it already exists (idempotent re-run).
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CheckSet_AppUser')
    ALTER TABLE [dbo].[CheckSet] DROP CONSTRAINT [FK_CheckSet_AppUser];
GO

-- Now that the table is empty, make the column NOT NULL.
ALTER TABLE [dbo].[CheckSet] ALTER COLUMN [OwnerId] NVARCHAR(256) NOT NULL;
GO

ALTER TABLE [dbo].[CheckSet]
    ADD CONSTRAINT [FK_CheckSet_AppUser] FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[AppUser] ([UserId]) ON DELETE CASCADE;
GO

-- Add OwnerId column to TemplateSet (nullable — templates are not strictly user-owned).
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[TemplateSet]') AND name = N'OwnerId'
)
BEGIN
    ALTER TABLE [dbo].[TemplateSet] ADD [OwnerId] NVARCHAR(256) NULL;
END
GO
