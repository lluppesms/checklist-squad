CREATE TABLE [CheckList].[TemplateList]
(
    [ListId] INT IDENTITY(1,1) NOT NULL,
    [SetId] INT NOT NULL,
    [ListName] NVARCHAR(255) NOT NULL,
    [ListDscr] NVARCHAR(1000) NOT NULL,
    [ActiveInd] NVARCHAR(1) NOT NULL,
    [SortOrder] INT NOT NULL,
    [CreateDateTime] DATETIME NOT NULL,
    [CreateUserName] NVARCHAR(255) NOT NULL,
    [ChangeDateTime] DATETIME NOT NULL,
    [ChangeUserName] NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_TemplateList] PRIMARY KEY CLUSTERED ([ListId] ASC),
    CONSTRAINT [FK_TemplateList_TemplateSet] FOREIGN KEY ([SetId]) REFERENCES [CheckList].[TemplateSet] ([SetId]) ON UPDATE CASCADE ON DELETE CASCADE
);
GO

ALTER TABLE [CheckList].[TemplateList] ADD CONSTRAINT [DF_TemplateList_ActiveInd] DEFAULT (N'Y') FOR [ActiveInd];
GO
ALTER TABLE [CheckList].[TemplateList] ADD CONSTRAINT [DF_TemplateList_SortOrder] DEFAULT ((50)) FOR [SortOrder];
GO
ALTER TABLE [CheckList].[TemplateList] ADD CONSTRAINT [DF_TemplateList_CreateDateTime] DEFAULT (GETDATE()) FOR [CreateDateTime];
GO
ALTER TABLE [CheckList].[TemplateList] ADD CONSTRAINT [DF_TemplateList_CreateUserName] DEFAULT (N'UNKNOWN') FOR [CreateUserName];
GO
ALTER TABLE [CheckList].[TemplateList] ADD CONSTRAINT [DF_TemplateList_ChangeDateTime] DEFAULT (GETDATE()) FOR [ChangeDateTime];
GO
ALTER TABLE [CheckList].[TemplateList] ADD CONSTRAINT [DF_TemplateList_ChangeUserName] DEFAULT (N'UNKNOWN') FOR [ChangeUserName];
GO
