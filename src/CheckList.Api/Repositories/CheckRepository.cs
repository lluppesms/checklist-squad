namespace CheckList.Api.Repositories;

public class CheckRepository(CheckListDbContext db) : ICheckRepository
{
    public async Task<List<CheckSet>> GetAllActiveSetsAsync()
    {
        return await db.CheckSets
            .Where(s => s.ActiveInd == "Y")
            .OrderByDescending(s => s.CreateDateTime)
            .ThenBy(s => s.SortOrder)
            .ThenBy(s => s.SetName)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<CheckSet?> GetSetWithHierarchyAsync(int setId)
    {
        return await db.CheckSets
            .Include(s => s.CheckLists.OrderBy(l => l.SortOrder))
                .ThenInclude(l => l.CheckCategories.OrderBy(c => c.SortOrder))
                    .ThenInclude(c => c.CheckActions.OrderBy(a => a.SortOrder))
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SetId == setId);
    }

    public async Task<CheckAction?> GetActionAsync(int actionId)
    {
        return await db.CheckActions.FindAsync(actionId);
    }

    public async Task<CheckAction> ToggleActionAsync(int actionId, string userName)
    {
        var action = await db.CheckActions.FindAsync(actionId)
            ?? throw new KeyNotFoundException($"CheckAction {actionId} not found.");

        action.CompleteInd = action.CompleteInd == "Y" ? "N" : "Y";
        action.ChangeDateTime = DateTime.UtcNow;
        action.ChangeUserName = userName;

        await db.SaveChangesAsync();
        return action;
    }

    public async Task<CheckSet> ActivateFromTemplateAsync(int templateSetId, string ownerName)
    {
        var template = await db.TemplateSets
            .Include(s => s.TemplateLists.OrderBy(l => l.SortOrder))
                .ThenInclude(l => l.TemplateCategories.OrderBy(c => c.SortOrder))
                    .ThenInclude(c => c.TemplateActions.OrderBy(a => a.SortOrder))
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SetId == templateSetId)
            ?? throw new KeyNotFoundException($"TemplateSet {templateSetId} not found.");

        var now = DateTime.UtcNow;
        var localDate = now.ToLocalTime();
        var dateSuffix = localDate.ToString("ddd, MMM d");

        var checkSet = new CheckSet
        {
            TemplateSetId = template.SetId,
            SetName = $"{template.SetName} — {dateSuffix}",
            SetDscr = template.SetDscr,
            OwnerName = ownerName,
            ActiveInd = "Y",
            SortOrder = template.SortOrder,
            CreateDateTime = now,
            CreateUserName = ownerName,
            ChangeDateTime = now,
            ChangeUserName = ownerName,
            CheckLists = template.TemplateLists.Select(tl => new CheckListEntity
            {
                ListName = tl.ListName,
                ListDscr = tl.ListDscr,
                ActiveInd = "Y",
                SortOrder = tl.SortOrder,
                CreateDateTime = now,
                CreateUserName = ownerName,
                ChangeDateTime = now,
                ChangeUserName = ownerName,
                CheckCategories = tl.TemplateCategories.Select(tc => new CheckCategory
                {
                    CategoryText = tc.CategoryText,
                    CategoryDscr = tc.CategoryDscr,
                    ActiveInd = "Y",
                    SortOrder = tc.SortOrder,
                    CreateDateTime = now,
                    CreateUserName = ownerName,
                    ChangeDateTime = now,
                    ChangeUserName = ownerName,
                    CheckActions = tc.TemplateActions.Select(ta => new CheckAction
                    {
                        ActionText = ta.ActionText ?? string.Empty,
                        ActionDscr = ta.ActionDscr,
                        CompleteInd = "N",
                        SortOrder = ta.SortOrder,
                        CreateDateTime = now,
                        CreateUserName = ownerName,
                        ChangeDateTime = now,
                        ChangeUserName = ownerName
                    }).ToList()
                }).ToList()
            }).ToList()
        };

        db.CheckSets.Add(checkSet);
        await db.SaveChangesAsync();
        return checkSet;
    }

    public async Task<bool> DeleteSetAsync(int setId)
    {
        var set = await db.CheckSets.FindAsync(setId);
        if (set is null) return false;

        db.CheckSets.Remove(set);
        await db.SaveChangesAsync();
        return true;
    }
}
