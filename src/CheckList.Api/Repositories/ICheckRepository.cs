namespace CheckList.Api.Repositories;

public interface ICheckRepository
{
    Task<List<CheckSet>> GetAllActiveSetsAsync();
    Task<CheckSet?> GetSetWithHierarchyAsync(int setId);
    Task<CheckAction?> GetActionAsync(int actionId);
    Task<CheckAction> ToggleActionAsync(int actionId, string userName);
    Task<CheckSet> ActivateFromTemplateAsync(int templateSetId, string ownerName, List<int>? selectedListIds = null);
    Task<bool> DeleteSetAsync(int setId);
}
