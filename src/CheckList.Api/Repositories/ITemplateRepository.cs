namespace CheckList.Api.Repositories;

public interface ITemplateRepository
{
    Task<List<TemplateSet>> GetAllSetsAsync();
    Task<TemplateSet?> GetSetWithHierarchyAsync(int setId);
}
