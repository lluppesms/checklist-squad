namespace CheckList.Web.Models;

public record TemplateExportDto(
    ExportMetadata Metadata,
    string SetName,
    string SetDscr,
    string OwnerName,
    int SortOrder,
    List<TemplateListExportDto> Lists);

public record TemplateListExportDto(
    string ListName,
    string ListDscr,
    int SortOrder,
    List<TemplateCategoryExportDto> Categories);

public record TemplateCategoryExportDto(
    string CategoryText,
    string? CategoryDscr,
    int SortOrder,
    List<TemplateActionExportDto> Actions);

public record TemplateActionExportDto(
    string? ActionText,
    string? ActionDscr,
    int SortOrder);

public record ExportMetadata(
    string ExportedAt,
    string ExportedBy,
    string Version);

public record TemplateImportDto(
    string SetName,
    string SetDscr,
    string OwnerName,
    int SortOrder,
    List<TemplateListImportDto> Lists);

public record TemplateListImportDto(
    string ListName,
    string ListDscr,
    int SortOrder,
    List<TemplateCategoryImportDto> Categories);

public record TemplateCategoryImportDto(
    string CategoryText,
    string? CategoryDscr,
    int SortOrder,
    List<TemplateActionImportDto> Actions);

public record TemplateActionImportDto(
    string? ActionText,
    string? ActionDscr,
    int SortOrder);

public record FullExportDto(
    ExportMetadata Metadata,
    List<TemplateExportDto> Templates,
    List<CheckSetExportDto> Checklists);

public record CheckSetExportDto(
    string SetName,
    string SetDscr,
    string OwnerName,
    int SortOrder,
    List<CheckListExportDto> Lists);

public record CheckListExportDto(
    string ListName,
    string ListDscr,
    int SortOrder,
    List<CheckCategoryExportDto> Categories);

public record CheckCategoryExportDto(
    string CategoryText,
    string? CategoryDscr,
    int SortOrder,
    List<CheckActionExportDto> Actions);

public record CheckActionExportDto(
    string? ActionText,
    string? ActionDscr,
    int SortOrder,
    string? CompletedBy,
    DateTime? CompletedDt);

public record FullImportDto(
    List<TemplateImportDto> Templates,
    List<CheckSetImportDto> Checklists);

public record CheckSetImportDto(
    string SetName,
    string SetDscr,
    string OwnerName,
    int SortOrder,
    List<CheckListImportDto> Lists);

public record CheckListImportDto(
    string ListName,
    string ListDscr,
    int SortOrder,
    List<CheckCategoryImportDto> Categories);

public record CheckCategoryImportDto(
    string CategoryText,
    string? CategoryDscr,
    int SortOrder,
    List<CheckActionImportDto> Actions);

public record CheckActionImportDto(
    string? ActionText,
    string? ActionDscr,
    int SortOrder,
    string? CompletedBy,
    DateTime? CompletedDt);
