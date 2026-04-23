CREATE TABLE [CheckList].[UserPartnership]
(
    [PartnershipId]        INT           IDENTITY(1,1) NOT NULL,
    [UserId]               NVARCHAR(256) NOT NULL,
    [PartnerUserId]        NVARCHAR(256) NOT NULL,
    [Role]                 NVARCHAR(50)  NOT NULL,
    [AutoShareEnabled]     BIT           NOT NULL,
    [CreatedFromInviteId]  INT           NULL,
    [CreatedAt]            DATETIME      NOT NULL,
    CONSTRAINT [PK_UserPartnership]              PRIMARY KEY CLUSTERED ([PartnershipId] ASC),
    CONSTRAINT [FK_UserPartnership_User]         FOREIGN KEY ([UserId])              REFERENCES [CheckList].[AppUser]      ([UserId])   ON DELETE CASCADE,
    CONSTRAINT [FK_UserPartnership_Partner]      FOREIGN KEY ([PartnerUserId])       REFERENCES [CheckList].[AppUser]      ([UserId])   ON DELETE NO ACTION,
    CONSTRAINT [FK_UserPartnership_Invite]       FOREIGN KEY ([CreatedFromInviteId]) REFERENCES [CheckList].[SharingInvite] ([InviteId]) ON DELETE NO ACTION,
    CONSTRAINT [UQ_UserPartnership_UserPartner]  UNIQUE ([UserId], [PartnerUserId])
);
GO

ALTER TABLE [CheckList].[UserPartnership] ADD CONSTRAINT [DF_UserPartnership_Role]            DEFAULT (N'user')   FOR [Role];
GO
ALTER TABLE [CheckList].[UserPartnership] ADD CONSTRAINT [DF_UserPartnership_AutoShareEnabled] DEFAULT (1)         FOR [AutoShareEnabled];
GO
ALTER TABLE [CheckList].[UserPartnership] ADD CONSTRAINT [DF_UserPartnership_CreatedAt]        DEFAULT (GETDATE()) FOR [CreatedAt];
GO
