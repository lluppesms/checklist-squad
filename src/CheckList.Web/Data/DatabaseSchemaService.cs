using Microsoft.EntityFrameworkCore;

namespace CheckList.Web.Data;

/// <summary>
/// Applies any pending schema changes to the database at startup.
/// All statements are idempotent so they are safe to run on every application start.
/// This eliminates the need for developers to run SQL patch scripts by hand.
/// </summary>
public static class DatabaseSchemaService
{
    public static async Task ApplySchemaUpdatesAsync(IServiceProvider services, ILogger logger)
    {
        var factory = services.GetRequiredService<IDbContextFactory<CheckListDbContext>>();
        await using var db = await factory.CreateDbContextAsync();

        try
        {
            logger.LogInformation("Applying database schema updates...");
            await db.Database.ExecuteSqlRawAsync(SchemaUpdateSql);
            logger.LogInformation("Database schema updates applied successfully.");
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 262 || ex.Number == 2760)
        {
            // Error 262 = CREATE TABLE/ALTER permission denied
            // Error 2760 = schema does not exist
            // In production the DACPAC handles DDL at deploy time, so the runtime
            // identity intentionally lacks DDL permissions. If the schema is already
            // up-to-date this code would have been a no-op anyway.
            logger.LogWarning(
                "Skipped startup schema migration — the runtime identity lacks DDL permissions. " +
                "This is expected when the DACPAC has already been deployed. " +
                "If tables are missing, deploy the DACPAC first. Detail: {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply database schema updates. " +
                "Ensure the database is reachable and the connection string is correct.");
            throw;
        }
    }

    /// <summary>
    /// Idempotent DDL that brings an existing pre-auth-feature database up to the current schema.
    /// Safe to execute on every startup — each statement checks for existence first.
    ///
    /// OwnerId (nullable): NULL means the row pre-dates the authentication feature and has no
    /// designated Entra ID owner. Such rows remain visible only via GetAllActiveSetsAsync()
    /// (admin/legacy usage). User-scoped queries via GetActiveSetsForUserAsync() will NOT
    /// return un-owned rows, providing safe isolation for new users.
    /// </summary>
    private const string SchemaUpdateSql = @"
-- =========================================================
-- AppUser table (stores Entra ID user profiles)
-- =========================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.tables
    WHERE schema_id = SCHEMA_ID('CheckList') AND name = 'AppUser'
)
BEGIN
    CREATE TABLE [CheckList].[AppUser]
    (
        [UserId]            NVARCHAR(256) NOT NULL,
        [DisplayName]       NVARCHAR(256) NOT NULL,
        [Email]             NVARCHAR(256) NULL,
        [CreateDateTime]    DATETIME      NOT NULL CONSTRAINT [DF_AppUser_CreateDateTime]    DEFAULT (GETDATE()),
        [LastLoginDateTime] DATETIME      NOT NULL CONSTRAINT [DF_AppUser_LastLoginDateTime] DEFAULT (GETDATE()),
        CONSTRAINT [PK_AppUser] PRIMARY KEY CLUSTERED ([UserId] ASC)
    );
END

-- =========================================================
-- OwnerId column on TemplateSet
-- NULL = pre-auth legacy row (no Entra owner assigned yet)
-- =========================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[CheckList].[TemplateSet]') AND name = N'OwnerId'
)
BEGIN
    ALTER TABLE [CheckList].[TemplateSet] ADD [OwnerId] NVARCHAR(256) NULL;
END

-- =========================================================
-- OwnerId column on CheckSet
-- NULL = pre-auth legacy row (no Entra owner assigned yet)
-- =========================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[CheckList].[CheckSet]') AND name = N'OwnerId'
)
BEGIN
    ALTER TABLE [CheckList].[CheckSet] ADD [OwnerId] NVARCHAR(256) NULL;
END

-- =========================================================
-- CheckSetShare table (sharing / role grants between users)
-- ON DELETE CASCADE on FK_CheckSetShare_AppUser is intentional:
-- if a user account is removed from the system, their shares are
-- also removed to avoid dangling access grants.
-- =========================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.tables
    WHERE schema_id = SCHEMA_ID('CheckList') AND name = 'CheckSetShare'
)
BEGIN
    CREATE TABLE [CheckList].[CheckSetShare]
    (
        [ShareId]          INT           IDENTITY(1,1) NOT NULL,
        [CheckSetId]       INT           NOT NULL,
        [SharedWithUserId] NVARCHAR(256) NOT NULL,
        [Role]             NVARCHAR(50)  NOT NULL CONSTRAINT [DF_CheckSetShare_Role]           DEFAULT (N'user'),
        [CreateDateTime]   DATETIME      NOT NULL CONSTRAINT [DF_CheckSetShare_CreateDateTime] DEFAULT (GETDATE()),
        [CreateUserName]   NVARCHAR(255) NOT NULL CONSTRAINT [DF_CheckSetShare_CreateUserName] DEFAULT (N'UNKNOWN'),
        CONSTRAINT [PK_CheckSetShare]         PRIMARY KEY CLUSTERED ([ShareId] ASC),
        CONSTRAINT [FK_CheckSetShare_CheckSet] FOREIGN KEY ([CheckSetId])       REFERENCES [CheckList].[CheckSet] ([SetId]) ON DELETE CASCADE,
        CONSTRAINT [FK_CheckSetShare_AppUser]  FOREIGN KEY ([SharedWithUserId]) REFERENCES [CheckList].[AppUser]  ([UserId]) ON DELETE CASCADE,
        CONSTRAINT [UQ_CheckSetShare_SetUser]  UNIQUE ([CheckSetId], [SharedWithUserId])
    );
END

-- =========================================================
-- SharingInvite table (pending partnership invitations)
-- =========================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.tables
    WHERE schema_id = SCHEMA_ID('CheckList') AND name = 'SharingInvite'
)
BEGIN
    CREATE TABLE [CheckList].[SharingInvite]
    (
        [InviteId]         INT           IDENTITY(1,1) NOT NULL,
        [InviteTokenHash]  NVARCHAR(128) NOT NULL,
        [SenderUserId]     NVARCHAR(256) NOT NULL,
        [RecipientEmail]   NVARCHAR(256) NOT NULL,
        [Role]             NVARCHAR(50)  NOT NULL CONSTRAINT [DF_SharingInvite_Role]      DEFAULT (N'user'),
        [Status]           NVARCHAR(50)  NOT NULL CONSTRAINT [DF_SharingInvite_Status]    DEFAULT (N'pending'),
        [ExpiresAt]        DATETIME      NOT NULL,
        [CreatedAt]        DATETIME      NOT NULL CONSTRAINT [DF_SharingInvite_CreatedAt] DEFAULT (GETDATE()),
        [AcceptedByUserId] NVARCHAR(256) NULL,
        [AcceptedAt]       DATETIME      NULL,
        CONSTRAINT [PK_SharingInvite]             PRIMARY KEY CLUSTERED ([InviteId] ASC),
        CONSTRAINT [FK_SharingInvite_Sender]      FOREIGN KEY ([SenderUserId])     REFERENCES [CheckList].[AppUser] ([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_SharingInvite_AcceptedBy]  FOREIGN KEY ([AcceptedByUserId]) REFERENCES [CheckList].[AppUser] ([UserId]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_SharingInvite_TokenHash]   UNIQUE ([InviteTokenHash])
    );
END

-- =========================================================
-- UserPartnership table (bidirectional partnership rows)
-- Each partnership creates TWO rows: A→B and B→A
-- =========================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.tables
    WHERE schema_id = SCHEMA_ID('CheckList') AND name = 'UserPartnership'
)
BEGIN
    CREATE TABLE [CheckList].[UserPartnership]
    (
        [PartnershipId]        INT           IDENTITY(1,1) NOT NULL,
        [UserId]               NVARCHAR(256) NOT NULL,
        [PartnerUserId]        NVARCHAR(256) NOT NULL,
        [Role]                 NVARCHAR(50)  NOT NULL CONSTRAINT [DF_UserPartnership_Role]            DEFAULT (N'user'),
        [AutoShareEnabled]     BIT           NOT NULL CONSTRAINT [DF_UserPartnership_AutoShareEnabled] DEFAULT (1),
        [CreatedFromInviteId]  INT           NULL,
        [CreatedAt]            DATETIME      NOT NULL CONSTRAINT [DF_UserPartnership_CreatedAt]        DEFAULT (GETDATE()),
        CONSTRAINT [PK_UserPartnership]              PRIMARY KEY CLUSTERED ([PartnershipId] ASC),
        CONSTRAINT [FK_UserPartnership_User]         FOREIGN KEY ([UserId])              REFERENCES [CheckList].[AppUser]      ([UserId])   ON DELETE CASCADE,
        CONSTRAINT [FK_UserPartnership_Partner]      FOREIGN KEY ([PartnerUserId])       REFERENCES [CheckList].[AppUser]      ([UserId])   ON DELETE NO ACTION,
        CONSTRAINT [FK_UserPartnership_Invite]       FOREIGN KEY ([CreatedFromInviteId]) REFERENCES [CheckList].[SharingInvite] ([InviteId]) ON DELETE NO ACTION,
        CONSTRAINT [UQ_UserPartnership_UserPartner]  UNIQUE ([UserId], [PartnerUserId])
    );
END

-- =========================================================
-- PartnershipId column on CheckSetShare (provenance tracking)
-- =========================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[CheckList].[CheckSetShare]') AND name = N'PartnershipId'
)
BEGIN
    ALTER TABLE [CheckList].[CheckSetShare] ADD [PartnershipId] INT NULL;
    
    -- Add FK only if the column was just created (prevents duplicate FK on re-run)
    IF NOT EXISTS (
        SELECT 1 FROM sys.foreign_keys
        WHERE object_id = OBJECT_ID(N'[CheckList].[FK_CheckSetShare_Partnership]')
    )
    BEGIN
        ALTER TABLE [CheckList].[CheckSetShare]
        ADD CONSTRAINT [FK_CheckSetShare_Partnership] 
        FOREIGN KEY ([PartnershipId]) REFERENCES [CheckList].[UserPartnership] ([PartnershipId]) ON DELETE NO ACTION;
    END
END
";
}
