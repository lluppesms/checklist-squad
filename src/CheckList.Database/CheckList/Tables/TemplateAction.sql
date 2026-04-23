CREATE TABLE [CheckList].[TemplateAction]
(
    [ActionId] INT IDENTITY(1,1) NOT NULL,
    [CategoryId] INT NULL,
    [ActionText] NVARCHAR(255) NULL,
    [ActionDscr] NVARCHAR(1000) NULL,
    [CompleteInd] NVARCHAR(1) NULL,
    [SortOrder] INT NOT NULL,
    [CreateDateTime] DATETIME NOT NULL,
    [CreateUserName] NVARCHAR(255) NOT NULL,
    [ChangeDateTime] DATETIME NOT NULL,
    [ChangeUserName] NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_TemplateAction] PRIMARY KEY CLUSTERED ([ActionId] ASC),
    CONSTRAINT [FK_TemplateAction_TemplateCategory] FOREIGN KEY ([CategoryId]) REFERENCES [CheckList].[TemplateCategory] ([CategoryId]) ON UPDATE CASCADE ON DELETE CASCADE
);
GO

ALTER TABLE [CheckList].[TemplateAction] ADD CONSTRAINT [DF_TemplateAction_SortOrder] DEFAULT ((50)) FOR [SortOrder];
GO
ALTER TABLE [CheckList].[TemplateAction] ADD CONSTRAINT [DF_TemplateAction_CreateDateTime] DEFAULT (GETDATE()) FOR [CreateDateTime];
GO
ALTER TABLE [CheckList].[TemplateAction] ADD CONSTRAINT [DF_TemplateAction_CreateUserName] DEFAULT (N'UNKNOWN') FOR [CreateUserName];
GO
ALTER TABLE [CheckList].[TemplateAction] ADD CONSTRAINT [DF_TemplateAction_ChangeDateTime] DEFAULT (GETDATE()) FOR [ChangeDateTime];
GO
ALTER TABLE [CheckList].[TemplateAction] ADD CONSTRAINT [DF_TemplateAction_ChangeUserName] DEFAULT (N'UNKNOWN') FOR [ChangeUserName];
GO
