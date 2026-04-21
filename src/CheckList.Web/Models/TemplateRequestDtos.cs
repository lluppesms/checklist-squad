namespace CheckList.Web.Models;

public record CreateTemplateSetRequest(
    string SetName,
    string SetDscr,
    string OwnerName,
    int SortOrder = 50);

public record UpdateTemplateSetRequest(
    string SetName,
    string SetDscr,
    string OwnerName,
    int SortOrder,
    string ActiveInd);

public record CreateTemplateListRequest(
    string ListName,
    string ListDscr,
    int SortOrder = 50);

public record UpdateTemplateListRequest(
    string ListName,
    string ListDscr,
    int SortOrder,
    string ActiveInd);

public record CreateTemplateCategoryRequest(
    string CategoryText,
    string? CategoryDscr,
    int SortOrder = 50);

public record UpdateTemplateCategoryRequest(
    string CategoryText,
    string? CategoryDscr,
    int SortOrder,
    string ActiveInd);

public record CreateTemplateActionRequest(
    string? ActionText,
    string? ActionDscr,
    int SortOrder = 50);

public record UpdateTemplateActionRequest(
    string? ActionText,
    string? ActionDscr,
    int SortOrder);
