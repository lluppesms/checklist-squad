using CheckList.Web.Data.Repositories;
using CheckList.Web.Models;
using CheckList.Web.Models.Mapping;

namespace CheckList.Web.Services;

public class CheckListService(
    ITemplateRepository templateRepo,
    ICheckRepository checkRepo) : ICheckListApiClient
{
    private const string DefaultUserName = "System";

    public async Task<List<TemplateSetSummaryDto>> GetTemplatesAsync()
    {
        var sets = await templateRepo.GetAllSetsAsync();
        return sets.Select(s => s.ToSummaryDto()).ToList();
    }

    public async Task<TemplateSetDto?> GetTemplateAsync(int setId)
    {
        var set = await templateRepo.GetSetWithHierarchyAsync(setId);
        return set?.ToDto();
    }

    public async Task<CheckSetDto?> ActivateCheckSetAsync(int templateSetId, string ownerName, List<int>? selectedListIds = null)
    {
        var checkSet = await checkRepo.ActivateFromTemplateAsync(templateSetId, ownerName, selectedListIds);
        return checkSet.ToDto();
    }

    public async Task<List<CheckSetSummaryDto>> GetActiveCheckSetsAsync()
    {
        var sets = await checkRepo.GetAllActiveSetsAsync();
        return sets.Select(s => s.ToSummaryDto()).ToList();
    }

    public async Task<CheckSetDto?> GetCheckSetAsync(int setId)
    {
        var set = await checkRepo.GetSetWithHierarchyAsync(setId);
        return set?.ToDto();
    }

    public async Task<CheckActionDto?> ToggleActionAsync(int actionId, string userName)
    {
        var action = await checkRepo.ToggleActionAsync(actionId, userName);
        return action.ToDto();
    }

    public async Task DeleteCheckSetAsync(int setId)
    {
        await checkRepo.DeleteSetAsync(setId);
    }

    // Template CRUD
    public async Task<TemplateSetDto> CreateTemplateSetAsync(CreateTemplateSetRequest request)
    {
        return await templateRepo.CreateSetAsync(request, DefaultUserName);
    }

    public async Task<TemplateSetDto> UpdateTemplateSetAsync(int setId, UpdateTemplateSetRequest request)
    {
        return await templateRepo.UpdateSetAsync(setId, request, DefaultUserName)
            ?? throw new InvalidOperationException($"Template set {setId} not found");
    }

    public async Task DeleteTemplateSetAsync(int setId)
    {
        await templateRepo.DeleteSetAsync(setId);
    }

    // Template List CRUD
    public async Task<TemplateListDto> CreateTemplateListAsync(int setId, CreateTemplateListRequest request)
    {
        return await templateRepo.CreateListAsync(setId, request, DefaultUserName);
    }

    public async Task<TemplateListDto> UpdateTemplateListAsync(int listId, UpdateTemplateListRequest request)
    {
        return await templateRepo.UpdateListAsync(listId, request, DefaultUserName)
            ?? throw new InvalidOperationException($"Template list {listId} not found");
    }

    public async Task DeleteTemplateListAsync(int listId)
    {
        await templateRepo.DeleteListAsync(listId);
    }

    // Template Category CRUD
    public async Task<TemplateCategoryDto> CreateTemplateCategoryAsync(int listId, CreateTemplateCategoryRequest request)
    {
        return await templateRepo.CreateCategoryAsync(listId, request, DefaultUserName);
    }

    public async Task<TemplateCategoryDto> UpdateTemplateCategoryAsync(int categoryId, UpdateTemplateCategoryRequest request)
    {
        return await templateRepo.UpdateCategoryAsync(categoryId, request, DefaultUserName)
            ?? throw new InvalidOperationException($"Template category {categoryId} not found");
    }

    public async Task DeleteTemplateCategoryAsync(int categoryId)
    {
        await templateRepo.DeleteCategoryAsync(categoryId);
    }

    // Template Action CRUD
    public async Task<TemplateActionDto> CreateTemplateActionAsync(int categoryId, CreateTemplateActionRequest request)
    {
        return await templateRepo.CreateActionAsync(categoryId, request, DefaultUserName);
    }

    public async Task<TemplateActionDto> UpdateTemplateActionAsync(int actionId, UpdateTemplateActionRequest request)
    {
        return await templateRepo.UpdateActionAsync(actionId, request, DefaultUserName)
            ?? throw new InvalidOperationException($"Template action {actionId} not found");
    }

    public async Task DeleteTemplateActionAsync(int actionId)
    {
        await templateRepo.DeleteActionAsync(actionId);
    }

    // Import/Export
    public async Task<TemplateExportDto> ExportTemplateAsync(int setId)
    {
        return await templateRepo.ExportSetAsync(setId);
    }

    public async Task<List<TemplateExportDto>> ExportAllTemplatesAsync()
    {
        return await templateRepo.ExportAllAsync();
    }

    public async Task<TemplateSetDto> ImportTemplateAsync(TemplateImportDto importDto)
    {
        return await templateRepo.ImportSetAsync(importDto, DefaultUserName);
    }

    public async Task<FullExportDto> ExportFullAsync()
    {
        return await templateRepo.FullExportAsync();
    }

    public async Task<(int templateCount, int checklistCount)> ImportFullAsync(FullImportDto importDto)
    {
        return await templateRepo.FullImportAsync(importDto, DefaultUserName);
    }
}
