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
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CheckListDbContext>();

        try
        {
            logger.LogInformation("Applying database schema updates...");
            await db.Database.ExecuteSqlRawAsync(SchemaUpdateSql);
            logger.LogInformation("Database schema updates applied successfully.");
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
    WHERE schema_id = SCHEMA_ID('dbo') AND name = 'AppUser'
)
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

-- =========================================================
-- OwnerId column on TemplateSet
-- NULL = pre-auth legacy row (no Entra owner assigned yet)
-- =========================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[TemplateSet]') AND name = N'OwnerId'
)
BEGIN
    ALTER TABLE [dbo].[TemplateSet] ADD [OwnerId] NVARCHAR(256) NULL;
END

-- =========================================================
-- OwnerId column on CheckSet
-- NULL = pre-auth legacy row (no Entra owner assigned yet)
-- =========================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[CheckSet]') AND name = N'OwnerId'
)
BEGIN
    ALTER TABLE [dbo].[CheckSet] ADD [OwnerId] NVARCHAR(256) NULL;
END

-- =========================================================
-- CheckSetShare table (sharing / role grants between users)
-- ON DELETE CASCADE on FK_CheckSetShare_AppUser is intentional:
-- if a user account is removed from the system, their shares are
-- also removed to avoid dangling access grants.
-- =========================================================
IF NOT EXISTS (
    SELECT 1 FROM sys.tables
    WHERE schema_id = SCHEMA_ID('dbo') AND name = 'CheckSetShare'
)
BEGIN
    CREATE TABLE [dbo].[CheckSetShare]
    (
        [ShareId]          INT           IDENTITY(1,1) NOT NULL,
        [CheckSetId]       INT           NOT NULL,
        [SharedWithUserId] NVARCHAR(256) NOT NULL,
        [Role]             NVARCHAR(50)  NOT NULL CONSTRAINT [DF_CheckSetShare_Role]           DEFAULT (N'user'),
        [CreateDateTime]   DATETIME      NOT NULL CONSTRAINT [DF_CheckSetShare_CreateDateTime] DEFAULT (GETDATE()),
        [CreateUserName]   NVARCHAR(255) NOT NULL CONSTRAINT [DF_CheckSetShare_CreateUserName] DEFAULT (N'UNKNOWN'),
        CONSTRAINT [PK_CheckSetShare]         PRIMARY KEY CLUSTERED ([ShareId] ASC),
        CONSTRAINT [FK_CheckSetShare_CheckSet] FOREIGN KEY ([CheckSetId])       REFERENCES [dbo].[CheckSet] ([SetId]) ON DELETE CASCADE,
        CONSTRAINT [FK_CheckSetShare_AppUser]  FOREIGN KEY ([SharedWithUserId]) REFERENCES [dbo].[AppUser]  ([UserId]) ON DELETE CASCADE,
        CONSTRAINT [UQ_CheckSetShare_SetUser]  UNIQUE ([CheckSetId], [SharedWithUserId])
    );
END
";
}
