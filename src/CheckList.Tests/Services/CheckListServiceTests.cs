namespace CheckList.Tests.Services;

[TestClass]
public sealed class CheckListServiceTests
{
    private Mock<ITemplateRepository> _templateRepo = null!;
    private Mock<ICheckRepository> _checkRepo = null!;
    private Mock<IHubContext<CheckListHub, ICheckListHubClient>> _hubContext = null!;
    private Mock<ICheckListHubClient> _hubClients = null!;
    private CheckListService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _templateRepo = new Mock<ITemplateRepository>();
        _checkRepo = new Mock<ICheckRepository>();
        _hubContext = new Mock<IHubContext<CheckListHub, ICheckListHubClient>>();
        _hubClients = new Mock<ICheckListHubClient>();

        var mockClientsAll = new Mock<IHubClients<ICheckListHubClient>>();
        mockClientsAll.Setup(c => c.All).Returns(_hubClients.Object);
        _hubContext.Setup(h => h.Clients).Returns(mockClientsAll.Object);

        var mockSharingService = new Mock<ISharingService>();

        _service = new CheckListService(_templateRepo.Object, _checkRepo.Object, _hubContext.Object, mockSharingService.Object);
    }

    // GetTemplatesAsync
    [TestMethod]
    public async Task GetTemplatesAsync_ReturnsEmpty_WhenNoTemplates()
    {
        _templateRepo.Setup(r => r.GetAllSetsAsync()).ReturnsAsync([]);
        var result = await _service.GetTemplatesAsync();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GetTemplatesAsync_ReturnsMappedSummaries()
    {
        var sets = new List<TemplateSet>
        {
            new() { SetId = 1, SetName = "Set1", SetDscr = "D1", OwnerName = "O", ActiveInd = "Y", SortOrder = 1 },
            new() { SetId = 2, SetName = "Set2", SetDscr = "D2", OwnerName = "O", ActiveInd = "Y", SortOrder = 2 }
        };
        _templateRepo.Setup(r => r.GetAllSetsAsync()).ReturnsAsync(sets);

        var result = await _service.GetTemplatesAsync();

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Set1", result[0].SetName);
        Assert.AreEqual("Set2", result[1].SetName);
    }

    // GetTemplateAsync
    [TestMethod]
    public async Task GetTemplateAsync_ReturnsNull_WhenNotFound()
    {
        _templateRepo.Setup(r => r.GetSetWithHierarchyAsync(99)).ReturnsAsync((TemplateSet?)null);
        var result = await _service.GetTemplateAsync(99);
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetTemplateAsync_ReturnsMappedDto()
    {
        var set = new TemplateSet { SetId = 1, SetName = "S", SetDscr = "D", OwnerName = "O", ActiveInd = "Y", SortOrder = 1 };
        _templateRepo.Setup(r => r.GetSetWithHierarchyAsync(1)).ReturnsAsync(set);

        var result = await _service.GetTemplateAsync(1);

        Assert.IsNotNull(result);
        Assert.AreEqual("S", result.SetName);
    }

    // ActivateCheckSetAsync
    [TestMethod]
    public async Task ActivateCheckSetAsync_ReturnsDto_AndBroadcasts()
    {
        var checkSet = new CheckSet
        {
            SetId = 10, SetName = "Active", SetDscr = "D", OwnerName = "O", ActiveInd = "Y", SortOrder = 1
        };
        _checkRepo.Setup(r => r.ActivateFromTemplateAsync(1, "Owner", null, null)).ReturnsAsync(checkSet);

        var result = await _service.ActivateCheckSetAsync(1, "Owner");

        Assert.IsNotNull(result);
        Assert.AreEqual("Active", result.SetName);
        _hubClients.Verify(c => c.CheckSetActivated(10, "Active"), Times.Once);
    }

    [TestMethod]
    public async Task ActivateCheckSetAsync_PassesSelectedListIds()
    {
        var ids = new List<int> { 1, 2 };
        var checkSet = new CheckSet { SetId = 5, SetName = "X", OwnerName = "O", ActiveInd = "Y" };
        _checkRepo.Setup(r => r.ActivateFromTemplateAsync(1, "O", ids, null)).ReturnsAsync(checkSet);

        await _service.ActivateCheckSetAsync(1, "O", ids);

        _checkRepo.Verify(r => r.ActivateFromTemplateAsync(1, "O", ids, null), Times.Once);
    }

    // GetActiveCheckSetsAsync
    [TestMethod]
    public async Task GetActiveCheckSetsAsync_ReturnsEmpty_WhenNone()
    {
        _checkRepo.Setup(r => r.GetAllActiveSetsAsync()).ReturnsAsync([]);
        var result = await _service.GetActiveCheckSetsAsync();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task GetActiveCheckSetsAsync_ReturnsSummaries()
    {
        var now = DateTime.UtcNow;
        var sets = new List<CheckSet>
        {
            new() { SetId = 1, SetName = "A", OwnerName = "O", ActiveInd = "Y", CreateDateTime = now }
        };
        _checkRepo.Setup(r => r.GetAllActiveSetsAsync()).ReturnsAsync(sets);

        var result = await _service.GetActiveCheckSetsAsync();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("A", result[0].SetName);
    }

    // GetCheckSetAsync
    [TestMethod]
    public async Task GetCheckSetAsync_ReturnsNull_WhenNotFound()
    {
        _checkRepo.Setup(r => r.GetSetWithHierarchyAsync(99)).ReturnsAsync((CheckSet?)null);
        var result = await _service.GetCheckSetAsync(99);
        Assert.IsNull(result);
    }

    // ToggleActionAsync
    [TestMethod]
    public async Task ToggleActionAsync_ReturnsDto_AndBroadcasts()
    {
        var now = DateTime.UtcNow;
        var action = new CheckAction
        {
            ActionId = 5, ActionText = "Do it", CompleteInd = "Y",
            ChangeDateTime = now, ChangeUserName = "Tester"
        };
        _checkRepo.Setup(r => r.ToggleActionAsync(5, "Tester")).ReturnsAsync(action);

        var result = await _service.ToggleActionAsync(5, "Tester");

        Assert.IsNotNull(result);
        Assert.AreEqual("Y", result.CompleteInd);
        _hubClients.Verify(c => c.ActionToggled(5, "Y", "Tester", now), Times.Once);
    }

    // DeleteCheckSetAsync
    [TestMethod]
    public async Task DeleteCheckSetAsync_CallsRepo_AndBroadcasts()
    {
        _checkRepo.Setup(r => r.DeleteSetAsync(3)).ReturnsAsync(true);

        await _service.DeleteCheckSetAsync(3);

        _checkRepo.Verify(r => r.DeleteSetAsync(3), Times.Once);
        _hubClients.Verify(c => c.CheckSetDeleted(3), Times.Once);
    }

    // Template CRUD - CreateTemplateSetAsync
    [TestMethod]
    public async Task CreateTemplateSetAsync_DelegatesToRepo()
    {
        var req = new CreateTemplateSetRequest("New", "Desc", "Owner");
        var expected = new TemplateSetDto(1, "New", "Desc", "Owner", "Y", 50, []);
        _templateRepo.Setup(r => r.CreateSetAsync(req, "System")).ReturnsAsync(expected);

        var result = await _service.CreateTemplateSetAsync(req);

        Assert.AreEqual("New", result.SetName);
    }

    // UpdateTemplateSetAsync
    [TestMethod]
    public async Task UpdateTemplateSetAsync_ReturnsUpdated()
    {
        var req = new UpdateTemplateSetRequest("Upd", "D", "O", 10, "Y");
        var expected = new TemplateSetDto(1, "Upd", "D", "O", "Y", 10, []);
        _templateRepo.Setup(r => r.UpdateSetAsync(1, req, "System")).ReturnsAsync(expected);

        var result = await _service.UpdateTemplateSetAsync(1, req);
        Assert.AreEqual("Upd", result.SetName);
    }

    [TestMethod]
    public async Task UpdateTemplateSetAsync_ThrowsWhenNotFound()
    {
        var req = new UpdateTemplateSetRequest("X", "D", "O", 1, "Y");
        _templateRepo.Setup(r => r.UpdateSetAsync(99, req, "System")).ReturnsAsync((TemplateSetDto?)null);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => _service.UpdateTemplateSetAsync(99, req));
    }

    // DeleteTemplateSetAsync
    [TestMethod]
    public async Task DeleteTemplateSetAsync_DelegatesToRepo()
    {
        await _service.DeleteTemplateSetAsync(5);
        _templateRepo.Verify(r => r.DeleteSetAsync(5), Times.Once);
    }

    // Template List CRUD
    [TestMethod]
    public async Task CreateTemplateListAsync_DelegatesToRepo()
    {
        var req = new CreateTemplateListRequest("List", "Desc");
        var expected = new TemplateListDto(1, "List", "Desc", 50, []);
        _templateRepo.Setup(r => r.CreateListAsync(1, req, "System")).ReturnsAsync(expected);

        var result = await _service.CreateTemplateListAsync(1, req);
        Assert.AreEqual("List", result.ListName);
    }

    [TestMethod]
    public async Task UpdateTemplateListAsync_ThrowsWhenNotFound()
    {
        var req = new UpdateTemplateListRequest("X", "D", 1, "Y");
        _templateRepo.Setup(r => r.UpdateListAsync(99, req, "System")).ReturnsAsync((TemplateListDto?)null);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => _service.UpdateTemplateListAsync(99, req));
    }

    // Template Category CRUD
    [TestMethod]
    public async Task CreateTemplateCategoryAsync_DelegatesToRepo()
    {
        var req = new CreateTemplateCategoryRequest("Cat", "D");
        var expected = new TemplateCategoryDto(1, "Cat", "D", 50, []);
        _templateRepo.Setup(r => r.CreateCategoryAsync(1, req, "System")).ReturnsAsync(expected);

        var result = await _service.CreateTemplateCategoryAsync(1, req);
        Assert.AreEqual("Cat", result.CategoryText);
    }

    [TestMethod]
    public async Task UpdateTemplateCategoryAsync_ThrowsWhenNotFound()
    {
        var req = new UpdateTemplateCategoryRequest("X", "D", 1, "Y");
        _templateRepo.Setup(r => r.UpdateCategoryAsync(99, req, "System")).ReturnsAsync((TemplateCategoryDto?)null);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => _service.UpdateTemplateCategoryAsync(99, req));
    }

    // Template Action CRUD
    [TestMethod]
    public async Task CreateTemplateActionAsync_DelegatesToRepo()
    {
        var req = new CreateTemplateActionRequest("Act", "D");
        var expected = new TemplateActionDto(1, "Act", "D", 50);
        _templateRepo.Setup(r => r.CreateActionAsync(1, req, "System")).ReturnsAsync(expected);

        var result = await _service.CreateTemplateActionAsync(1, req);
        Assert.AreEqual("Act", result.ActionText);
    }

    [TestMethod]
    public async Task UpdateTemplateActionAsync_ThrowsWhenNotFound()
    {
        var req = new UpdateTemplateActionRequest("X", "D", 1);
        _templateRepo.Setup(r => r.UpdateActionAsync(99, req, "System")).ReturnsAsync((TemplateActionDto?)null);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => _service.UpdateTemplateActionAsync(99, req));
    }

    // Import/Export
    [TestMethod]
    public async Task ExportTemplateAsync_DelegatesToRepo()
    {
        var expected = new TemplateExportDto("S", "D", "O", 1, [], new ExportMetadata(DateTime.UtcNow, "1.0.0", 1));
        _templateRepo.Setup(r => r.ExportSetAsync(1)).ReturnsAsync(expected);

        var result = await _service.ExportTemplateAsync(1);
        Assert.AreEqual("S", result.SetName);
    }

    [TestMethod]
    public async Task ExportAllTemplatesAsync_ReturnsAll()
    {
        _templateRepo.Setup(r => r.ExportAllAsync()).ReturnsAsync([]);
        var result = await _service.ExportAllTemplatesAsync();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task ImportTemplateAsync_DelegatesToRepo()
    {
        var import = new TemplateImportDto("S", "D", "O", 1, []);
        var expected = new TemplateSetDto(1, "S", "D", "O", "Y", 1, []);
        _templateRepo.Setup(r => r.ImportSetAsync(import, "System")).ReturnsAsync(expected);

        var result = await _service.ImportTemplateAsync(import);
        Assert.AreEqual("S", result.SetName);
    }

    [TestMethod]
    public async Task ExportFullAsync_DelegatesToRepo()
    {
        var expected = new FullExportDto([], [], new ExportMetadata(DateTime.UtcNow, "1.0.0", 0));
        _templateRepo.Setup(r => r.FullExportAsync()).ReturnsAsync(expected);

        var result = await _service.ExportFullAsync();
        Assert.AreEqual(0, result.Templates.Count);
    }

    [TestMethod]
    public async Task ImportFullAsync_ReturnsCounts()
    {
        var import = new FullImportDto([], []);
        _templateRepo.Setup(r => r.FullImportAsync(import, "System")).ReturnsAsync((2, 3));

        var result = await _service.ImportFullAsync(import);
        Assert.AreEqual(2, result.templateCount);
        Assert.AreEqual(3, result.checklistCount);
    }
}
