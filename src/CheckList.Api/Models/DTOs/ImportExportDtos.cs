namespace CheckList.Api.Models.DTOs;

using System.Text.Json.Serialization;

public record TemplateExportDto(
    [property: JsonPropertyName("setName")] string SetName,
    [property: JsonPropertyName("setDscr")] string SetDscr,
    [property: JsonPropertyName("ownerName")] string OwnerName,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("lists")] List<TemplateListExportDto> Lists,
    [property: JsonPropertyName("metadata")] ExportMetadata Metadata);

public record TemplateListExportDto(
    [property: JsonPropertyName("listName")] string ListName,
    [property: JsonPropertyName("listDscr")] string ListDscr,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("categories")] List<TemplateCategoryExportDto> Categories);

public record TemplateCategoryExportDto(
    [property: JsonPropertyName("categoryText")] string CategoryText,
    [property: JsonPropertyName("categoryDscr")] string? CategoryDscr,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("actions")] List<TemplateActionExportDto> Actions);

public record TemplateActionExportDto(
    [property: JsonPropertyName("actionText")] string? ActionText,
    [property: JsonPropertyName("actionDscr")] string? ActionDscr,
    [property: JsonPropertyName("sortOrder")] int SortOrder);

public record ExportMetadata(
    [property: JsonPropertyName("exportDate")] DateTime ExportDate,
    [property: JsonPropertyName("appVersion")] string AppVersion,
    [property: JsonPropertyName("itemCount")] int ItemCount);

public record TemplateImportDto(
    [property: JsonPropertyName("setName")] string SetName,
    [property: JsonPropertyName("setDscr")] string SetDscr,
    [property: JsonPropertyName("ownerName")] string OwnerName,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("lists")] List<TemplateListImportDto> Lists);

public record TemplateListImportDto(
    [property: JsonPropertyName("listName")] string ListName,
    [property: JsonPropertyName("listDscr")] string ListDscr,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("categories")] List<TemplateCategoryImportDto> Categories);

public record TemplateCategoryImportDto(
    [property: JsonPropertyName("categoryText")] string CategoryText,
    [property: JsonPropertyName("categoryDscr")] string? CategoryDscr,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("actions")] List<TemplateActionImportDto> Actions);

public record TemplateActionImportDto(
    [property: JsonPropertyName("actionText")] string? ActionText,
    [property: JsonPropertyName("actionDscr")] string? ActionDscr,
    [property: JsonPropertyName("sortOrder")] int SortOrder);

public record FullExportDto(
    [property: JsonPropertyName("templates")] List<TemplateExportDto> Templates,
    [property: JsonPropertyName("checklists")] List<CheckSetExportDto> Checklists,
    [property: JsonPropertyName("metadata")] ExportMetadata Metadata);

public record CheckSetExportDto(
    [property: JsonPropertyName("setName")] string SetName,
    [property: JsonPropertyName("setDscr")] string? SetDscr,
    [property: JsonPropertyName("ownerName")] string OwnerName,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("lists")] List<CheckListExportDto> Lists);

public record CheckListExportDto(
    [property: JsonPropertyName("listName")] string ListName,
    [property: JsonPropertyName("listDscr")] string? ListDscr,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("categories")] List<CheckCategoryExportDto> Categories);

public record CheckCategoryExportDto(
    [property: JsonPropertyName("categoryText")] string CategoryText,
    [property: JsonPropertyName("categoryDscr")] string? CategoryDscr,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("actions")] List<CheckActionExportDto> Actions);

public record CheckActionExportDto(
    [property: JsonPropertyName("actionText")] string ActionText,
    [property: JsonPropertyName("actionDscr")] string? ActionDscr,
    [property: JsonPropertyName("completeInd")] string CompleteInd,
    [property: JsonPropertyName("sortOrder")] int SortOrder);

public record FullImportDto(
    [property: JsonPropertyName("templates")] List<TemplateImportDto> Templates,
    [property: JsonPropertyName("checklists")] List<CheckSetImportDto> Checklists);

public record CheckSetImportDto(
    [property: JsonPropertyName("setName")] string SetName,
    [property: JsonPropertyName("setDscr")] string? SetDscr,
    [property: JsonPropertyName("ownerName")] string OwnerName,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("lists")] List<CheckListImportDto> Lists);

public record CheckListImportDto(
    [property: JsonPropertyName("listName")] string ListName,
    [property: JsonPropertyName("listDscr")] string? ListDscr,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("categories")] List<CheckCategoryImportDto> Categories);

public record CheckCategoryImportDto(
    [property: JsonPropertyName("categoryText")] string CategoryText,
    [property: JsonPropertyName("categoryDscr")] string? CategoryDscr,
    [property: JsonPropertyName("sortOrder")] int SortOrder,
    [property: JsonPropertyName("actions")] List<CheckActionImportDto> Actions);

public record CheckActionImportDto(
    [property: JsonPropertyName("actionText")] string ActionText,
    [property: JsonPropertyName("actionDscr")] string? ActionDscr,
    [property: JsonPropertyName("completeInd")] string CompleteInd,
    [property: JsonPropertyName("sortOrder")] int SortOrder);
