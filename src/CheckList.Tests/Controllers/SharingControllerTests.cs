namespace CheckList.Tests.Controllers;

[TestClass]
public sealed class SharingControllerTests
{
    private Mock<ISharingService> _sharingService = null!;
    private Mock<IEmailService> _emailService = null!;
    private Mock<IUserIdentity> _userIdentity = null!;
    private SharingController _controller = null!;

    [TestInitialize]
    public void Setup()
    {
        _sharingService = new Mock<ISharingService>();
        _emailService = new Mock<IEmailService>();
        _userIdentity = new Mock<IUserIdentity>();

        _userIdentity.Setup(u => u.UserId).Returns("user-current");
        _userIdentity.Setup(u => u.Email).Returns("current@example.com");
        _userIdentity.Setup(u => u.NickName).Returns("Current User");

        _controller = new SharingController(_sharingService.Object, _emailService.Object, _userIdentity.Object);

        // Mock HTTP context for controller
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("localhost");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    // POST /api/sharing/invite
    [TestMethod]
    public async Task CreateInvite_ReturnsInviteLinkAndToken()
    {
        var invite = new SharingInvite
        {
            InviteId = 1,
            InviteTokenHash = "hash123",
            SenderUserId = "user-current",
            RecipientEmail = "recipient@example.com",
            Role = "user",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _sharingService.Setup(s => s.CreateInviteAsync("user-current", "recipient@example.com", "user"))
            .ReturnsAsync(("token123", invite));

        var request = new CreateInviteRequest("recipient@example.com", "user");
        var result = await _controller.CreateInvite(request);

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        dynamic? value = okResult.Value;
        Assert.IsNotNull(value);
        
        var inviteToken = value.GetType().GetProperty("inviteToken")?.GetValue(value, null)?.ToString();
        var inviteLink = value.GetType().GetProperty("inviteLink")?.GetValue(value, null)?.ToString();
        
        Assert.AreEqual("token123", inviteToken);
        Assert.IsTrue(inviteLink?.Contains("/invite/token123"));
    }

    [TestMethod]
    public async Task CreateInvite_Returns401_WhenNotAuthenticated()
    {
        _userIdentity.Setup(u => u.UserId).Returns((string?)null);

        var request = new CreateInviteRequest("recipient@example.com", "user");
        var result = await _controller.CreateInvite(request);

        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public async Task CreateInvite_Returns400_OnServiceError()
    {
        _sharingService.Setup(s => s.CreateInviteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        var request = new CreateInviteRequest("recipient@example.com", "user");
        var result = await _controller.CreateInvite(request);

        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [TestMethod]
    public async Task CreateInvite_SendsEmailInvite()
    {
        var invite = new SharingInvite
        {
            InviteId = 1,
            InviteTokenHash = "hash123",
            SenderUserId = "user-current",
            RecipientEmail = "recipient@example.com",
            Role = "admin",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        _sharingService.Setup(s => s.CreateInviteAsync("user-current", "recipient@example.com", "admin"))
            .ReturnsAsync(("token123", invite));

        var request = new CreateInviteRequest("recipient@example.com", "admin");
        await _controller.CreateInvite(request);

        _emailService.Verify(e => e.SendSharingInviteAsync(
            "recipient@example.com",
            "Current User",
            It.Is<string>(link => link.Contains("/invite/token123")),
            "admin"), Times.Once);
    }

    // POST /api/sharing/accept/{token}
    [TestMethod]
    public async Task AcceptInvite_ReturnsSuccessWithPartnerName()
    {
        _sharingService.Setup(s => s.AcceptInviteAsync(
            "token123",
            "user-current",
            "current@example.com",
            "Current User"))
            .ReturnsAsync(new SharingInviteResult(true, PartnerDisplayName: "Partner Name"));

        var result = await _controller.AcceptInvite("token123");

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        dynamic? value = okResult.Value;
        Assert.IsNotNull(value);
        
        var success = (bool?)value.GetType().GetProperty("success")?.GetValue(value, null);
        var partnerName = value.GetType().GetProperty("partnerName")?.GetValue(value, null)?.ToString();
        
        Assert.IsTrue(success);
        Assert.AreEqual("Partner Name", partnerName);
    }

    [TestMethod]
    public async Task AcceptInvite_Returns400_WhenInviteInvalid()
    {
        _sharingService.Setup(s => s.AcceptInviteAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>()))
            .ReturnsAsync(new SharingInviteResult(false, "Invalid invite"));

        var result = await _controller.AcceptInvite("invalid-token");

        var badRequestResult = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }

    [TestMethod]
    public async Task AcceptInvite_Returns401_WhenNotAuthenticated()
    {
        _userIdentity.Setup(u => u.UserId).Returns((string?)null);

        var result = await _controller.AcceptInvite("token123");

        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public async Task AcceptInvite_Returns401_WhenEmailMissing()
    {
        _userIdentity.Setup(u => u.Email).Returns((string?)null);

        var result = await _controller.AcceptInvite("token123");

        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public async Task AcceptInvite_Returns401_WhenDisplayNameMissing()
    {
        _userIdentity.Setup(u => u.NickName).Returns((string?)null);

        var result = await _controller.AcceptInvite("token123");

        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    // GET /api/sharing/partners
    [TestMethod]
    public async Task GetPartners_ReturnsPartnerList()
    {
        var partners = new List<PartnerDto>
        {
            new("user-bob", "Bob", "bob@example.com", "user", DateTime.UtcNow.AddDays(-5)),
            new("user-charlie", "Charlie", "charlie@example.com", "admin", DateTime.UtcNow.AddDays(-2))
        };

        _sharingService.Setup(s => s.GetPartnersAsync("user-current")).ReturnsAsync(partners);

        var result = await _controller.GetPartners();

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var returnedPartners = okResult.Value as List<PartnerDto>;
        Assert.IsNotNull(returnedPartners);
        Assert.AreEqual(2, returnedPartners.Count);
        Assert.AreEqual("Bob", returnedPartners[0].DisplayName);
        Assert.AreEqual("Charlie", returnedPartners[1].DisplayName);
    }

    [TestMethod]
    public async Task GetPartners_ReturnsEmptyList_WhenNoPartners()
    {
        _sharingService.Setup(s => s.GetPartnersAsync("user-current")).ReturnsAsync([]);

        var result = await _controller.GetPartners();

        var okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);

        var returnedPartners = okResult.Value as List<PartnerDto>;
        Assert.IsNotNull(returnedPartners);
        Assert.AreEqual(0, returnedPartners.Count);
    }

    [TestMethod]
    public async Task GetPartners_Returns401_WhenNotAuthenticated()
    {
        _userIdentity.Setup(u => u.UserId).Returns((string?)null);

        var result = await _controller.GetPartners();

        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    // DELETE /api/sharing/partners/{id}
    [TestMethod]
    public async Task RevokePartnership_Returns204_OnSuccess()
    {
        _sharingService.Setup(s => s.RevokePartnershipAsync("user-current", "user-partner"))
            .Returns(Task.CompletedTask);

        var result = await _controller.RevokePartnership("user-partner");

        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult);
        Assert.AreEqual(204, noContentResult.StatusCode);
    }

    [TestMethod]
    public async Task RevokePartnership_Returns401_WhenNotAuthenticated()
    {
        _userIdentity.Setup(u => u.UserId).Returns((string?)null);

        var result = await _controller.RevokePartnership("user-partner");

        var unauthorizedResult = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public async Task RevokePartnership_Returns400_OnServiceError()
    {
        _sharingService.Setup(s => s.RevokePartnershipAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Service error"));

        var result = await _controller.RevokePartnership("user-partner");

        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
    }
}
