namespace CheckList.Tests.Hubs;

[TestClass]
public sealed class CheckListHubTests
{
    private Mock<IHubCallerClients<ICheckListHubClient>> _clients = null!;
    private Mock<ICheckListHubClient> _othersInGroup = null!;
    private Mock<HubCallerContext> _context = null!;
    private Mock<IGroupManager> _groups = null!;
    private CheckListHub _hub = null!;

    [TestInitialize]
    public void Setup()
    {
        _clients = new Mock<IHubCallerClients<ICheckListHubClient>>();
        _othersInGroup = new Mock<ICheckListHubClient>();
        _context = new Mock<HubCallerContext>();
        _groups = new Mock<IGroupManager>();

        _context.Setup(c => c.ConnectionId).Returns("conn-123");
        _clients.Setup(c => c.OthersInGroup(It.IsAny<string>())).Returns(_othersInGroup.Object);

        _hub = new CheckListHub
        {
            Clients = _clients.Object,
            Context = _context.Object,
            Groups = _groups.Object
        };
    }

    [TestMethod]
    public async Task JoinCheckSet_AddsToGroup_AndNotifiesOthers()
    {
        await _hub.JoinCheckSet(42, "Alice");

        _groups.Verify(g => g.AddToGroupAsync("conn-123", "checkset-42", default), Times.Once);
        _othersInGroup.Verify(c => c.UserJoined("Alice", 42), Times.Once);
    }

    [TestMethod]
    public async Task LeaveCheckSet_RemovesFromGroup_AndNotifiesOthers()
    {
        await _hub.LeaveCheckSet(42, "Alice");

        _groups.Verify(g => g.RemoveFromGroupAsync("conn-123", "checkset-42", default), Times.Once);
        _othersInGroup.Verify(c => c.UserLeft("Alice", 42), Times.Once);
    }

    [TestMethod]
    public async Task JoinCheckSet_UsesCorrectGroupName()
    {
        await _hub.JoinCheckSet(99, "Bob");

        _groups.Verify(g => g.AddToGroupAsync(It.IsAny<string>(), "checkset-99", default), Times.Once);
    }
}
