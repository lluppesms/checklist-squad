namespace CheckList.Api.Repositories;

public class TemplateRepository(CheckListDbContext db) : ITemplateRepository
{
    public async Task<List<TemplateSet>> GetAllSetsAsync()
    {
        return await db.TemplateSets
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.SetName)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TemplateSet?> GetSetWithHierarchyAsync(int setId)
    {
        return await db.TemplateSets
            .Include(s => s.TemplateLists.OrderBy(l => l.SortOrder))
                .ThenInclude(l => l.TemplateCategories.OrderBy(c => c.SortOrder))
                    .ThenInclude(c => c.TemplateActions.OrderBy(a => a.SortOrder))
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SetId == setId);
    }
}
