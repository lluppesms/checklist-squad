CREATE PROCEDURE [dbo].[usp_CopyTemplateToCheck]
    @TemplateSetId INT,
    @OwnerName NVARCHAR(256),
    @NewCheckSetId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME = GETDATE();

    -- Copy TemplateSet to CheckSet
    INSERT INTO [dbo].[CheckSet] ([TemplateSetId], [SetName], [SetDscr], [OwnerName], [ActiveInd], [SortOrder], [CreateDateTime], [CreateUserName], [ChangeDateTime], [ChangeUserName])
    SELECT [SetId], [SetName], [SetDscr], @OwnerName, [ActiveInd], [SortOrder], @Now, @OwnerName, @Now, @OwnerName
    FROM [dbo].[TemplateSet]
    WHERE [SetId] = @TemplateSetId;

    SET @NewCheckSetId = SCOPE_IDENTITY();

    -- Copy TemplateLists to CheckLists using cursor for proper ID mapping
    DECLARE @OldListId INT, @NewListId INT;
    DECLARE list_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT [ListId] FROM [dbo].[TemplateList] WHERE [SetId] = @TemplateSetId ORDER BY [SortOrder];

    OPEN list_cursor;
    FETCH NEXT FROM list_cursor INTO @OldListId;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        INSERT INTO [dbo].[CheckList] ([SetId], [ListName], [ListDscr], [ActiveInd], [SortOrder], [CreateDateTime], [CreateUserName], [ChangeDateTime], [ChangeUserName])
        SELECT @NewCheckSetId, [ListName], ISNULL([ListDscr], N''), [ActiveInd], [SortOrder], @Now, @OwnerName, @Now, @OwnerName
        FROM [dbo].[TemplateList] WHERE [ListId] = @OldListId;

        SET @NewListId = SCOPE_IDENTITY();

        -- Copy categories for this list
        DECLARE @OldCatId INT, @NewCatId INT;

        DECLARE cat_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT [CategoryId] FROM [dbo].[TemplateCategory] WHERE [ListId] = @OldListId ORDER BY [SortOrder];

        OPEN cat_cursor;
        FETCH NEXT FROM cat_cursor INTO @OldCatId;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            INSERT INTO [dbo].[CheckCategory] ([ListId], [CategoryText], [CategoryDscr], [ActiveInd], [SortOrder], [CreateDateTime], [CreateUserName], [ChangeDateTime], [ChangeUserName])
            SELECT @NewListId, [CategoryText], [CategoryDscr], [ActiveInd], [SortOrder], @Now, @OwnerName, @Now, @OwnerName
            FROM [dbo].[TemplateCategory] WHERE [CategoryId] = @OldCatId;

            SET @NewCatId = SCOPE_IDENTITY();

            -- Copy actions for this category
            INSERT INTO [dbo].[CheckAction] ([CategoryId], [ListId], [ActionText], [ActionDscr], [CompleteInd], [SortOrder], [CreateDateTime], [CreateUserName], [ChangeDateTime], [ChangeUserName])
            SELECT @NewCatId, @NewListId, [ActionText], [ActionDscr], N'N', [SortOrder], @Now, @OwnerName, @Now, @OwnerName
            FROM [dbo].[TemplateAction] WHERE [CategoryId] = @OldCatId ORDER BY [SortOrder];

            FETCH NEXT FROM cat_cursor INTO @OldCatId;
        END

        CLOSE cat_cursor;
        DEALLOCATE cat_cursor;

        FETCH NEXT FROM list_cursor INTO @OldListId;
    END

    CLOSE list_cursor;
    DEALLOCATE list_cursor;
END
GO
