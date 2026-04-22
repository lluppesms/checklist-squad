namespace CheckList.Tests.Controllers;

[TestClass]
public sealed class CheckListsControllerTests
{
    private Mock<ICheckRepository> _checkRepo = null!;
    private Mock<IHubContext<CheckListHub, ICheckListHubClient>> _hubContext = null!;
    private Mock<ICheckListHubClient> _hubClients = null!;
    private CheckListsController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _checkRepo = new Mock<ICheckRepository>();
        _hubContext = new Mock<IHubContext<CheckListHub, ICheckListHubClient>>();
        _hubClients = new Mock<ICheckListHubClient>();

        var mockClientsAll = new Mock<IHubClients<ICheckListHubClient>>();
        mockClientsAll.Setup(c => c.All).Returns(_hubClients.Object);
        _hubContext.Setup(h => h.Clients).Returns(mockClientsAll.Object);

        _controller = new CheckListsController(_checkRepo.Object, _hubContext.Object);
    }

    // Activate
    [TestMethod]
    public async Task Activate_ReturnsCreated_WhenSuccessful()
    {
        var checkSet = new CheckSet
        {
            SetId = 1, SetName = "Active", OwnerName = "O", ActiveInd = "Y"
        };
        _checkRepo.Setup(r => r.ActivateFromTemplateAsync(1, "Owner", null)).ReturnsAsync(checkSet);

        var result = await _controller.Activate(1, new ActivateCheckSetRequest("Owner"));

        Assert.IsInstanceOfType<CreatedAtActionResult>(result.Result);
    }

    [TestMethod]
    public async Task Activate_BroadcastsSignalR()
    {
        var checkSet = new CheckSet { SetId = 5, SetName = "Test", OwnerName = "O", ActiveInd = "Y" };
        _checkRepo.Setup(r => r.ActivateFromTemplateAsync(1, "O", null)).ReturnsAsync(checkSet);

        await _controller.Activate(1, new ActivateCheckSetRequest("O"));

        _hubClients.Verify(c => c.CheckSetActivated(5, "Test"), Times.Once);
    }

    [TestMethod]
    public async Task Activate_ReturnsNotFound_WhenTemplateNotFound()
    {
        _checkRepo.Setup(r => r.ActivateFromTemplateAsync(99, "O", null))
            .ThrowsAsync(new KeyNotFoundException("Not found"));

        var result = await _controller.Activate(99, new ActivateCheckSetRequest("O"));

        Assert.IsInstanceOfType<NotFoundObjectResult>(result.Result);
    }

    [TestMethod]
    public async Task Activate_ReturnsBadRequest_WhenArgumentException()
    {
        _checkRepo.Setup(r => r.ActivateFromTemplateAsync(1, "O", It.IsAny<List<int>>()))
            .ThrowsAsync(new ArgumentException("Bad"));

        var result = await _controller.Activate(1, new ActivateCheckSetRequest("O", [999]));

        Assert.IsInstanceOfType<BadRequestObjectResult>(result.Result);
    }

    // GetAll
    [TestMethod]
    public async Task GetAll_ReturnsOk_WithSummaries()
    {
        var now = DateTime.UtcNow;
        _checkRepo.Setup(r => r.GetAllActiveSetsAsync()).ReturnsAsync(
        [
            new CheckSet { SetId = 1, SetName = "A", OwnerName = "O", ActiveInd = "Y", CreateDateTime = now }
        ]);

        var result = await _controller.GetAll();

        var ok = result.Result as OkObjectResult;
        Assert.IsNotNull(ok);
        var list = ok.Value as List<CheckSetSummaryDto>;
        Assert.AreEqual(1, list?.Count);
    }

    [TestMethod]
    public async Task GetAll_ReturnsEmpty_WhenNone()
    {
        _checkRepo.Setup(r => r.GetAllActiveSetsAsync()).ReturnsAsync([]);

        var result = await _controller.GetAll();

        var ok = result.Result as OkObjectResult;
        Assert.IsNotNull(ok);
        var list = ok.Value as List<CheckSetSummaryDto>;
        Assert.AreEqual(0, list?.Count);
    }

    // GetById
    [TestMethod]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var set = new CheckSet { SetId = 1, SetName = "S", OwnerName = "O", ActiveInd = "Y" };
        _checkRepo.Setup(r => r.GetSetWithHierarchyAsync(1)).ReturnsAsync(set);

        var result = await _controller.GetById(1);

        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
    }

    [TestMethod]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        _checkRepo.Setup(r => r.GetSetWithHierarchyAsync(99)).ReturnsAsync((CheckSet?)null);

        var result = await _controller.GetById(99);

        Assert.IsInstanceOfType<NotFoundResult>(result.Result);
    }

    // ToggleAction
    [TestMethod]
    public async Task ToggleAction_ReturnsOk_AndBroadcasts()
    {
        var now = DateTime.UtcNow;
        var action = new CheckAction
        {
            ActionId = 5, ActionText = "Do it", CompleteInd = "Y",
            ChangeDateTime = now, ChangeUserName = "User"
        };
        _checkRepo.Setup(r => r.ToggleActionAsync(5, "User")).ReturnsAsync(action);

        var result = await _controller.ToggleAction(5, new ToggleActionRequest("User"));

        Assert.IsInstanceOfType<OkObjectResult>(result.Result);
        _hubClients.Verify(c => c.ActionToggled(5, "Y", "User", now), Times.Once);
    }

    [TestMethod]
    public async Task ToggleAction_ReturnsNotFound_WhenMissing()
    {
        _checkRepo.Setup(r => r.ToggleActionAsync(99, "U"))
            .ThrowsAsync(new KeyNotFoundException("Not found"));

        var result = await _controller.ToggleAction(99, new ToggleActionRequest("U"));

        Assert.IsInstanceOfType<NotFoundObjectResult>(result.Result);
    }

    // Delete
    [TestMethod]
    public async Task Delete_ReturnsNoContent_WhenDeleted()
    {
        _checkRepo.Setup(r => r.DeleteSetAsync(1)).ReturnsAsync(true);

        var result = await _controller.Delete(1);

        Assert.IsInstanceOfType<NoContentResult>(result);
        _hubClients.Verify(c => c.CheckSetDeleted(1), Times.Once);
    }

    [TestMethod]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        _checkRepo.Setup(r => r.DeleteSetAsync(99)).ReturnsAsync(false);

        var result = await _controller.Delete(99);

        Assert.IsInstanceOfType<NotFoundResult>(result);
    }
}
