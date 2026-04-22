using CheckList.Tests.Helpers;

namespace CheckList.Tests.Repositories;

[TestClass]
public sealed class CheckRepositoryTests
{
    private CheckListDbContext _db = null!;
    private CheckRepository _repo = null!;

    [TestInitialize]
    public void Setup()
    {
        _db = DbContextHelper.CreateInMemoryContext();
        _repo = new CheckRepository(_db);
    }

    [TestCleanup]
    public void Cleanup() => _db.Dispose();

    // GetAllActiveSetsAsync
    [TestMethod]
    public async Task GetAllActiveSetsAsync_ReturnsEmpty_WhenNoSets()
    {
        var result = await _repo.GetAllActiveSetsAsync();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GetAllActiveSetsAsync_ReturnsOnlyActiveSets()
    {
        DbContextHelper.SeedCheckSet(_db, "Active1");
        var inactive = DbContextHelper.SeedCheckSet(_db, "Inactive1");
        inactive.ActiveInd = "N";
        _db.SaveChanges();

        var result = await _repo.GetAllActiveSetsAsync();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Active1", result[0].SetName);
    }

    [TestMethod]
    public async Task GetAllActiveSetsAsync_OrderedByCreateDateDesc()
    {
        var set1 = DbContextHelper.SeedCheckSet(_db, "Older");
        set1.CreateDateTime = DateTime.UtcNow.AddDays(-1);
        _db.SaveChanges();

        var set2 = DbContextHelper.SeedCheckSet(_db, "Newer");
        set2.CreateDateTime = DateTime.UtcNow;
        _db.SaveChanges();

        var result = await _repo.GetAllActiveSetsAsync();

        Assert.AreEqual("Newer", result[0].SetName);
        Assert.AreEqual("Older", result[1].SetName);
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
        var seeded = DbContextHelper.SeedCheckSet(_db);
        var result = await _repo.GetSetWithHierarchyAsync(seeded.SetId);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.CheckLists.Count);
        var list = result.CheckLists.First();
        Assert.AreEqual(1, list.CheckCategories.Count);
        Assert.AreEqual(1, list.CheckCategories.First().CheckActions.Count);
    }

    // GetActionAsync
    [TestMethod]
    public async Task GetActionAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _repo.GetActionAsync(999);
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetActionAsync_ReturnsAction_WhenExists()
    {
        var seeded = DbContextHelper.SeedCheckSet(_db);
        var actionId = seeded.CheckLists.First().CheckCategories.First().CheckActions.First().ActionId;

        var result = await _repo.GetActionAsync(actionId);

        Assert.IsNotNull(result);
        Assert.AreEqual("Extend main awning", result.ActionText);
    }

    // ToggleActionAsync
    [TestMethod]
    public async Task ToggleActionAsync_TogglesFromN_ToY()
    {
        var seeded = DbContextHelper.SeedCheckSet(_db);
        var action = seeded.CheckLists.First().CheckCategories.First().CheckActions.First();
        Assert.AreEqual("N", action.CompleteInd);

        var result = await _repo.ToggleActionAsync(action.ActionId, "Tester");

        Assert.AreEqual("Y", result.CompleteInd);
        Assert.AreEqual("Tester", result.ChangeUserName);
    }

    [TestMethod]
    public async Task ToggleActionAsync_TogglesFromY_ToN()
    {
        var seeded = DbContextHelper.SeedCheckSet(_db);
        var action = seeded.CheckLists.First().CheckCategories.First().CheckActions.First();

        // First toggle N -> Y
        await _repo.ToggleActionAsync(action.ActionId, "User1");
        // Second toggle Y -> N
        var result = await _repo.ToggleActionAsync(action.ActionId, "User2");

        Assert.AreEqual("N", result.CompleteInd);
        Assert.AreEqual("User2", result.ChangeUserName);
    }

    [TestMethod]
    public async Task ToggleActionAsync_ThrowsKeyNotFound_WhenMissing()
    {
        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _repo.ToggleActionAsync(999, "User"));
    }

    [TestMethod]
    public async Task ToggleActionAsync_UpdatesChangeDateTime()
    {
        var seeded = DbContextHelper.SeedCheckSet(_db);
        var action = seeded.CheckLists.First().CheckCategories.First().CheckActions.First();
        var beforeToggle = action.ChangeDateTime;

        await Task.Delay(10); // Small delay to ensure different timestamp
        var result = await _repo.ToggleActionAsync(action.ActionId, "User");

        Assert.IsTrue(result.ChangeDateTime >= beforeToggle);
    }

    // ActivateFromTemplateAsync
    [TestMethod]
    public async Task ActivateFromTemplateAsync_CreatesCheckSet_FromTemplate()
    {
        var template = DbContextHelper.SeedTemplateSet(_db);

        var result = await _repo.ActivateFromTemplateAsync(template.SetId, "Camper");

        Assert.IsNotNull(result);
        Assert.IsTrue(result.SetName.Contains("Camp Setup"));
        Assert.AreEqual("Y", result.ActiveInd);
        Assert.AreEqual("Camper", result.OwnerName);
        Assert.AreEqual(2, result.CheckLists.Count);
    }

    [TestMethod]
    public async Task ActivateFromTemplateAsync_CopiesActions_WithCompleteIndN()
    {
        var template = DbContextHelper.SeedTemplateSet(_db);

        var result = await _repo.ActivateFromTemplateAsync(template.SetId, "Camper");

        var actions = result.CheckLists.SelectMany(l =>
            l.CheckCategories.SelectMany(c => c.CheckActions)).ToList();
        Assert.IsTrue(actions.All(a => a.CompleteInd == "N"));
        Assert.AreEqual(3, actions.Count);
    }

    [TestMethod]
    public async Task ActivateFromTemplateAsync_WithSelectedListIds_OnlyCopiesThoseLists()
    {
        var template = DbContextHelper.SeedTemplateSet(_db);
        var firstListId = template.TemplateLists.First().ListId;

        var result = await _repo.ActivateFromTemplateAsync(template.SetId, "Camper", [firstListId]);

        Assert.AreEqual(1, result.CheckLists.Count);
        Assert.AreEqual("Exterior", result.CheckLists.First().ListName);
    }

    [TestMethod]
    public async Task ActivateFromTemplateAsync_ThrowsKeyNotFound_WhenTemplateMissing()
    {
        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _repo.ActivateFromTemplateAsync(999, "User"));
    }

    [TestMethod]
    public async Task ActivateFromTemplateAsync_ThrowsArgumentException_WhenNoMatchingLists()
    {
        var template = DbContextHelper.SeedTemplateSet(_db);

        await Assert.ThrowsExactlyAsync<ArgumentException>(
            () => _repo.ActivateFromTemplateAsync(template.SetId, "User", [9999]));
    }

    [TestMethod]
    public async Task ActivateFromTemplateAsync_UsesCustomName_WhenProvided()
    {
        var template = DbContextHelper.SeedTemplateSet(_db);

        var result = await _repo.ActivateFromTemplateAsync(template.SetId, "Camper", null, "My Custom Checklist");

        Assert.AreEqual("My Custom Checklist", result.SetName);
    }

    [TestMethod]
    public async Task ActivateFromTemplateAsync_UsesAutoName_WhenCustomNameIsNull()
    {
        var template = DbContextHelper.SeedTemplateSet(_db);

        var result = await _repo.ActivateFromTemplateAsync(template.SetId, "Camper", null, null);

        Assert.IsTrue(result.SetName.Contains("Camp Setup"));
    }

    [TestMethod]
    public async Task ActivateFromTemplateAsync_UsesAutoName_WhenCustomNameIsWhitespace()
    {
        var template = DbContextHelper.SeedTemplateSet(_db);

        var result = await _repo.ActivateFromTemplateAsync(template.SetId, "Camper", null, "   ");

        Assert.IsTrue(result.SetName.Contains("Camp Setup"));
    }

    [TestMethod]
    public async Task ActivateFromTemplateAsync_PersistsToDatabase()
    {
        var template = DbContextHelper.SeedTemplateSet(_db);
        var result = await _repo.ActivateFromTemplateAsync(template.SetId, "Camper");

        var fromDb = await _db.CheckSets.FindAsync(result.SetId);
        Assert.IsNotNull(fromDb);
    }

    // DeleteSetAsync
    [TestMethod]
    public async Task DeleteSetAsync_ReturnsTrue_WhenFound()
    {
        var seeded = DbContextHelper.SeedCheckSet(_db);
        var result = await _repo.DeleteSetAsync(seeded.SetId);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task DeleteSetAsync_ReturnsFalse_WhenNotFound()
    {
        var result = await _repo.DeleteSetAsync(999);
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task DeleteSetAsync_RemovesFromDatabase()
    {
        var seeded = DbContextHelper.SeedCheckSet(_db);
        await _repo.DeleteSetAsync(seeded.SetId);

        var fromDb = await _db.CheckSets.FindAsync(seeded.SetId);
        Assert.IsNull(fromDb);
    }
}
