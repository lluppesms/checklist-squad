CREATE TABLE [CheckList].[CheckSetShare]
(
    [ShareId]          INT           IDENTITY(1,1) NOT NULL,
    [CheckSetId]       INT           NOT NULL,
    [SharedWithUserId] NVARCHAR(256) NOT NULL,
    [Role]             NVARCHAR(50)  NOT NULL,
    [PartnershipId]    INT           NULL,
    [CreateDateTime]   DATETIME      NOT NULL,
    [CreateUserName]   NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_CheckSetShare] PRIMARY KEY CLUSTERED ([ShareId] ASC),
    CONSTRAINT [FK_CheckSetShare_CheckSet]     FOREIGN KEY ([CheckSetId])       REFERENCES [CheckList].[CheckSet]      ([SetId])        ON DELETE CASCADE,
    CONSTRAINT [FK_CheckSetShare_AppUser]      FOREIGN KEY ([SharedWithUserId]) REFERENCES [CheckList].[AppUser]       ([UserId])       ON DELETE NO ACTION,
    CONSTRAINT [FK_CheckSetShare_Partnership]  FOREIGN KEY ([PartnershipId])    REFERENCES [CheckList].[UserPartnership] ([PartnershipId]) ON DELETE NO ACTION,
    CONSTRAINT [UQ_CheckSetShare_SetUser]      UNIQUE ([CheckSetId], [SharedWithUserId])
);
GO

ALTER TABLE [CheckList].[CheckSetShare] ADD CONSTRAINT [DF_CheckSetShare_Role]           DEFAULT (N'user')    FOR [Role];
GO
ALTER TABLE [CheckList].[CheckSetShare] ADD CONSTRAINT [DF_CheckSetShare_CreateDateTime] DEFAULT (GETDATE())  FOR [CreateDateTime];
GO
ALTER TABLE [CheckList].[CheckSetShare] ADD CONSTRAINT [DF_CheckSetShare_CreateUserName] DEFAULT (N'UNKNOWN') FOR [CreateUserName];
GO
