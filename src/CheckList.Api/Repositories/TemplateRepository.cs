namespace CheckList.Api.Repositories;

using CheckList.Api.Models.DTOs;
using CheckList.Api.Models.Mapping;

public class TemplateRepository(CheckListDbContext db) : ITemplateRepository
{
    // Read operations
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

    // Template Set CRUD
    public async Task<TemplateSetDto> CreateSetAsync(CreateTemplateSetRequest request, string userName)
    {
        var now = DateTime.UtcNow;
        var entity = new TemplateSet
        {
            SetName = request.SetName,
            SetDscr = request.SetDscr,
            OwnerName = request.OwnerName,
            SortOrder = request.SortOrder,
            ActiveInd = "Y",
            CreateDateTime = now,
            CreateUserName = userName,
            ChangeDateTime = now,
            ChangeUserName = userName
        };

        db.TemplateSets.Add(entity);
        await db.SaveChangesAsync();

        return entity.ToDto();
    }

    public async Task<TemplateSetDto?> UpdateSetAsync(int setId, UpdateTemplateSetRequest request, string userName)
    {
        var entity = await db.TemplateSets
            .Include(s => s.TemplateLists.OrderBy(l => l.SortOrder))
                .ThenInclude(l => l.TemplateCategories.OrderBy(c => c.SortOrder))
                    .ThenInclude(c => c.TemplateActions.OrderBy(a => a.SortOrder))
            .FirstOrDefaultAsync(s => s.SetId == setId);

        if (entity is null) return null;

        entity.SetName = request.SetName;
        entity.SetDscr = request.SetDscr;
        entity.OwnerName = request.OwnerName;
        entity.SortOrder = request.SortOrder;
        entity.ActiveInd = request.ActiveInd;
        entity.ChangeDateTime = DateTime.UtcNow;
        entity.ChangeUserName = userName;

        await db.SaveChangesAsync();
        return entity.ToDto();
    }

    public async Task<bool> DeleteSetAsync(int setId)
    {
        var entity = await db.TemplateSets.FindAsync(setId);
        if (entity is null) return false;

        db.TemplateSets.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    // Template List CRUD
    public async Task<TemplateListDto> CreateListAsync(int setId, CreateTemplateListRequest request, string userName)
    {
        var now = DateTime.UtcNow;
        var entity = new TemplateList
        {
            SetId = setId,
            ListName = request.ListName,
            ListDscr = request.ListDscr,
            SortOrder = request.SortOrder,
            ActiveInd = "Y",
            CreateDateTime = now,
            CreateUserName = userName,
            ChangeDateTime = now,
            ChangeUserName = userName
        };

        db.TemplateLists.Add(entity);
        await db.SaveChangesAsync();

        return entity.ToDto();
    }

    public async Task<TemplateListDto?> UpdateListAsync(int listId, UpdateTemplateListRequest request, string userName)
    {
        var entity = await db.TemplateLists
            .Include(l => l.TemplateCategories.OrderBy(c => c.SortOrder))
                .ThenInclude(c => c.TemplateActions.OrderBy(a => a.SortOrder))
            .FirstOrDefaultAsync(l => l.ListId == listId);

        if (entity is null) return null;

        entity.ListName = request.ListName;
        entity.ListDscr = request.ListDscr;
        entity.SortOrder = request.SortOrder;
        entity.ActiveInd = request.ActiveInd;
        entity.ChangeDateTime = DateTime.UtcNow;
        entity.ChangeUserName = userName;

        await db.SaveChangesAsync();
        return entity.ToDto();
    }

    public async Task<bool> DeleteListAsync(int listId)
    {
        var entity = await db.TemplateLists.FindAsync(listId);
        if (entity is null) return false;

        db.TemplateLists.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    // Template Category CRUD
    public async Task<TemplateCategoryDto> CreateCategoryAsync(int listId, CreateTemplateCategoryRequest request, string userName)
    {
        var now = DateTime.UtcNow;
        var entity = new TemplateCategory
        {
            ListId = listId,
            CategoryText = request.CategoryText,
            CategoryDscr = request.CategoryDscr,
            SortOrder = request.SortOrder,
            ActiveInd = "Y",
            CreateDateTime = now,
            CreateUserName = userName,
            ChangeDateTime = now,
            ChangeUserName = userName
        };

        db.TemplateCategories.Add(entity);
        await db.SaveChangesAsync();

        return entity.ToDto();
    }

    public async Task<TemplateCategoryDto?> UpdateCategoryAsync(int categoryId, UpdateTemplateCategoryRequest request, string userName)
    {
        var entity = await db.TemplateCategories
            .Include(c => c.TemplateActions.OrderBy(a => a.SortOrder))
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

        if (entity is null) return null;

        entity.CategoryText = request.CategoryText;
        entity.CategoryDscr = request.CategoryDscr;
        entity.SortOrder = request.SortOrder;
        entity.ActiveInd = request.ActiveInd;
        entity.ChangeDateTime = DateTime.UtcNow;
        entity.ChangeUserName = userName;

        await db.SaveChangesAsync();
        return entity.ToDto();
    }

    public async Task<bool> DeleteCategoryAsync(int categoryId)
    {
        var entity = await db.TemplateCategories.FindAsync(categoryId);
        if (entity is null) return false;

        db.TemplateCategories.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    // Template Action CRUD
    public async Task<TemplateActionDto> CreateActionAsync(int categoryId, CreateTemplateActionRequest request, string userName)
    {
        var now = DateTime.UtcNow;
        var entity = new TemplateAction
        {
            CategoryId = categoryId,
            ActionText = request.ActionText,
            ActionDscr = request.ActionDscr,
            SortOrder = request.SortOrder,
            CreateDateTime = now,
            CreateUserName = userName,
            ChangeDateTime = now,
            ChangeUserName = userName
        };

        db.TemplateActions.Add(entity);
        await db.SaveChangesAsync();

        return entity.ToDto();
    }

    public async Task<TemplateActionDto?> UpdateActionAsync(int actionId, UpdateTemplateActionRequest request, string userName)
    {
        var entity = await db.TemplateActions.FindAsync(actionId);
        if (entity is null) return null;

        entity.ActionText = request.ActionText;
        entity.ActionDscr = request.ActionDscr;
        entity.SortOrder = request.SortOrder;
        entity.ChangeDateTime = DateTime.UtcNow;
        entity.ChangeUserName = userName;

        await db.SaveChangesAsync();
        return entity.ToDto();
    }

    public async Task<bool> DeleteActionAsync(int actionId)
    {
        var entity = await db.TemplateActions.FindAsync(actionId);
        if (entity is null) return false;

        db.TemplateActions.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }

    // Import/Export
    public async Task<TemplateExportDto> ExportSetAsync(int setId)
    {
        var set = await GetSetWithHierarchyAsync(setId);
        if (set is null) throw new InvalidOperationException($"Template set {setId} not found");

        return set.ToExportDto();
    }

    public async Task<List<TemplateExportDto>> ExportAllAsync()
    {
        var sets = await db.TemplateSets
            .Include(s => s.TemplateLists.OrderBy(l => l.SortOrder))
                .ThenInclude(l => l.TemplateCategories.OrderBy(c => c.SortOrder))
                    .ThenInclude(c => c.TemplateActions.OrderBy(a => a.SortOrder))
            .Where(s => s.ActiveInd == "Y")
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.SetName)
            .AsNoTracking()
            .ToListAsync();

        return sets.Select(s => s.ToExportDto()).ToList();
    }

    public async Task<TemplateSetDto> ImportSetAsync(TemplateImportDto import, string userName)
    {
        var now = DateTime.UtcNow;
        var set = new TemplateSet
        {
            SetName = import.SetName,
            SetDscr = import.SetDscr,
            OwnerName = import.OwnerName,
            SortOrder = import.SortOrder,
            ActiveInd = "Y",
            CreateDateTime = now,
            CreateUserName = userName,
            ChangeDateTime = now,
            ChangeUserName = userName,
            TemplateLists = import.Lists.Select(l => new TemplateList
            {
                ListName = l.ListName,
                ListDscr = l.ListDscr,
                SortOrder = l.SortOrder,
                ActiveInd = "Y",
                CreateDateTime = now,
                CreateUserName = userName,
                ChangeDateTime = now,
                ChangeUserName = userName,
                TemplateCategories = l.Categories.Select(c => new TemplateCategory
                {
                    CategoryText = c.CategoryText,
                    CategoryDscr = c.CategoryDscr,
                    SortOrder = c.SortOrder,
                    ActiveInd = "Y",
                    CreateDateTime = now,
                    CreateUserName = userName,
                    ChangeDateTime = now,
                    ChangeUserName = userName,
                    TemplateActions = c.Actions.Select(a => new TemplateAction
                    {
                        ActionText = a.ActionText,
                        ActionDscr = a.ActionDscr,
                        SortOrder = a.SortOrder,
                        CreateDateTime = now,
                        CreateUserName = userName,
                        ChangeDateTime = now,
                        ChangeUserName = userName
                    }).ToList()
                }).ToList()
            }).ToList()
        };

        db.TemplateSets.Add(set);
        await db.SaveChangesAsync();

        return set.ToDto();
    }

    public async Task<FullExportDto> FullExportAsync()
    {
        var templates = await ExportAllAsync();
        
        var checklists = await db.CheckSets
            .Include(s => s.CheckLists.OrderBy(l => l.SortOrder))
                .ThenInclude(l => l.CheckCategories.OrderBy(c => c.SortOrder))
                    .ThenInclude(c => c.CheckActions.OrderBy(a => a.SortOrder))
            .Where(s => s.ActiveInd == "Y")
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.SetName)
            .AsNoTracking()
            .ToListAsync();

        var checklistExports = checklists.Select(s => s.ToExportDto()).ToList();

        var metadata = new ExportMetadata(
            DateTime.UtcNow,
            "1.0.0",
            templates.Count + checklists.Count);

        return new FullExportDto(templates, checklistExports, metadata);
    }

    public async Task<(int templateCount, int checklistCount)> FullImportAsync(FullImportDto import, string userName)
    {
        var templateCount = 0;
        var checklistCount = 0;

        foreach (var template in import.Templates)
        {
            await ImportSetAsync(template, userName);
            templateCount++;
        }

        foreach (var checklist in import.Checklists)
        {
            var now = DateTime.UtcNow;
            var checkSet = new CheckSet
            {
                SetName = checklist.SetName,
                SetDscr = checklist.SetDscr,
                OwnerName = checklist.OwnerName,
                SortOrder = checklist.SortOrder,
                ActiveInd = "Y",
                CreateDateTime = now,
                CreateUserName = userName,
                ChangeDateTime = now,
                ChangeUserName = userName,
                CheckLists = checklist.Lists.Select(l => new CheckListEntity
                {
                    ListName = l.ListName,
                    ListDscr = l.ListDscr,
                    SortOrder = l.SortOrder,
                    ActiveInd = "Y",
                    CreateDateTime = now,
                    CreateUserName = userName,
                    ChangeDateTime = now,
                    ChangeUserName = userName,
                    CheckCategories = l.Categories.Select(c => new CheckCategory
                    {
                        CategoryText = c.CategoryText,
                        CategoryDscr = c.CategoryDscr,
                        SortOrder = c.SortOrder,
                        ActiveInd = "Y",
                        CreateDateTime = now,
                        CreateUserName = userName,
                        ChangeDateTime = now,
                        ChangeUserName = userName,
                        CheckActions = c.Actions.Select(a => new CheckAction
                        {
                            ActionText = a.ActionText,
                            ActionDscr = a.ActionDscr,
                            CompleteInd = a.CompleteInd,
                            SortOrder = a.SortOrder,
                            CreateDateTime = now,
                            CreateUserName = userName,
                            ChangeDateTime = now,
                            ChangeUserName = userName
                        }).ToList()
                    }).ToList()
                }).ToList()
            };

            db.CheckSets.Add(checkSet);
            checklistCount++;
        }

        await db.SaveChangesAsync();
        return (templateCount, checklistCount);
    }
}
