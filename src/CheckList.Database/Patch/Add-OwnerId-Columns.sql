-- Add OwnerId column to CheckSet to store the Entra ID object identifier of the owner.
-- This is nullable so that existing rows are not broken.
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[CheckSet]') AND name = N'OwnerId'
)
BEGIN
    ALTER TABLE [dbo].[CheckSet] ADD [OwnerId] NVARCHAR(256) NULL;
END
GO

-- Add OwnerId column to TemplateSet to store the Entra ID object identifier of the owner.
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[TemplateSet]') AND name = N'OwnerId'
)
BEGIN
    ALTER TABLE [dbo].[TemplateSet] ADD [OwnerId] NVARCHAR(256) NULL;
END
GO
