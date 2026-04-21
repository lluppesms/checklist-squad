using CheckList.Web.Models;

namespace CheckList.Web.Services;

public interface ICheckListApiClient
{
    Task<List<TemplateSetSummaryDto>> GetTemplatesAsync();
    Task<TemplateSetDto?> GetTemplateAsync(int setId);
    Task<CheckSetDto?> ActivateCheckSetAsync(int templateSetId, string ownerName);
    Task<List<CheckSetSummaryDto>> GetActiveCheckSetsAsync();
    Task<CheckSetDto?> GetCheckSetAsync(int setId);
    Task<CheckActionDto?> ToggleActionAsync(int actionId, string userName);
    Task DeleteCheckSetAsync(int setId);
}
