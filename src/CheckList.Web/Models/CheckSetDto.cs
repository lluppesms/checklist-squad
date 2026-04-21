namespace CheckList.Web.Models;

public record CheckSetSummaryDto(
    int SetId,
    int? TemplateSetId,
    string SetName,
    string? SetDscr,
    string OwnerName,
    string ActiveInd,
    int SortOrder,
    DateTime CreateDateTime);

public record CheckSetDto(
    int SetId,
    int? TemplateSetId,
    string SetName,
    string? SetDscr,
    string OwnerName,
    string ActiveInd,
    int SortOrder,
    List<CheckListDto> Lists);

public record CheckListDto(
    int ListId,
    string ListName,
    string? ListDscr,
    int SortOrder,
    List<CheckCategoryDto> Categories);

public record CheckCategoryDto(
    int CategoryId,
    string CategoryText,
    string? CategoryDscr,
    int SortOrder,
    List<CheckActionDto> Actions);

public record CheckActionDto(
    int ActionId,
    string ActionText,
    string? ActionDscr,
    string CompleteInd,
    int SortOrder);
