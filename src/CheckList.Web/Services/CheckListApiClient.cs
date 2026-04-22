using System.Net.Http.Json;
using CheckList.Web.Models;

namespace CheckList.Web.Services;

public class CheckListApiClient(HttpClient httpClient) : ICheckListApiClient
{
    public async Task<List<TemplateSetSummaryDto>> GetTemplatesAsync()
    {
        var result = await httpClient.GetFromJsonAsync<List<TemplateSetSummaryDto>>("api/templates");
        return result ?? [];
    }

    public async Task<TemplateSetDto?> GetTemplateAsync(int setId)
    {
        return await httpClient.GetFromJsonAsync<TemplateSetDto>($"api/templates/{setId}");
    }

    public async Task<CheckSetDto?> ActivateCheckSetAsync(int templateSetId, string ownerName, List<int>? selectedListIds = null)
    {
        var request = new ActivateCheckSetRequest(ownerName, selectedListIds);
        var response = await httpClient.PostAsJsonAsync($"api/checklists/activate/{templateSetId}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CheckSetDto>();
    }

    public async Task<List<CheckSetSummaryDto>> GetActiveCheckSetsAsync()
    {
        var result = await httpClient.GetFromJsonAsync<List<CheckSetSummaryDto>>("api/checklists");
        return result ?? [];
    }

    public async Task<CheckSetDto?> GetCheckSetAsync(int setId)
    {
        return await httpClient.GetFromJsonAsync<CheckSetDto>($"api/checklists/{setId}");
    }

    public async Task<CheckActionDto?> ToggleActionAsync(int actionId, string userName)
    {
        var request = new ToggleActionRequest(userName);
        var response = await httpClient.PutAsJsonAsync($"api/checklists/actions/{actionId}/toggle", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CheckActionDto>();
    }

    public async Task DeleteCheckSetAsync(int setId)
    {
        var response = await httpClient.DeleteAsync($"api/checklists/{setId}");
        response.EnsureSuccessStatusCode();
    }

    // Template CRUD
    public async Task<TemplateSetDto> CreateTemplateSetAsync(CreateTemplateSetRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("api/templates", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TemplateSetDto>()
            ?? throw new InvalidOperationException("Failed to create template set");
    }

    public async Task<TemplateSetDto> UpdateTemplateSetAsync(int setId, UpdateTemplateSetRequest request)
    {
        var response = await httpClient.PutAsJsonAsync($"api/templates/{setId}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TemplateSetDto>()
            ?? throw new InvalidOperationException("Failed to update template set");
    }

    public async Task DeleteTemplateSetAsync(int setId)
    {
        var response = await httpClient.DeleteAsync($"api/templates/{setId}");
        response.EnsureSuccessStatusCode();
    }

    // Template List CRUD
    public async Task<TemplateListDto> CreateTemplateListAsync(int setId, CreateTemplateListRequest request)
    {
        var response = await httpClient.PostAsJsonAsync($"api/templates/{setId}/lists", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TemplateListDto>()
            ?? throw new InvalidOperationException("Failed to create template list");
    }

    public async Task<TemplateListDto> UpdateTemplateListAsync(int listId, UpdateTemplateListRequest request)
    {
        var response = await httpClient.PutAsJsonAsync($"api/templates/lists/{listId}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TemplateListDto>()
            ?? throw new InvalidOperationException("Failed to update template list");
    }

    public async Task DeleteTemplateListAsync(int listId)
    {
        var response = await httpClient.DeleteAsync($"api/templates/lists/{listId}");
        response.EnsureSuccessStatusCode();
    }

    // Template Category CRUD
    public async Task<TemplateCategoryDto> CreateTemplateCategoryAsync(int listId, CreateTemplateCategoryRequest request)
    {
        var response = await httpClient.PostAsJsonAsync($"api/templates/lists/{listId}/categories", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TemplateCategoryDto>()
            ?? throw new InvalidOperationException("Failed to create template category");
    }

    public async Task<TemplateCategoryDto> UpdateTemplateCategoryAsync(int categoryId, UpdateTemplateCategoryRequest request)
    {
        var response = await httpClient.PutAsJsonAsync($"api/templates/categories/{categoryId}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TemplateCategoryDto>()
            ?? throw new InvalidOperationException("Failed to update template category");
    }

    public async Task DeleteTemplateCategoryAsync(int categoryId)
    {
        var response = await httpClient.DeleteAsync($"api/templates/categories/{categoryId}");
        response.EnsureSuccessStatusCode();
    }

    // Template Action CRUD
    public async Task<TemplateActionDto> CreateTemplateActionAsync(int categoryId, CreateTemplateActionRequest request)
    {
        var response = await httpClient.PostAsJsonAsync($"api/templates/categories/{categoryId}/actions", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TemplateActionDto>()
            ?? throw new InvalidOperationException("Failed to create template action");
    }

    public async Task<TemplateActionDto> UpdateTemplateActionAsync(int actionId, UpdateTemplateActionRequest request)
    {
        var response = await httpClient.PutAsJsonAsync($"api/templates/actions/{actionId}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TemplateActionDto>()
            ?? throw new InvalidOperationException("Failed to update template action");
    }

    public async Task DeleteTemplateActionAsync(int actionId)
    {
        var response = await httpClient.DeleteAsync($"api/templates/actions/{actionId}");
        response.EnsureSuccessStatusCode();
    }

    // Import/Export
    public async Task<TemplateExportDto> ExportTemplateAsync(int setId)
    {
        return await httpClient.GetFromJsonAsync<TemplateExportDto>($"api/templates/{setId}/export")
            ?? throw new InvalidOperationException("Failed to export template");
    }

    public async Task<List<TemplateExportDto>> ExportAllTemplatesAsync()
    {
        var result = await httpClient.GetFromJsonAsync<List<TemplateExportDto>>("api/templates/export");
        return result ?? [];
    }

    public async Task<TemplateSetDto> ImportTemplateAsync(TemplateImportDto importDto)
    {
        var response = await httpClient.PostAsJsonAsync("api/templates/import", importDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TemplateSetDto>()
            ?? throw new InvalidOperationException("Failed to import template");
    }

    public async Task<FullExportDto> ExportFullAsync()
    {
        return await httpClient.GetFromJsonAsync<FullExportDto>("api/export/full")
            ?? throw new InvalidOperationException("Failed to export full data");
    }

    public async Task<object> ImportFullAsync(FullImportDto importDto)
    {
        var response = await httpClient.PostAsJsonAsync("api/import/full", importDto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<object>()
            ?? throw new InvalidOperationException("Failed to import full data");
    }
}
