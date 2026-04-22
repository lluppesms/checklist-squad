using CheckList.Web.Models;

namespace CheckList.Web.Services;

public interface ICheckListApiClient
{
    Task<List<TemplateSetSummaryDto>> GetTemplatesAsync();
    Task<TemplateSetDto?> GetTemplateAsync(int setId);
    Task<CheckSetDto?> ActivateCheckSetAsync(int templateSetId, string ownerName, List<int>? selectedListIds = null);
    Task<List<CheckSetSummaryDto>> GetActiveCheckSetsAsync();
    Task<CheckSetDto?> GetCheckSetAsync(int setId);
    Task<CheckActionDto?> ToggleActionAsync(int actionId, string userName);
    Task DeleteCheckSetAsync(int setId);

    // Template CRUD
    Task<TemplateSetDto> CreateTemplateSetAsync(CreateTemplateSetRequest request);
    Task<TemplateSetDto> UpdateTemplateSetAsync(int setId, UpdateTemplateSetRequest request);
    Task DeleteTemplateSetAsync(int setId);

    // Template List CRUD
    Task<TemplateListDto> CreateTemplateListAsync(int setId, CreateTemplateListRequest request);
    Task<TemplateListDto> UpdateTemplateListAsync(int listId, UpdateTemplateListRequest request);
    Task DeleteTemplateListAsync(int listId);

    // Template Category CRUD
    Task<TemplateCategoryDto> CreateTemplateCategoryAsync(int listId, CreateTemplateCategoryRequest request);
    Task<TemplateCategoryDto> UpdateTemplateCategoryAsync(int categoryId, UpdateTemplateCategoryRequest request);
    Task DeleteTemplateCategoryAsync(int categoryId);

    // Template Action CRUD
    Task<TemplateActionDto> CreateTemplateActionAsync(int categoryId, CreateTemplateActionRequest request);
    Task<TemplateActionDto> UpdateTemplateActionAsync(int actionId, UpdateTemplateActionRequest request);
    Task DeleteTemplateActionAsync(int actionId);

    // Import/Export
    Task<TemplateExportDto> ExportTemplateAsync(int setId);
    Task<List<TemplateExportDto>> ExportAllTemplatesAsync();
    Task<TemplateSetDto> ImportTemplateAsync(TemplateImportDto importDto);
    Task<FullExportDto> ExportFullAsync();
    Task<object> ImportFullAsync(FullImportDto importDto);
}
