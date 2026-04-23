CREATE TABLE [CheckList].[CheckSet]
(
    [SetId] INT IDENTITY(1,1) NOT NULL,
    [TemplateSetId] INT NULL,
    [SetName] NVARCHAR(255) NOT NULL,
    [SetDscr] NVARCHAR(1000) NULL,
    [OwnerName] NVARCHAR(256) NOT NULL,
    [OwnerId] NVARCHAR(256) NOT NULL,
    [ActiveInd] NVARCHAR(1) NOT NULL,
    [SortOrder] INT NOT NULL,
    [CreateDateTime] DATETIME NOT NULL,
    [CreateUserName] NVARCHAR(255) NOT NULL,
    [ChangeDateTime] DATETIME NOT NULL,
    [ChangeUserName] NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_CheckSet] PRIMARY KEY CLUSTERED ([SetId] ASC),
    CONSTRAINT [FK_CheckSet_TemplateSet] FOREIGN KEY ([TemplateSetId]) REFERENCES [CheckList].[TemplateSet] ([SetId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CheckSet_AppUser]     FOREIGN KEY ([OwnerId])       REFERENCES [CheckList].[AppUser]    ([UserId]) ON DELETE CASCADE
);
GO

ALTER TABLE [CheckList].[CheckSet] ADD CONSTRAINT [DF_CheckSet_ActiveInd] DEFAULT (N'Y') FOR [ActiveInd];
GO
ALTER TABLE [CheckList].[CheckSet] ADD CONSTRAINT [DF_CheckSet_SortOrder] DEFAULT ((50)) FOR [SortOrder];
GO
ALTER TABLE [CheckList].[CheckSet] ADD CONSTRAINT [DF_CheckSet_CreateDateTime] DEFAULT (GETDATE()) FOR [CreateDateTime];
GO
ALTER TABLE [CheckList].[CheckSet] ADD CONSTRAINT [DF_CheckSet_CreateUserName] DEFAULT (N'UNKNOWN') FOR [CreateUserName];
GO
ALTER TABLE [CheckList].[CheckSet] ADD CONSTRAINT [DF_CheckSet_ChangeDateTime] DEFAULT (GETDATE()) FOR [ChangeDateTime];
GO
ALTER TABLE [CheckList].[CheckSet] ADD CONSTRAINT [DF_CheckSet_ChangeUserName] DEFAULT (N'UNKNOWN') FOR [ChangeUserName];
GO
