CREATE TABLE [dbo].[TemplateCategory]
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
    CONSTRAINT [PK_TemplateCategory] PRIMARY KEY CLUSTERED ([CategoryId] ASC),
    CONSTRAINT [FK_TemplateCategory_TemplateList] FOREIGN KEY ([ListId]) REFERENCES [dbo].[TemplateList] ([ListId]) ON UPDATE CASCADE ON DELETE CASCADE
);
GO

ALTER TABLE [dbo].[TemplateCategory] ADD CONSTRAINT [DF_TemplateCategory_ActiveInd] DEFAULT (N'Y') FOR [ActiveInd];
GO
ALTER TABLE [dbo].[TemplateCategory] ADD CONSTRAINT [DF_TemplateCategory_SortOrder] DEFAULT ((50)) FOR [SortOrder];
GO
ALTER TABLE [dbo].[TemplateCategory] ADD CONSTRAINT [DF_TemplateCategory_CreateDateTime] DEFAULT (GETDATE()) FOR [CreateDateTime];
GO
ALTER TABLE [dbo].[TemplateCategory] ADD CONSTRAINT [DF_TemplateCategory_CreateUserName] DEFAULT (N'UNKNOWN') FOR [CreateUserName];
GO
ALTER TABLE [dbo].[TemplateCategory] ADD CONSTRAINT [DF_TemplateCategory_ChangeDateTime] DEFAULT (GETDATE()) FOR [ChangeDateTime];
GO
ALTER TABLE [dbo].[TemplateCategory] ADD CONSTRAINT [DF_TemplateCategory_ChangeUserName] DEFAULT (N'UNKNOWN') FOR [ChangeUserName];
GO
