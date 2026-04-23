CREATE TABLE [dbo].[CheckSetShare]
(
    [ShareId]          INT           IDENTITY(1,1) NOT NULL,
    [CheckSetId]       INT           NOT NULL,
    [SharedWithUserId] NVARCHAR(256) NOT NULL,
    [Role]             NVARCHAR(50)  NOT NULL,
    [CreateDateTime]   DATETIME      NOT NULL,
    [CreateUserName]   NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_CheckSetShare] PRIMARY KEY CLUSTERED ([ShareId] ASC),
    CONSTRAINT [FK_CheckSetShare_CheckSet]  FOREIGN KEY ([CheckSetId])       REFERENCES [dbo].[CheckSet]  ([SetId])  ON DELETE CASCADE,
    CONSTRAINT [FK_CheckSetShare_AppUser]   FOREIGN KEY ([SharedWithUserId]) REFERENCES [dbo].[AppUser]   ([UserId]) ON DELETE CASCADE,
    CONSTRAINT [UQ_CheckSetShare_SetUser]   UNIQUE ([CheckSetId], [SharedWithUserId])
);
GO

ALTER TABLE [dbo].[CheckSetShare] ADD CONSTRAINT [DF_CheckSetShare_Role]           DEFAULT (N'user')    FOR [Role];
GO
ALTER TABLE [dbo].[CheckSetShare] ADD CONSTRAINT [DF_CheckSetShare_CreateDateTime] DEFAULT (GETDATE())  FOR [CreateDateTime];
GO
ALTER TABLE [dbo].[CheckSetShare] ADD CONSTRAINT [DF_CheckSetShare_CreateUserName] DEFAULT (N'UNKNOWN') FOR [CreateUserName];
GO
