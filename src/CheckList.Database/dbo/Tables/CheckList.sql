CREATE TABLE [dbo].[CheckList]
(
    [ListId] INT IDENTITY(1,1) NOT NULL,
    [SetId] INT NOT NULL,
    [ListName] NVARCHAR(255) NOT NULL,
    [ListDscr] NVARCHAR(1000) NULL,
    [ActiveInd] NVARCHAR(1) NOT NULL,
    [SortOrder] INT NOT NULL,
    [CreateDateTime] DATETIME NOT NULL,
    [CreateUserName] NVARCHAR(255) NOT NULL,
    [ChangeDateTime] DATETIME NOT NULL,
    [ChangeUserName] NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_CheckList] PRIMARY KEY CLUSTERED ([ListId] ASC),
    CONSTRAINT [FK_CheckList_CheckSet] FOREIGN KEY ([SetId]) REFERENCES [dbo].[CheckSet] ([SetId]) ON UPDATE CASCADE ON DELETE CASCADE
);
GO

ALTER TABLE [dbo].[CheckList] ADD CONSTRAINT [DF_CheckList_ActiveInd] DEFAULT (N'Y') FOR [ActiveInd];
GO
ALTER TABLE [dbo].[CheckList] ADD CONSTRAINT [DF_CheckList_SortOrder] DEFAULT ((50)) FOR [SortOrder];
GO
ALTER TABLE [dbo].[CheckList] ADD CONSTRAINT [DF_CheckList_CreateDateTime] DEFAULT (GETDATE()) FOR [CreateDateTime];
GO
ALTER TABLE [dbo].[CheckList] ADD CONSTRAINT [DF_CheckList_CreateUserName] DEFAULT (N'UNKNOWN') FOR [CreateUserName];
GO
ALTER TABLE [dbo].[CheckList] ADD CONSTRAINT [DF_CheckList_ChangeDateTime] DEFAULT (GETDATE()) FOR [ChangeDateTime];
GO
ALTER TABLE [dbo].[CheckList] ADD CONSTRAINT [DF_CheckList_ChangeUserName] DEFAULT (N'UNKNOWN') FOR [ChangeUserName];
GO
