namespace CheckList.Web.Data.Repositories;

public interface ICheckRepository
{
    /// <summary>Returns all active check sets visible to the given user (owned or shared).</summary>
    Task<List<CheckSet>> GetActiveSetsForUserAsync(string userId);

    /// <summary>Legacy: returns all active check sets without user filtering.</summary>
    Task<List<CheckSet>> GetAllActiveSetsAsync();

    Task<CheckSet?> GetSetWithHierarchyAsync(int setId);
    Task<CheckAction?> GetActionAsync(int actionId);
    Task<CheckAction> ToggleActionAsync(int actionId, string userName);
    Task<CheckSet> ActivateFromTemplateAsync(int templateSetId, string ownerName, List<int>? selectedListIds = null, string? customName = null, string? ownerId = null);
    Task<bool> DeleteSetAsync(int setId);
}
