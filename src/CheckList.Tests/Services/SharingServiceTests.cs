using CheckList.Tests.Helpers;
using System.Text;
using System.Security.Cryptography;

namespace CheckList.Tests.Services;

[TestClass]
public sealed class SharingServiceTests
{
    private Mock<ISharingRepository> _sharingRepo = null!;
    private CheckListDbContext _db = null!;
    private SharingService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _sharingRepo = new Mock<ISharingRepository>();
        _db = DbContextHelper.CreateInMemoryContext();
        _service = new SharingService(_sharingRepo.Object, _db);
    }

    [TestCleanup]
    public void Cleanup() => _db.Dispose();

    // CreateInviteAsync
    [TestMethod]
    public async Task CreateInviteAsync_GeneratesUniqueTokenEachTime()
    {
        var (token1, _) = await _service.CreateInviteAsync("user-sender", "recipient@example.com", "user");
        var (token2, _) = await _service.CreateInviteAsync("user-sender", "recipient@example.com", "user");

        Assert.AreNotEqual(token1, token2);
        Assert.IsTrue(token1.Length > 40);
        Assert.IsTrue(token2.Length > 40);
    }

    [TestMethod]
    public async Task CreateInviteAsync_StoresSHA256Hash()
    {
        SharingInvite? capturedInvite = null;
        _sharingRepo.Setup(r => r.CreateInviteAsync(It.IsAny<SharingInvite>()))
            .Callback<SharingInvite>(i => capturedInvite = i)
            .ReturnsAsync((SharingInvite i) => i);

        var (token, _) = await _service.CreateInviteAsync("user-sender", "recipient@example.com", "user");

        Assert.IsNotNull(capturedInvite);
        var expectedHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        Assert.AreEqual(expectedHash, capturedInvite.InviteTokenHash);
        Assert.AreNotEqual(token, capturedInvite.InviteTokenHash);
    }

    [TestMethod]
    public async Task CreateInviteAsync_Sets7DayExpiry()
    {
        SharingInvite? capturedInvite = null;
        _sharingRepo.Setup(r => r.CreateInviteAsync(It.IsAny<SharingInvite>()))
            .Callback<SharingInvite>(i => capturedInvite = i)
            .ReturnsAsync((SharingInvite i) => i);

        var beforeCreate = DateTime.UtcNow;
        await _service.CreateInviteAsync("user-sender", "recipient@example.com", "user");
        var afterCreate = DateTime.UtcNow;

        Assert.IsNotNull(capturedInvite);
        Assert.IsTrue(capturedInvite.ExpiresAt >= beforeCreate.AddDays(7).AddSeconds(-1));
        Assert.IsTrue(capturedInvite.ExpiresAt <= afterCreate.AddDays(7).AddSeconds(1));
    }

    [TestMethod]
    public async Task CreateInviteAsync_NormalizesEmailToLowercase()
    {
        SharingInvite? capturedInvite = null;
        _sharingRepo.Setup(r => r.CreateInviteAsync(It.IsAny<SharingInvite>()))
            .Callback<SharingInvite>(i => capturedInvite = i)
            .ReturnsAsync((SharingInvite i) => i);

        await _service.CreateInviteAsync("user-sender", "Recipient@Example.COM", "user");

        Assert.IsNotNull(capturedInvite);
        Assert.AreEqual("recipient@example.com", capturedInvite.RecipientEmail);
    }

    // AcceptInviteAsync
    [TestMethod]
    public async Task AcceptInviteAsync_HappyPath_CreatesPartnershipsAndAutoShares()
    {
        var sender = new AppUser
        {
            UserId = "user-sender",
            DisplayName = "Sender Name",
            Email = "sender@example.com",
            CreateDateTime = DateTime.UtcNow
        };
        _db.AppUsers.Add(sender);
        var senderCheckSet = DbContextHelper.SeedCheckSet(_db, "Sender Set", "Sender");
        senderCheckSet.OwnerId = "user-sender";
        _db.SaveChanges();

        var invite = new SharingInvite
        {
            InviteId = 1,
            InviteTokenHash = "hash123",
            SenderUserId = "user-sender",
            RecipientEmail = "acceptor@example.com",
            Role = "user",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            Sender = sender
        };

        var token = "rawtoken123";
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        invite.InviteTokenHash = tokenHash;

        _sharingRepo.Setup(r => r.GetInviteByTokenHashAsync(tokenHash)).ReturnsAsync(invite);
        _sharingRepo.Setup(r => r.CreatePartnershipAsync(It.IsAny<UserPartnership>()))
            .ReturnsAsync((UserPartnership p) => { p.PartnershipId = 1; return p; });

        var result = await _service.AcceptInviteAsync(token, "user-acceptor", "acceptor@example.com", "Acceptor Name");

        Assert.IsTrue(result.Success);
        Assert.AreEqual("Sender Name", result.PartnerDisplayName);
        _sharingRepo.Verify(r => r.CreatePartnershipAsync(It.Is<UserPartnership>(p => p.UserId == "user-sender" && p.PartnerUserId == "user-acceptor")), Times.Once);
        _sharingRepo.Verify(r => r.CreatePartnershipAsync(It.Is<UserPartnership>(p => p.UserId == "user-acceptor" && p.PartnerUserId == "user-sender")), Times.Once);
        _sharingRepo.Verify(r => r.AcceptInviteAsync(1, "user-acceptor"), Times.Once);
    }

    [TestMethod]
    public async Task AcceptInviteAsync_RejectsExpiredInvite()
    {
        var sender = new AppUser { UserId = "user-sender", DisplayName = "Sender", Email = "sender@example.com", CreateDateTime = DateTime.UtcNow };
        var invite = new SharingInvite
        {
            InviteId = 1,
            InviteTokenHash = "hash123",
            SenderUserId = "user-sender",
            RecipientEmail = "acceptor@example.com",
            Role = "user",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-8),
            Sender = sender
        };

        var token = "rawtoken123";
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        invite.InviteTokenHash = tokenHash;

        _sharingRepo.Setup(r => r.GetInviteByTokenHashAsync(tokenHash)).ReturnsAsync(invite);

        var result = await _service.AcceptInviteAsync(token, "user-acceptor", "acceptor@example.com", "Acceptor");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("This invite has expired.", result.ErrorMessage);
        _sharingRepo.Verify(r => r.CreatePartnershipAsync(It.IsAny<UserPartnership>()), Times.Never);
    }

    [TestMethod]
    public async Task AcceptInviteAsync_RejectsAlreadyUsedInvite()
    {
        var sender = new AppUser { UserId = "user-sender", DisplayName = "Sender", Email = "sender@example.com", CreateDateTime = DateTime.UtcNow };
        var invite = new SharingInvite
        {
            InviteId = 1,
            InviteTokenHash = "hash123",
            SenderUserId = "user-sender",
            RecipientEmail = "acceptor@example.com",
            Role = "user",
            Status = "accepted",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            AcceptedByUserId = "user-other",
            AcceptedAt = DateTime.UtcNow.AddMinutes(-10),
            Sender = sender
        };

        var token = "rawtoken123";
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        invite.InviteTokenHash = tokenHash;

        _sharingRepo.Setup(r => r.GetInviteByTokenHashAsync(tokenHash)).ReturnsAsync(invite);

        var result = await _service.AcceptInviteAsync(token, "user-acceptor", "acceptor@example.com", "Acceptor");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("This invite has already been used or revoked.", result.ErrorMessage);
    }

    [TestMethod]
    public async Task AcceptInviteAsync_RejectsWhenAcceptorEmailDoesNotMatch()
    {
        var sender = new AppUser { UserId = "user-sender", DisplayName = "Sender", Email = "sender@example.com", CreateDateTime = DateTime.UtcNow };
        var invite = new SharingInvite
        {
            InviteId = 1,
            InviteTokenHash = "hash123",
            SenderUserId = "user-sender",
            RecipientEmail = "recipient@example.com",
            Role = "user",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            Sender = sender
        };

        var token = "rawtoken123";
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        invite.InviteTokenHash = tokenHash;

        _sharingRepo.Setup(r => r.GetInviteByTokenHashAsync(tokenHash)).ReturnsAsync(invite);

        var result = await _service.AcceptInviteAsync(token, "user-acceptor", "wrongemail@example.com", "Acceptor");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("This invite was sent to a different email address.", result.ErrorMessage);
    }

    [TestMethod]
    public async Task AcceptInviteAsync_RejectsInvalidToken()
    {
        _sharingRepo.Setup(r => r.GetInviteByTokenHashAsync(It.IsAny<string>())).ReturnsAsync((SharingInvite?)null);

        var result = await _service.AcceptInviteAsync("invalid-token", "user-acceptor", "acceptor@example.com", "Acceptor");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Invalid or expired invite link.", result.ErrorMessage);
    }

    [TestMethod]
    public async Task AcceptInviteAsync_CreatesAppUserForNewAcceptor()
    {
        var sender = new AppUser
        {
            UserId = "user-sender",
            DisplayName = "Sender Name",
            Email = "sender@example.com",
            CreateDateTime = DateTime.UtcNow
        };
        _db.AppUsers.Add(sender);
        _db.SaveChanges();

        var invite = new SharingInvite
        {
            InviteId = 1,
            InviteTokenHash = "hash123",
            SenderUserId = "user-sender",
            RecipientEmail = "newuser@example.com",
            Role = "user",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            Sender = sender
        };

        var token = "rawtoken123";
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        invite.InviteTokenHash = tokenHash;

        _sharingRepo.Setup(r => r.GetInviteByTokenHashAsync(tokenHash)).ReturnsAsync(invite);
        _sharingRepo.Setup(r => r.CreatePartnershipAsync(It.IsAny<UserPartnership>()))
            .ReturnsAsync((UserPartnership p) => { p.PartnershipId = 1; return p; });

        var result = await _service.AcceptInviteAsync(token, "user-newacceptor", "newuser@example.com", "New User");

        Assert.IsTrue(result.Success);
        var acceptor = await _db.AppUsers.FindAsync("user-newacceptor");
        Assert.IsNotNull(acceptor);
        Assert.AreEqual("New User", acceptor.DisplayName);
        Assert.AreEqual("newuser@example.com", acceptor.Email);
    }

    [TestMethod]
    public async Task AcceptInviteAsync_CreatesTwoPartnershipRows()
    {
        var sender = new AppUser
        {
            UserId = "user-sender",
            DisplayName = "Sender",
            Email = "sender@example.com",
            CreateDateTime = DateTime.UtcNow
        };
        _db.AppUsers.Add(sender);
        _db.SaveChanges();

        var invite = new SharingInvite
        {
            InviteId = 1,
            InviteTokenHash = "hash123",
            SenderUserId = "user-sender",
            RecipientEmail = "acceptor@example.com",
            Role = "admin",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            Sender = sender
        };

        var token = "rawtoken123";
        var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        invite.InviteTokenHash = tokenHash;

        _sharingRepo.Setup(r => r.GetInviteByTokenHashAsync(tokenHash)).ReturnsAsync(invite);
        _sharingRepo.Setup(r => r.CreatePartnershipAsync(It.IsAny<UserPartnership>()))
            .ReturnsAsync((UserPartnership p) => { p.PartnershipId = 1; return p; });

        await _service.AcceptInviteAsync(token, "user-acceptor", "acceptor@example.com", "Acceptor");

        _sharingRepo.Verify(r => r.CreatePartnershipAsync(It.Is<UserPartnership>(
            p => p.UserId == "user-sender" && p.PartnerUserId == "user-acceptor" && p.Role == "admin" && p.AutoShareEnabled)), Times.Once);
        _sharingRepo.Verify(r => r.CreatePartnershipAsync(It.Is<UserPartnership>(
            p => p.UserId == "user-acceptor" && p.PartnerUserId == "user-sender" && p.Role == "admin" && p.AutoShareEnabled)), Times.Once);
    }

    // GetPartnersAsync
    [TestMethod]
    public async Task GetPartnersAsync_MapsToPartnerDtoCorrectly()
    {
        var partnerships = new List<UserPartnership>
        {
            new()
            {
                PartnershipId = 1,
                UserId = "user-alice",
                PartnerUserId = "user-bob",
                Role = "user",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Partner = new AppUser { UserId = "user-bob", DisplayName = "Bob", Email = "bob@example.com", CreateDateTime = DateTime.UtcNow }
            },
            new()
            {
                PartnershipId = 2,
                UserId = "user-alice",
                PartnerUserId = "user-charlie",
                Role = "admin",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Partner = new AppUser { UserId = "user-charlie", DisplayName = "Charlie", Email = "charlie@example.com", CreateDateTime = DateTime.UtcNow }
            }
        };

        _sharingRepo.Setup(r => r.GetPartnershipsForUserAsync("user-alice")).ReturnsAsync(partnerships);

        var result = await _service.GetPartnersAsync("user-alice");

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("user-bob", result[0].UserId);
        Assert.AreEqual("Bob", result[0].DisplayName);
        Assert.AreEqual("bob@example.com", result[0].Email);
        Assert.AreEqual("user", result[0].Role);
        Assert.AreEqual("user-charlie", result[1].UserId);
        Assert.AreEqual("Charlie", result[1].DisplayName);
        Assert.AreEqual("admin", result[1].Role);
    }

    // RevokePartnershipAsync
    [TestMethod]
    public async Task RevokePartnershipAsync_DeletesPartnershipsAndAutoCreatedShares()
    {
        var partnerships = new List<UserPartnership>
        {
            new() { PartnershipId = 1, UserId = "user-alice", PartnerUserId = "user-bob", Role = "user", AutoShareEnabled = true, CreatedAt = DateTime.UtcNow },
            new() { PartnershipId = 2, UserId = "user-bob", PartnerUserId = "user-alice", Role = "user", AutoShareEnabled = true, CreatedAt = DateTime.UtcNow }
        };

        _db.UserPartnerships.AddRange(partnerships);
        await _db.SaveChangesAsync();

        await _service.RevokePartnershipAsync("user-alice", "user-bob");

        _sharingRepo.Verify(r => r.DeleteSharesByPartnershipAsync(1), Times.Once);
        _sharingRepo.Verify(r => r.DeleteSharesByPartnershipAsync(2), Times.Once);
        _sharingRepo.Verify(r => r.RevokePartnershipAsync("user-alice", "user-bob"), Times.Once);
    }

    // AutoShareNewCheckSetAsync
    [TestMethod]
    public async Task AutoShareNewCheckSetAsync_CreatesSharesForAllPartners()
    {
        var checkSet = DbContextHelper.SeedCheckSet(_db, "New Set", "Owner");
        checkSet.OwnerId = "user-owner";
        _db.SaveChanges();

        var partnership1 = new UserPartnership
        {
            PartnershipId = 1,
            UserId = "user-owner",
            PartnerUserId = "user-alice",
            Role = "user",
            AutoShareEnabled = true,
            CreatedAt = DateTime.UtcNow
        };
        var partnership2 = new UserPartnership
        {
            PartnershipId = 2,
            UserId = "user-owner",
            PartnerUserId = "user-bob",
            Role = "admin",
            AutoShareEnabled = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.UserPartnerships.AddRange(partnership1, partnership2);
        await _db.SaveChangesAsync();

        _sharingRepo.Setup(r => r.GetPartnerUserIdsAsync("user-owner")).ReturnsAsync(["user-alice", "user-bob"]);

        await _service.AutoShareNewCheckSetAsync(checkSet.SetId, "user-owner");

        _sharingRepo.Verify(r => r.CreateCheckSetShareAsync(It.Is<CheckSetShare>(
            s => s.CheckSetId == checkSet.SetId && s.SharedWithUserId == "user-alice" && s.Role == "user" && s.PartnershipId == 1)), Times.Once);
        _sharingRepo.Verify(r => r.CreateCheckSetShareAsync(It.Is<CheckSetShare>(
            s => s.CheckSetId == checkSet.SetId && s.SharedWithUserId == "user-bob" && s.Role == "admin" && s.PartnershipId == 2)), Times.Once);
    }

    [TestMethod]
    public async Task AutoShareNewCheckSetAsync_HandlesNoPartners()
    {
        var checkSet = DbContextHelper.SeedCheckSet(_db, "New Set", "Owner");
        checkSet.OwnerId = "user-owner";
        _db.SaveChanges();

        _sharingRepo.Setup(r => r.GetPartnerUserIdsAsync("user-owner")).ReturnsAsync([]);

        await _service.AutoShareNewCheckSetAsync(checkSet.SetId, "user-owner");

        _sharingRepo.Verify(r => r.CreateCheckSetShareAsync(It.IsAny<CheckSetShare>()), Times.Never);
    }

    // UserHasAccessAsync
    [TestMethod]
    public async Task UserHasAccessAsync_OwnerHasAccess()
    {
        var checkSet = DbContextHelper.SeedCheckSet(_db, "Owner Set", "Owner");
        checkSet.OwnerId = "user-owner";
        _db.SaveChanges();

        var result = await _service.UserHasAccessAsync("user-owner", checkSet.SetId);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task UserHasAccessAsync_SharedUserHasAccess()
    {
        var checkSet = DbContextHelper.SeedCheckSet(_db, "Shared Set", "Owner");
        checkSet.OwnerId = "user-owner";
        _db.SaveChanges();

        var share = new CheckSetShare
        {
            CheckSetId = checkSet.SetId,
            SharedWithUserId = "user-shared",
            Role = "user",
            CreateDateTime = DateTime.UtcNow,
            CreateUserName = "System"
        };
        _db.CheckSetShares.Add(share);
        await _db.SaveChangesAsync();

        var result = await _service.UserHasAccessAsync("user-shared", checkSet.SetId);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task UserHasAccessAsync_NonSharedUserDenied()
    {
        var checkSet = DbContextHelper.SeedCheckSet(_db, "Private Set", "Owner");
        checkSet.OwnerId = "user-owner";
        _db.SaveChanges();

        var result = await _service.UserHasAccessAsync("user-other", checkSet.SetId);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task UserHasAccessAsync_AdminRoleGrantsAccessWhenUserRoleRequired()
    {
        var checkSet = DbContextHelper.SeedCheckSet(_db, "Shared Set", "Owner");
        checkSet.OwnerId = "user-owner";
        _db.SaveChanges();

        var share = new CheckSetShare
        {
            CheckSetId = checkSet.SetId,
            SharedWithUserId = "user-admin",
            Role = "admin",
            CreateDateTime = DateTime.UtcNow,
            CreateUserName = "System"
        };
        _db.CheckSetShares.Add(share);
        await _db.SaveChangesAsync();

        var result = await _service.UserHasAccessAsync("user-admin", checkSet.SetId, "user");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task UserHasAccessAsync_UserRoleDeniedWhenAdminRequired()
    {
        var checkSet = DbContextHelper.SeedCheckSet(_db, "Shared Set", "Owner");
        checkSet.OwnerId = "user-owner";
        _db.SaveChanges();

        var share = new CheckSetShare
        {
            CheckSetId = checkSet.SetId,
            SharedWithUserId = "user-basic",
            Role = "user",
            CreateDateTime = DateTime.UtcNow,
            CreateUserName = "System"
        };
        _db.CheckSetShares.Add(share);
        await _db.SaveChangesAsync();

        var result = await _service.UserHasAccessAsync("user-basic", checkSet.SetId, "admin");

        Assert.IsFalse(result);
    }
}
