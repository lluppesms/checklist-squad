CREATE TABLE [dbo].[CheckAction]
(
    [ActionId] INT IDENTITY(1,1) NOT NULL,
    [CategoryId] INT NOT NULL,
    [ListId] INT NOT NULL,
    [ActionText] NVARCHAR(255) NOT NULL,
    [ActionDscr] NVARCHAR(1000) NULL,
    [CompleteInd] NVARCHAR(1) NOT NULL,
    [SortOrder] INT NOT NULL,
    [CreateDateTime] DATETIME NOT NULL,
    [CreateUserName] NVARCHAR(255) NOT NULL,
    [ChangeDateTime] DATETIME NOT NULL,
    [ChangeUserName] NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_CheckAction] PRIMARY KEY CLUSTERED ([ActionId] ASC),
    CONSTRAINT [FK_CheckAction_CheckCategory] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[CheckCategory] ([CategoryId]) ON UPDATE CASCADE ON DELETE CASCADE
);
GO

ALTER TABLE [dbo].[CheckAction] ADD CONSTRAINT [DF_CheckAction_CompleteInd] DEFAULT (N'N') FOR [CompleteInd];
GO
ALTER TABLE [dbo].[CheckAction] ADD CONSTRAINT [DF_CheckAction_SortOrder] DEFAULT ((50)) FOR [SortOrder];
GO
ALTER TABLE [dbo].[CheckAction] ADD CONSTRAINT [DF_CheckAction_CreateDateTime] DEFAULT (GETDATE()) FOR [CreateDateTime];
GO
ALTER TABLE [dbo].[CheckAction] ADD CONSTRAINT [DF_CheckAction_CreateUserName] DEFAULT (N'UNKNOWN') FOR [CreateUserName];
GO
ALTER TABLE [dbo].[CheckAction] ADD CONSTRAINT [DF_CheckAction_ChangeDateTime] DEFAULT (GETDATE()) FOR [ChangeDateTime];
GO
ALTER TABLE [dbo].[CheckAction] ADD CONSTRAINT [DF_CheckAction_ChangeUserName] DEFAULT (N'UNKNOWN') FOR [ChangeUserName];
GO
