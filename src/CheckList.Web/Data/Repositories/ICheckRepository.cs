namespace CheckList.Web.Data.Repositories;

public interface ICheckRepository
{
    Task<List<CheckSet>> GetAllActiveSetsAsync();
    Task<CheckSet?> GetSetWithHierarchyAsync(int setId);
    Task<CheckAction?> GetActionAsync(int actionId);
    Task<CheckAction> ToggleActionAsync(int actionId, string userName);
    Task<CheckSet> ActivateFromTemplateAsync(int templateSetId, string ownerName, List<int>? selectedListIds = null, string? customName = null);
    Task<bool> DeleteSetAsync(int setId);
}
