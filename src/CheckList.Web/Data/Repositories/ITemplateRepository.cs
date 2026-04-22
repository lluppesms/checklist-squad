namespace CheckList.Web.Data.Repositories;

using CheckList.Web.Models;

public interface ITemplateRepository
{
    // Read operations
    Task<List<TemplateSet>> GetAllSetsAsync();
    Task<TemplateSet?> GetSetWithHierarchyAsync(int setId);

    // Template Set CRUD
    Task<TemplateSetDto> CreateSetAsync(CreateTemplateSetRequest request, string userName);
    Task<TemplateSetDto?> UpdateSetAsync(int setId, UpdateTemplateSetRequest request, string userName);
    Task<bool> DeleteSetAsync(int setId);

    // Template List CRUD
    Task<TemplateListDto> CreateListAsync(int setId, CreateTemplateListRequest request, string userName);
    Task<TemplateListDto?> UpdateListAsync(int listId, UpdateTemplateListRequest request, string userName);
    Task<bool> DeleteListAsync(int listId);

    // Template Category CRUD
    Task<TemplateCategoryDto> CreateCategoryAsync(int listId, CreateTemplateCategoryRequest request, string userName);
    Task<TemplateCategoryDto?> UpdateCategoryAsync(int categoryId, UpdateTemplateCategoryRequest request, string userName);
    Task<bool> DeleteCategoryAsync(int categoryId);

    // Template Action CRUD
    Task<TemplateActionDto> CreateActionAsync(int categoryId, CreateTemplateActionRequest request, string userName);
    Task<TemplateActionDto?> UpdateActionAsync(int actionId, UpdateTemplateActionRequest request, string userName);
    Task<bool> DeleteActionAsync(int actionId);

    // Import/Export
    Task<TemplateExportDto> ExportSetAsync(int setId);
    Task<List<TemplateExportDto>> ExportAllAsync();
    Task<TemplateSetDto> ImportSetAsync(TemplateImportDto import, string userName);
    Task<FullExportDto> FullExportAsync();
    Task<(int templateCount, int checklistCount)> FullImportAsync(FullImportDto import, string userName);
}
