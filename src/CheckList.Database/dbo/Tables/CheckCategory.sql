CREATE TABLE [dbo].[CheckCategory]
(
    [CategoryId] INT IDENTITY(1,1) NOT NULL,
    [ListId] INT NOT NULL,
    [CategoryText] NVARCHAR(255) NOT NULL,
    [CategoryDscr] NVARCHAR(1000) NULL,
    [ActiveInd] NVARCHAR(1) NOT NULL,
    [SortOrder] INT NOT NULL,
    [CreateDateTime] DATETIME NOT NULL,
    [CreateUserName] NVARCHAR(255) NOT NULL,
    [ChangeDateTime] DATETIME NOT NULL,
    [ChangeUserName] NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_CheckCategory] PRIMARY KEY CLUSTERED ([CategoryId] ASC),
    CONSTRAINT [FK_CheckCategory_CheckList] FOREIGN KEY ([ListId]) REFERENCES [dbo].[CheckList] ([ListId]) ON UPDATE CASCADE ON DELETE CASCADE
);
GO

ALTER TABLE [dbo].[CheckCategory] ADD CONSTRAINT [DF_CheckCategory_ActiveInd] DEFAULT (N'Y') FOR [ActiveInd];
GO
ALTER TABLE [dbo].[CheckCategory] ADD CONSTRAINT [DF_CheckCategory_SortOrder] DEFAULT ((50)) FOR [SortOrder];
GO
ALTER TABLE [dbo].[CheckCategory] ADD CONSTRAINT [DF_CheckCategory_CreateDateTime] DEFAULT (GETDATE()) FOR [CreateDateTime];
GO
ALTER TABLE [dbo].[CheckCategory] ADD CONSTRAINT [DF_CheckCategory_CreateUserName] DEFAULT (N'UNKNOWN') FOR [CreateUserName];
GO
ALTER TABLE [dbo].[CheckCategory] ADD CONSTRAINT [DF_CheckCategory_ChangeDateTime] DEFAULT (GETDATE()) FOR [ChangeDateTime];
GO
ALTER TABLE [dbo].[CheckCategory] ADD CONSTRAINT [DF_CheckCategory_ChangeUserName] DEFAULT (N'UNKNOWN') FOR [ChangeUserName];
GO
