CREATE TABLE [CheckList].[AppUser]
(
    [UserId]            NVARCHAR(256) NOT NULL,
    [DisplayName]       NVARCHAR(256) NOT NULL,
    [Email]             NVARCHAR(256) NULL,
    [CreateDateTime]    DATETIME      NOT NULL,
    [LastLoginDateTime] DATETIME      NOT NULL,
    CONSTRAINT [PK_AppUser] PRIMARY KEY CLUSTERED ([UserId] ASC)
);
GO

ALTER TABLE [CheckList].[AppUser] ADD CONSTRAINT [DF_AppUser_CreateDateTime]    DEFAULT (GETDATE()) FOR [CreateDateTime];
GO
ALTER TABLE [CheckList].[AppUser] ADD CONSTRAINT [DF_AppUser_LastLoginDateTime] DEFAULT (GETDATE()) FOR [LastLoginDateTime];
GO
