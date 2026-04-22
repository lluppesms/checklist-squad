namespace CheckList.Tests.Controllers;

[TestClass]
public sealed class TemplatesControllerTests
{
    private Mock<ITemplateRepository> _templateRepo = null!;
    private TemplatesController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _templateRepo = new Mock<ITemplateRepository>();
        _controller = new TemplatesController(_templateRepo.Object);
    }

    // GetAll
    [TestMethod]
    public async Task GetAll_ReturnsOk_WithSummaries()
    {
        var sets = new List<TemplateSet>
        {
            new() { SetId = 1, SetName = "S1", SetDscr = "D", OwnerName = "O", ActiveInd = "Y", SortOrder = 1 }
        };
        _templateRepo.Setup(r => r.GetAllSetsAsync()).ReturnsAsync(sets);

        var result = await _controller.GetAll();

        var ok = result.Result as OkObjectResult;
        Assert.IsNotNull(ok);
    }

    [TestMethod]
    public async Task GetAll_ReturnsEmpty_WhenNone()
    {
        _templateRepo.Setup(r => r.GetAllSetsAsync()).ReturnsAsync([]);

        var result = await _controller.GetAll();

        var ok = result.Result as OkObjectResult;
        Assert.IsNotNull(ok);
        var list = ok.Value as List<TemplateSetSummaryDto>;
        Assert.AreEqual(0, list?.Count);
    }

    // GetById
    [TestMethod]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var set = new TemplateSet { SetId = 1, SetName = "S", SetDscr = "D", OwnerName = "O", ActiveInd = "Y" };
        _templateRepo.Setup(r => r.GetSetWithHierarchyAsync(1)).ReturnsAsync(set);

        var result = await _controller.GetById(1);

        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        _templateRepo.Setup(r => r.GetSetWithHierarchyAsync(99)).ReturnsAsync((TemplateSet?)null);

        var result = await _controller.GetById(99);

        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
    }

    // CreateSet
    [TestMethod]
    public async Task CreateSet_ReturnsCreated()
    {
        var req = new CreateTemplateSetRequest("New", "D", "O");
        var dto = new TemplateSetDto(1, "New", "D", "O", "Y", 50, []);
        _templateRepo.Setup(r => r.CreateSetAsync(req, "System")).ReturnsAsync(dto);

        var result = await _controller.CreateSet(req);

        Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
    }

    // UpdateSet
    [TestMethod]
    public async Task UpdateSet_ReturnsOk_WhenFound()
    {
        var req = new UpdateTemplateSetRequest("Upd", "D", "O", 1, "Y");
        var dto = new TemplateSetDto(1, "Upd", "D", "O", "Y", 1, []);
        _templateRepo.Setup(r => r.UpdateSetAsync(1, req, "System")).ReturnsAsync(dto);

        var result = await _controller.UpdateSet(1, req);

        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
    }

    [TestMethod]
    public async Task UpdateSet_ReturnsNotFound_WhenMissing()
    {
        var req = new UpdateTemplateSetRequest("X", "D", "O", 1, "Y");
        _templateRepo.Setup(r => r.UpdateSetAsync(99, req, "System")).ReturnsAsync((TemplateSetDto?)null);

        var result = await _controller.UpdateSet(99, req);

        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
    }

    // DeleteSet
    [TestMethod]
    public async Task DeleteSet_ReturnsNoContent_WhenDeleted()
    {
        _templateRepo.Setup(r => r.DeleteSetAsync(1)).ReturnsAsync(true);
        var result = await _controller.DeleteSet(1);
        Assert.IsInstanceOfType<NoContentResult>(result);
    }

    [TestMethod]
    public async Task DeleteSet_ReturnsNotFound_WhenMissing()
    {
        _templateRepo.Setup(r => r.DeleteSetAsync(99)).ReturnsAsync(false);
        var result = await _controller.DeleteSet(99);
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    // CreateList
    [TestMethod]
    public async Task CreateList_ReturnsCreated()
    {
        var req = new CreateTemplateListRequest("L", "D");
        var dto = new TemplateListDto(1, "L", "D", 50, []);
        _templateRepo.Setup(r => r.CreateListAsync(1, req, "System")).ReturnsAsync(dto);

        var result = await _controller.CreateList(1, req);

        Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
    }

    // UpdateList
    [TestMethod]
    public async Task UpdateList_ReturnsNotFound_WhenMissing()
    {
        var req = new UpdateTemplateListRequest("X", "D", 1, "Y");
        _templateRepo.Setup(r => r.UpdateListAsync(99, req, "System")).ReturnsAsync((TemplateListDto?)null);

        var result = await _controller.UpdateList(99, req);

        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
    }

    [TestMethod]
    public async Task UpdateList_ReturnsOk_WhenFound()
    {
        var req = new UpdateTemplateListRequest("Upd", "D", 1, "Y");
        var dto = new TemplateListDto(1, "Upd", "D", 1, []);
        _templateRepo.Setup(r => r.UpdateListAsync(1, req, "System")).ReturnsAsync(dto);

        var result = await _controller.UpdateList(1, req);

        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
    }

    // DeleteList
    [TestMethod]
    public async Task DeleteList_ReturnsNoContent_WhenDeleted()
    {
        _templateRepo.Setup(r => r.DeleteListAsync(1)).ReturnsAsync(true);
        var result = await _controller.DeleteList(1);
        Assert.IsInstanceOfType<NoContentResult>(result);
    }

    [TestMethod]
    public async Task DeleteList_ReturnsNotFound_WhenMissing()
    {
        _templateRepo.Setup(r => r.DeleteListAsync(99)).ReturnsAsync(false);
        var result = await _controller.DeleteList(99);
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    // CreateCategory
    [TestMethod]
    public async Task CreateCategory_ReturnsOk()
    {
        var req = new CreateTemplateCategoryRequest("C", "D");
        var dto = new TemplateCategoryDto(1, "C", "D", 50, []);
        _templateRepo.Setup(r => r.CreateCategoryAsync(1, req, "System")).ReturnsAsync(dto);

        var result = await _controller.CreateCategory(1, req);

        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
    }

    // UpdateCategory
    [TestMethod]
    public async Task UpdateCategory_ReturnsNotFound_WhenMissing()
    {
        var req = new UpdateTemplateCategoryRequest("X", "D", 1, "Y");
        _templateRepo.Setup(r => r.UpdateCategoryAsync(99, req, "System")).ReturnsAsync((TemplateCategoryDto?)null);

        var result = await _controller.UpdateCategory(99, req);

        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
    }

    // DeleteCategory
    [TestMethod]
    public async Task DeleteCategory_ReturnsNoContent_WhenDeleted()
    {
        _templateRepo.Setup(r => r.DeleteCategoryAsync(1)).ReturnsAsync(true);
        var result = await _controller.DeleteCategory(1);
        Assert.IsInstanceOfType<NoContentResult>(result);
    }

    [TestMethod]
    public async Task DeleteCategory_ReturnsNotFound_WhenMissing()
    {
        _templateRepo.Setup(r => r.DeleteCategoryAsync(99)).ReturnsAsync(false);
        var result = await _controller.DeleteCategory(99);
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    // CreateAction
    [TestMethod]
    public async Task CreateAction_ReturnsOk()
    {
        var req = new CreateTemplateActionRequest("A", "D");
        var dto = new TemplateActionDto(1, "A", "D", 50);
        _templateRepo.Setup(r => r.CreateActionAsync(1, req, "System")).ReturnsAsync(dto);

        var result = await _controller.CreateAction(1, req);

        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
    }

    // UpdateAction
    [TestMethod]
    public async Task UpdateAction_ReturnsNotFound_WhenMissing()
    {
        var req = new UpdateTemplateActionRequest("X", "D", 1);
        _templateRepo.Setup(r => r.UpdateActionAsync(99, req, "System")).ReturnsAsync((TemplateActionDto?)null);

        var result = await _controller.UpdateAction(99, req);

        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
    }

    // DeleteAction
    [TestMethod]
    public async Task DeleteAction_ReturnsNoContent_WhenDeleted()
    {
        _templateRepo.Setup(r => r.DeleteActionAsync(1)).ReturnsAsync(true);
        var result = await _controller.DeleteAction(1);
        Assert.IsInstanceOfType<NoContentResult>(result);
    }

    [TestMethod]
    public async Task DeleteAction_ReturnsNotFound_WhenMissing()
    {
        _templateRepo.Setup(r => r.DeleteActionAsync(99)).ReturnsAsync(false);
        var result = await _controller.DeleteAction(99);
        Assert.IsInstanceOfType<NotFoundResult>(result);
    }

    // ExportSet
    [TestMethod]
    public async Task ExportSet_ReturnsOk()
    {
        var dto = new TemplateExportDto("S", "D", "O", 1, [], new ExportMetadata(DateTime.UtcNow, "1.0.0", 1));
        _templateRepo.Setup(r => r.ExportSetAsync(1)).ReturnsAsync(dto);

        var result = await _controller.ExportSet(1);

        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
    }

    [TestMethod]
    public async Task ExportSet_ReturnsNotFound_WhenMissing()
    {
        _templateRepo.Setup(r => r.ExportSetAsync(99))
            .ThrowsAsync(new InvalidOperationException("Not found"));

        var result = await _controller.ExportSet(99);

        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
    }

    // ExportAll
    [TestMethod]
    public async Task ExportAll_ReturnsOk()
    {
        _templateRepo.Setup(r => r.ExportAllAsync()).ReturnsAsync([]);
        var result = await _controller.ExportAll();
        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
    }

    // ImportSet
    [TestMethod]
    public async Task ImportSet_ReturnsCreated()
    {
        var import = new TemplateImportDto("S", "D", "O", 1, []);
        var dto = new TemplateSetDto(1, "S", "D", "O", "Y", 1, []);
        _templateRepo.Setup(r => r.ImportSetAsync(import, "System")).ReturnsAsync(dto);

        var result = await _controller.ImportSet(import);

        Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
    }
}
