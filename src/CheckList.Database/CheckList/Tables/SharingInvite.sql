CREATE TABLE [CheckList].[SharingInvite]
(
    [InviteId]         INT           IDENTITY(1,1) NOT NULL,
    [InviteTokenHash]  NVARCHAR(128) NOT NULL,
    [SenderUserId]     NVARCHAR(256) NOT NULL,
    [RecipientEmail]   NVARCHAR(256) NOT NULL,
    [Role]             NVARCHAR(50)  NOT NULL,
    [Status]           NVARCHAR(50)  NOT NULL,
    [ExpiresAt]        DATETIME      NOT NULL,
    [CreatedAt]        DATETIME      NOT NULL,
    [AcceptedByUserId] NVARCHAR(256) NULL,
    [AcceptedAt]       DATETIME      NULL,
    CONSTRAINT [PK_SharingInvite]             PRIMARY KEY CLUSTERED ([InviteId] ASC),
    CONSTRAINT [FK_SharingInvite_Sender]      FOREIGN KEY ([SenderUserId])     REFERENCES [CheckList].[AppUser] ([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_SharingInvite_AcceptedBy]  FOREIGN KEY ([AcceptedByUserId]) REFERENCES [CheckList].[AppUser] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [UQ_SharingInvite_TokenHash]   UNIQUE ([InviteTokenHash])
);
GO

ALTER TABLE [CheckList].[SharingInvite] ADD CONSTRAINT [DF_SharingInvite_Role]      DEFAULT (N'user')    FOR [Role];
GO
ALTER TABLE [CheckList].[SharingInvite] ADD CONSTRAINT [DF_SharingInvite_Status]    DEFAULT (N'pending') FOR [Status];
GO
ALTER TABLE [CheckList].[SharingInvite] ADD CONSTRAINT [DF_SharingInvite_CreatedAt] DEFAULT (GETDATE())  FOR [CreatedAt];
GO
