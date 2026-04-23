namespace CheckList.Web.Models.Mapping;

public static class DtoMapper
{
    // Template Set mappings
    public static TemplateSetSummaryDto ToSummaryDto(this TemplateSet entity) =>
        new(entity.SetId, entity.SetName, entity.SetDscr, entity.OwnerName, entity.ActiveInd, entity.SortOrder);

    public static TemplateSetDto ToDto(this TemplateSet entity) =>
        new(entity.SetId, entity.SetName, entity.SetDscr, entity.OwnerName, entity.ActiveInd, entity.SortOrder,
            entity.TemplateLists.Select(l => l.ToDto()).ToList());

    public static TemplateListDto ToDto(this TemplateList entity) =>
        new(entity.ListId, entity.ListName, entity.ListDscr, entity.SortOrder,
            entity.TemplateCategories.Select(c => c.ToDto()).ToList());

    public static TemplateCategoryDto ToDto(this TemplateCategory entity) =>
        new(entity.CategoryId, entity.CategoryText, entity.CategoryDscr, entity.SortOrder,
            entity.TemplateActions.Select(a => a.ToDto()).ToList());

    public static TemplateActionDto ToDto(this TemplateAction entity) =>
        new(entity.ActionId, entity.ActionText, entity.ActionDscr, entity.SortOrder);

    // Template export mappings
    public static TemplateExportDto ToExportDto(this TemplateSet entity)
    {
        var lists = entity.TemplateLists
            .Where(l => l.ActiveInd == "Y")
            .Select(l => l.ToExportDto())
            .ToList();

        var metadata = new ExportMetadata(
            DateTime.UtcNow,
            "1.0.0",
            1 + lists.Sum(l => 1 + l.Categories.Sum(c => 1 + c.Actions.Count)));

        return new TemplateExportDto(
            entity.SetName,
            entity.SetDscr,
            entity.OwnerName,
            entity.SortOrder,
            lists,
            metadata);
    }

    public static TemplateListExportDto ToExportDto(this TemplateList entity) =>
        new(entity.ListName,
            entity.ListDscr,
            entity.SortOrder,
            entity.TemplateCategories
                .Where(c => c.ActiveInd == "Y")
                .Select(c => c.ToExportDto())
                .ToList());

    public static TemplateCategoryExportDto ToExportDto(this TemplateCategory entity) =>
        new(entity.CategoryText,
            entity.CategoryDscr,
            entity.SortOrder,
            entity.TemplateActions.Select(a => a.ToExportDto()).ToList());

    public static TemplateActionExportDto ToExportDto(this TemplateAction entity) =>
        new(entity.ActionText, entity.ActionDscr, entity.SortOrder);

    // CheckSet export mappings
    public static CheckSetExportDto ToExportDto(this CheckSet entity) =>
        new(entity.SetName,
            entity.SetDscr,
            entity.OwnerName,
            entity.SortOrder,
            entity.CheckLists
                .Where(l => l.ActiveInd == "Y")
                .Select(l => l.ToExportDto())
                .ToList());

    public static CheckListExportDto ToExportDto(this CheckListEntity entity) =>
        new(entity.ListName,
            entity.ListDscr,
            entity.SortOrder,
            entity.CheckCategories
                .Where(c => c.ActiveInd == "Y")
                .Select(c => c.ToExportDto())
                .ToList());

    public static CheckCategoryExportDto ToExportDto(this CheckCategory entity) =>
        new(entity.CategoryText,
            entity.CategoryDscr,
            entity.SortOrder,
            entity.CheckActions.Select(a => a.ToExportDto()).ToList());

    public static CheckActionExportDto ToExportDto(this CheckAction entity) =>
        new(entity.ActionText, entity.ActionDscr, entity.CompleteInd, entity.SortOrder);

    // CheckSet regular mappings
    public static CheckSetSummaryDto ToSummaryDto(this CheckSet entity) =>
        new(entity.SetId, entity.TemplateSetId, entity.SetName, entity.SetDscr, entity.OwnerName, entity.OwnerId, entity.ActiveInd, entity.SortOrder, entity.CreateDateTime);

    public static CheckSetDto ToDto(this CheckSet entity) =>
        new(entity.SetId, entity.TemplateSetId, entity.SetName, entity.SetDscr, entity.OwnerName, entity.OwnerId, entity.ActiveInd, entity.SortOrder,
            entity.CheckLists.Select(l => l.ToDto()).ToList());

    public static CheckListDto ToDto(this CheckListEntity entity) =>
        new(entity.ListId, entity.ListName, entity.ListDscr, entity.SortOrder,
            entity.CheckCategories.Select(c => c.ToDto()).ToList());

    public static CheckCategoryDto ToDto(this CheckCategory entity) =>
        new(entity.CategoryId, entity.CategoryText, entity.CategoryDscr, entity.SortOrder,
            entity.CheckActions.Select(a => a.ToDto()).ToList());

    public static CheckActionDto ToDto(this CheckAction entity) =>
        new(entity.ActionId, entity.ActionText, entity.ActionDscr, entity.CompleteInd, entity.ChangeUserName, entity.SortOrder);
}
