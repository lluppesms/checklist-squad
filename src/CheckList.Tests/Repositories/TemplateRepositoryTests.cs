using CheckList.Tests.Helpers;

namespace CheckList.Tests.Repositories;

[TestClass]
public sealed class TemplateRepositoryTests
{
    private CheckListDbContext _db = null!;
    private TemplateRepository _repo = null!;

    [TestInitialize]
    public void Setup()
    {
        _db = DbContextHelper.CreateInMemoryContext();
        _repo = new TemplateRepository(_db);
    }

    [TestCleanup]
    public void Cleanup() => _db.Dispose();

    // GetAllSetsAsync
    [TestMethod]
    public async Task GetAllSetsAsync_ReturnsEmpty_WhenNone()
    {
        var result = await _repo.GetAllSetsAsync();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GetAllSetsAsync_ReturnsSets_OrderedBySortOrder()
    {
        var s1 = DbContextHelper.SeedTemplateSet(_db, "Beta");
        s1.SortOrder = 20;
        _db.SaveChanges();

        var s2 = DbContextHelper.SeedTemplateSet(_db, "Alpha");
        s2.SortOrder = 10;
        _db.SaveChanges();

        var result = await _repo.GetAllSetsAsync();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Alpha", result[0].SetName);
    }

    // GetSetWithHierarchyAsync
    [TestMethod]
    public async Task GetSetWithHierarchyAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _repo.GetSetWithHierarchyAsync(999);
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetSetWithHierarchyAsync_IncludesFullHierarchy()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        var result = await _repo.GetSetWithHierarchyAsync(seeded.SetId);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.TemplateLists.Count);
        var list = result.TemplateLists.First();
        Assert.AreEqual(1, list.TemplateCategories.Count);
        Assert.IsTrue(list.TemplateCategories.First().TemplateActions.Count > 0);
    }

    // CreateSetAsync
    [TestMethod]
    public async Task CreateSetAsync_CreatesAndReturnsDto()
    {
        var req = new CreateTemplateSetRequest("New Set", "Description", "Owner");
        var result = await _repo.CreateSetAsync(req, "TestUser");

        Assert.AreEqual("New Set", result.SetName);
        Assert.AreEqual("Description", result.SetDscr);
        Assert.AreEqual("Owner", result.OwnerName);
        Assert.IsTrue(result.SetId > 0);
    }

    [TestMethod]
    public async Task CreateSetAsync_PersistsToDb()
    {
        var req = new CreateTemplateSetRequest("Persisted", "D", "O");
        var result = await _repo.CreateSetAsync(req, "User");

        var fromDb = await _db.TemplateSets.FindAsync(result.SetId);
        Assert.IsNotNull(fromDb);
        Assert.AreEqual("Persisted", fromDb.SetName);
    }

    // UpdateSetAsync
    [TestMethod]
    public async Task UpdateSetAsync_ReturnsNull_WhenNotFound()
    {
        var req = new UpdateTemplateSetRequest("X", "D", "O", 1, "Y");
        var result = await _repo.UpdateSetAsync(999, req, "User");
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task UpdateSetAsync_UpdatesFields()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        var req = new UpdateTemplateSetRequest("Updated", "New Desc", "NewOwner", 99, "N");

        var result = await _repo.UpdateSetAsync(seeded.SetId, req, "Editor");

        Assert.IsNotNull(result);
        Assert.AreEqual("Updated", result.SetName);
        Assert.AreEqual("New Desc", result.SetDscr);
        Assert.AreEqual("NewOwner", result.OwnerName);
        Assert.AreEqual(99, result.SortOrder);
        Assert.AreEqual("N", result.ActiveInd);
    }

    // DeleteSetAsync
    [TestMethod]
    public async Task DeleteSetAsync_ReturnsTrue_WhenFound()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        Assert.IsTrue(await _repo.DeleteSetAsync(seeded.SetId));
    }

    [TestMethod]
    public async Task DeleteSetAsync_ReturnsFalse_WhenNotFound()
    {
        Assert.IsFalse(await _repo.DeleteSetAsync(999));
    }

    // CreateListAsync
    [TestMethod]
    public async Task CreateListAsync_CreatesAndReturnsDto()
    {
        var set = DbContextHelper.SeedTemplateSet(_db);
        var req = new CreateTemplateListRequest("New List", "Desc");

        var result = await _repo.CreateListAsync(set.SetId, req, "User");

        Assert.AreEqual("New List", result.ListName);
        Assert.IsTrue(result.ListId > 0);
    }

    // UpdateListAsync
    [TestMethod]
    public async Task UpdateListAsync_ReturnsNull_WhenNotFound()
    {
        var req = new UpdateTemplateListRequest("X", "D", 1, "Y");
        Assert.IsNull(await _repo.UpdateListAsync(999, req, "User"));
    }

    [TestMethod]
    public async Task UpdateListAsync_UpdatesFields()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        var listId = seeded.TemplateLists.First().ListId;
        var req = new UpdateTemplateListRequest("Renamed", "New Desc", 99, "N");

        var result = await _repo.UpdateListAsync(listId, req, "Editor");

        Assert.IsNotNull(result);
        Assert.AreEqual("Renamed", result.ListName);
    }

    // DeleteListAsync
    [TestMethod]
    public async Task DeleteListAsync_ReturnsTrue_WhenFound()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        var listId = seeded.TemplateLists.First().ListId;
        Assert.IsTrue(await _repo.DeleteListAsync(listId));
    }

    [TestMethod]
    public async Task DeleteListAsync_ReturnsFalse_WhenNotFound()
    {
        Assert.IsFalse(await _repo.DeleteListAsync(999));
    }

    // CreateCategoryAsync
    [TestMethod]
    public async Task CreateCategoryAsync_CreatesAndReturnsDto()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        var listId = seeded.TemplateLists.First().ListId;
        var req = new CreateTemplateCategoryRequest("New Cat", "Desc");

        var result = await _repo.CreateCategoryAsync(listId, req, "User");

        Assert.AreEqual("New Cat", result.CategoryText);
        Assert.IsTrue(result.CategoryId > 0);
    }

    // UpdateCategoryAsync
    [TestMethod]
    public async Task UpdateCategoryAsync_ReturnsNull_WhenNotFound()
    {
        var req = new UpdateTemplateCategoryRequest("X", "D", 1, "Y");
        Assert.IsNull(await _repo.UpdateCategoryAsync(999, req, "User"));
    }

    [TestMethod]
    public async Task UpdateCategoryAsync_UpdatesFields()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        var catId = seeded.TemplateLists.First().TemplateCategories.First().CategoryId;
        var req = new UpdateTemplateCategoryRequest("Renamed Cat", "New D", 99, "N");

        var result = await _repo.UpdateCategoryAsync(catId, req, "Editor");

        Assert.IsNotNull(result);
        Assert.AreEqual("Renamed Cat", result.CategoryText);
    }

    // DeleteCategoryAsync
    [TestMethod]
    public async Task DeleteCategoryAsync_ReturnsTrue_WhenFound()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        var catId = seeded.TemplateLists.First().TemplateCategories.First().CategoryId;
        Assert.IsTrue(await _repo.DeleteCategoryAsync(catId));
    }

    [TestMethod]
    public async Task DeleteCategoryAsync_ReturnsFalse_WhenNotFound()
    {
        Assert.IsFalse(await _repo.DeleteCategoryAsync(999));
    }

    // CreateActionAsync
    [TestMethod]
    public async Task CreateActionAsync_CreatesAndReturnsDto()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        var catId = seeded.TemplateLists.First().TemplateCategories.First().CategoryId;
        var req = new CreateTemplateActionRequest("New Action", "Desc");

        var result = await _repo.CreateActionAsync(catId, req, "User");

        Assert.AreEqual("New Action", result.ActionText);
        Assert.IsTrue(result.ActionId > 0);
    }

    // UpdateActionAsync
    [TestMethod]
    public async Task UpdateActionAsync_ReturnsNull_WhenNotFound()
    {
        var req = new UpdateTemplateActionRequest("X", "D", 1);
        Assert.IsNull(await _repo.UpdateActionAsync(999, req, "User"));
    }

    [TestMethod]
    public async Task UpdateActionAsync_UpdatesFields()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        var actionId = seeded.TemplateLists.First().TemplateCategories.First().TemplateActions.First().ActionId;
        var req = new UpdateTemplateActionRequest("Renamed", "New D", 99);

        var result = await _repo.UpdateActionAsync(actionId, req, "Editor");

        Assert.IsNotNull(result);
        Assert.AreEqual("Renamed", result.ActionText);
    }

    // DeleteActionAsync
    [TestMethod]
    public async Task DeleteActionAsync_ReturnsTrue_WhenFound()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        var actionId = seeded.TemplateLists.First().TemplateCategories.First().TemplateActions.First().ActionId;
        Assert.IsTrue(await _repo.DeleteActionAsync(actionId));
    }

    [TestMethod]
    public async Task DeleteActionAsync_ReturnsFalse_WhenNotFound()
    {
        Assert.IsFalse(await _repo.DeleteActionAsync(999));
    }

    // ExportSetAsync
    [TestMethod]
    public async Task ExportSetAsync_ReturnsExportDto()
    {
        var seeded = DbContextHelper.SeedTemplateSet(_db);
        var result = await _repo.ExportSetAsync(seeded.SetId);

        Assert.AreEqual("Camp Setup", result.SetName);
        Assert.IsTrue(result.Lists.Count > 0);
    }

    [TestMethod]
    public async Task ExportSetAsync_ThrowsWhenNotFound()
    {
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => _repo.ExportSetAsync(999));
    }

    // ExportAllAsync
    [TestMethod]
    public async Task ExportAllAsync_ReturnsOnlyActive()
    {
        var s1 = DbContextHelper.SeedTemplateSet(_db, "Active");
        var s2 = DbContextHelper.SeedTemplateSet(_db, "Inactive");
        s2.ActiveInd = "N";
        _db.SaveChanges();

        var result = await _repo.ExportAllAsync();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Active", result[0].SetName);
    }

    // ImportSetAsync
    [TestMethod]
    public async Task ImportSetAsync_CreatesFullHierarchy()
    {
        var import = new TemplateImportDto(
            "Imported Set", "Desc", "Importer", 10,
            [
                new TemplateListImportDto("List1", "LD", 1,
                [
                    new TemplateCategoryImportDto("Cat1", "CD", 1,
                    [
                        new TemplateActionImportDto("Action1", "AD", 1)
                    ])
                ])
            ]);

        var result = await _repo.ImportSetAsync(import, "System");

        Assert.AreEqual("Imported Set", result.SetName);
        Assert.AreEqual(1, result.Lists.Count);
        Assert.AreEqual(1, result.Lists[0].Categories.Count);
        Assert.AreEqual(1, result.Lists[0].Categories[0].Actions.Count);
    }

    // FullExportAsync
    [TestMethod]
    public async Task FullExportAsync_IncludesTemplatesAndChecklists()
    {
        DbContextHelper.SeedTemplateSet(_db);
        DbContextHelper.SeedCheckSet(_db);

        var result = await _repo.FullExportAsync();

        Assert.IsTrue(result.Templates.Count > 0);
        Assert.IsTrue(result.Checklists.Count > 0);
        Assert.IsNotNull(result.Metadata);
    }

    // FullImportAsync
    [TestMethod]
    public async Task FullImportAsync_ImportsTemplatesAndChecklists()
    {
        var import = new FullImportDto(
            [
                new TemplateImportDto("T1", "D", "O", 1,
                [
                    new TemplateListImportDto("L1", "D", 1,
                    [
                        new TemplateCategoryImportDto("C1", "D", 1,
                        [
                            new TemplateActionImportDto("A1", "D", 1)
                        ])
                    ])
                ])
            ],
            [
                new CheckSetImportDto("CS1", "D", "O", 1,
                [
                    new CheckListImportDto("CL1", "D", 1,
                    [
                        new CheckCategoryImportDto("CC1", "D", 1,
                        [
                            new CheckActionImportDto("CA1", "D", "N", 1)
                        ])
                    ])
                ])
            ]);

        var (templateCount, checklistCount) = await _repo.FullImportAsync(import, "System");

        Assert.AreEqual(1, templateCount);
        Assert.AreEqual(1, checklistCount);
    }

    [TestMethod]
    public async Task FullImportAsync_EmptyImport_ReturnsZeroCounts()
    {
        var import = new FullImportDto([], []);
        var (tc, cc) = await _repo.FullImportAsync(import, "System");
        Assert.AreEqual(0, tc);
        Assert.AreEqual(0, cc);
    }
}
