namespace CheckList.Api.Models.Mapping;

using CheckList.Api.Models.DTOs;

public static class DtoMapper
{
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

    public static CheckSetSummaryDto ToSummaryDto(this CheckSet entity) =>
        new(entity.SetId, entity.TemplateSetId, entity.SetName, entity.SetDscr, entity.OwnerName, entity.ActiveInd, entity.SortOrder, entity.CreateDateTime);

    public static CheckSetDto ToDto(this CheckSet entity) =>
        new(entity.SetId, entity.TemplateSetId, entity.SetName, entity.SetDscr, entity.OwnerName, entity.ActiveInd, entity.SortOrder,
            entity.CheckLists.Select(l => l.ToDto()).ToList());

    public static CheckListDto ToDto(this CheckListEntity entity) =>
        new(entity.ListId, entity.ListName, entity.ListDscr, entity.SortOrder,
            entity.CheckCategories.Select(c => c.ToDto()).ToList());

    public static CheckCategoryDto ToDto(this CheckCategory entity) =>
        new(entity.CategoryId, entity.CategoryText, entity.CategoryDscr, entity.SortOrder,
            entity.CheckActions.Select(a => a.ToDto()).ToList());

    public static CheckActionDto ToDto(this CheckAction entity) =>
        new(entity.ActionId, entity.ActionText, entity.ActionDscr, entity.CompleteInd, entity.SortOrder);
}
