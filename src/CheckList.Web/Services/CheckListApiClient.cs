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

    public async Task<CheckSetDto?> ActivateCheckSetAsync(int templateSetId, string ownerName)
    {
        var request = new ActivateCheckSetRequest(ownerName);
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
}
