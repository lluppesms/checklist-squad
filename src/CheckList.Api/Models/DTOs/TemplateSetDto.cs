namespace CheckList.Api.Models.DTOs;

public record TemplateSetDto(
    int SetId,
    string SetName,
    string SetDscr,
    string OwnerName,
    string ActiveInd,
    int SortOrder,
    List<TemplateListDto> Lists);

public record TemplateListDto(
    int ListId,
    string ListName,
    string ListDscr,
    int SortOrder,
    List<TemplateCategoryDto> Categories);

public record TemplateCategoryDto(
    int CategoryId,
    string CategoryText,
    string? CategoryDscr,
    int SortOrder,
    List<TemplateActionDto> Actions);

public record TemplateActionDto(
    int ActionId,
    string? ActionText,
    string? ActionDscr,
    int SortOrder);

public record TemplateSetSummaryDto(
    int SetId,
    string SetName,
    string SetDscr,
    string OwnerName,
    string ActiveInd,
    int SortOrder);
