CREATE TABLE [dbo].[TemplateSet]
(
    [SetId] INT IDENTITY(1,1) NOT NULL,
    [SetName] NVARCHAR(255) NOT NULL,
    [SetDscr] NVARCHAR(1000) NOT NULL,
    [OwnerName] NVARCHAR(256) NOT NULL,
    [OwnerId] NVARCHAR(256) NULL,
    [ActiveInd] NVARCHAR(1) NOT NULL,
    [SortOrder] INT NOT NULL,
    [CreateDateTime] DATETIME NOT NULL,
    [CreateUserName] NVARCHAR(255) NOT NULL,
    [ChangeDateTime] DATETIME NOT NULL,
    [ChangeUserName] NVARCHAR(255) NOT NULL,
    CONSTRAINT [PK_TemplateSet] PRIMARY KEY CLUSTERED ([SetId] ASC)
);
GO

-- Defaults
ALTER TABLE [dbo].[TemplateSet] ADD CONSTRAINT [DF_TemplateSet_ActiveInd] DEFAULT (N'Y') FOR [ActiveInd];
GO
ALTER TABLE [dbo].[TemplateSet] ADD CONSTRAINT [DF_TemplateSet_SortOrder] DEFAULT ((50)) FOR [SortOrder];
GO
ALTER TABLE [dbo].[TemplateSet] ADD CONSTRAINT [DF_TemplateSet_CreateDateTime] DEFAULT (GETDATE()) FOR [CreateDateTime];
GO
ALTER TABLE [dbo].[TemplateSet] ADD CONSTRAINT [DF_TemplateSet_CreateUserName] DEFAULT (N'UNKNOWN') FOR [CreateUserName];
GO
ALTER TABLE [dbo].[TemplateSet] ADD CONSTRAINT [DF_TemplateSet_ChangeDateTime] DEFAULT (GETDATE()) FOR [ChangeDateTime];
GO
ALTER TABLE [dbo].[TemplateSet] ADD CONSTRAINT [DF_TemplateSet_ChangeUserName] DEFAULT (N'UNKNOWN') FOR [ChangeUserName];
GO
