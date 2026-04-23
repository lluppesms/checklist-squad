using CheckList.Tests.Helpers;

namespace CheckList.Tests.Repositories;

[TestClass]
public sealed class SharingRepositoryTests
{
    private CheckListDbContext _db = null!;
    private SharingRepository _repo = null!;

    [TestInitialize]
    public void Setup()
    {
        _db = DbContextHelper.CreateInMemoryContext();
        _repo = new SharingRepository(_db);
    }

    [TestCleanup]
    public void Cleanup() => _db.Dispose();

    // CreateInviteAsync
    [TestMethod]
    public async Task CreateInviteAsync_CreatesInviteWithAllFields()
    {
        var now = DateTime.UtcNow;
        var invite = new SharingInvite
        {
            InviteTokenHash = "abc123",
            SenderUserId = "user-sender",
            RecipientEmail = "recipient@example.com",
            Role = "user",
            Status = "pending",
            ExpiresAt = now.AddDays(7),
            CreatedAt = now
        };

        var result = await _repo.CreateInviteAsync(invite);

        Assert.IsNotNull(result);
        Assert.AreEqual("abc123", result.InviteTokenHash);
        Assert.AreEqual("user-sender", result.SenderUserId);
        Assert.AreEqual("recipient@example.com", result.RecipientEmail);
        Assert.AreEqual("user", result.Role);
        Assert.AreEqual("pending", result.Status);
    }

    // GetInviteByTokenHashAsync
    [TestMethod]
    public async Task GetInviteByTokenHashAsync_FindsByHash()
    {
        var sender = new AppUser
        {
            UserId = "user-sender",
            DisplayName = "Sender Name",
            Email = "sender@example.com",
            CreateDateTime = DateTime.UtcNow
        };
        _db.AppUsers.Add(sender);
        await _db.SaveChangesAsync();

        var invite = new SharingInvite
        {
            InviteTokenHash = "hash123",
            SenderUserId = sender.UserId,
            RecipientEmail = "recipient@example.com",
            Role = "user",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        await _repo.CreateInviteAsync(invite);

        var result = await _repo.GetInviteByTokenHashAsync("hash123");

        Assert.IsNotNull(result);
        Assert.AreEqual("hash123", result.InviteTokenHash);
        Assert.IsNotNull(result.Sender);
        Assert.AreEqual("Sender Name", result.Sender.DisplayName);
    }

    [TestMethod]
    public async Task GetInviteByTokenHashAsync_ReturnsNull_WhenNotFound()
    {
        var result = await _repo.GetInviteByTokenHashAsync("unknown-hash");
        Assert.IsNull(result);
    }

    // GetInvitesForUserAsync
    [TestMethod]
    public async Task GetInvitesForUserAsync_ReturnsOnlySenderInvites()
    {
        var invite1 = new SharingInvite
        {
            InviteTokenHash = "hash1",
            SenderUserId = "user-alice",
            RecipientEmail = "bob@example.com",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        var invite2 = new SharingInvite
        {
            InviteTokenHash = "hash2",
            SenderUserId = "user-alice",
            RecipientEmail = "charlie@example.com",
            Status = "accepted",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow.AddMinutes(-5)
        };
        var invite3 = new SharingInvite
        {
            InviteTokenHash = "hash3",
            SenderUserId = "user-other",
            RecipientEmail = "alice@example.com",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        await _repo.CreateInviteAsync(invite1);
        await _repo.CreateInviteAsync(invite2);
        await _repo.CreateInviteAsync(invite3);

        var result = await _repo.GetInvitesForUserAsync("user-alice");

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("hash1", result[0].InviteTokenHash);
        Assert.AreEqual("hash2", result[1].InviteTokenHash);
    }

    // AcceptInviteAsync
    [TestMethod]
    public async Task AcceptInviteAsync_UpdatesStatusAndAcceptorFields()
    {
        var invite = new SharingInvite
        {
            InviteTokenHash = "hash1",
            SenderUserId = "user-sender",
            RecipientEmail = "recipient@example.com",
            Status = "pending",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };
        await _repo.CreateInviteAsync(invite);

        await _repo.AcceptInviteAsync(invite.InviteId, "user-acceptor");

        var updated = await _db.SharingInvites.FindAsync(invite.InviteId);
        Assert.IsNotNull(updated);
        Assert.AreEqual("accepted", updated.Status);
        Assert.AreEqual("user-acceptor", updated.AcceptedByUserId);
        Assert.IsNotNull(updated.AcceptedAt);
    }

    [TestMethod]
    public async Task AcceptInviteAsync_ThrowsKeyNotFoundException_WhenInviteNotFound()
    {
        await Assert.ThrowsExactlyAsync<KeyNotFoundException>(
            () => _repo.AcceptInviteAsync(9999, "user-acceptor"));
    }

    // CreatePartnershipAsync
    [TestMethod]
    public async Task CreatePartnershipAsync_CreatesPartnershipRecord()
    {
        var partnership = new UserPartnership
        {
            UserId = "user-alice",
            PartnerUserId = "user-bob",
            Role = "user",
            AutoShareEnabled = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _repo.CreatePartnershipAsync(partnership);

        Assert.IsNotNull(result);
        Assert.AreEqual("user-alice", result.UserId);
        Assert.AreEqual("user-bob", result.PartnerUserId);
        Assert.AreEqual("user", result.Role);
        Assert.IsTrue(result.AutoShareEnabled);
    }

    // GetPartnershipsForUserAsync
    [TestMethod]
    public async Task GetPartnershipsForUserAsync_ReturnsPartnershipsWithPartnerNavigation()
    {
        var alice = new AppUser { UserId = "user-alice", DisplayName = "Alice", Email = "alice@example.com", CreateDateTime = DateTime.UtcNow };
        var bob = new AppUser { UserId = "user-bob", DisplayName = "Bob", Email = "bob@example.com", CreateDateTime = DateTime.UtcNow };
        var charlie = new AppUser { UserId = "user-charlie", DisplayName = "Charlie", Email = "charlie@example.com", CreateDateTime = DateTime.UtcNow };
        _db.AppUsers.AddRange(alice, bob, charlie);
        await _db.SaveChangesAsync();

        var partnership1 = new UserPartnership
        {
            UserId = "user-alice",
            PartnerUserId = "user-bob",
            Role = "user",
            AutoShareEnabled = true,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        };
        var partnership2 = new UserPartnership
        {
            UserId = "user-alice",
            PartnerUserId = "user-charlie",
            Role = "admin",
            AutoShareEnabled = true,
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };
        await _repo.CreatePartnershipAsync(partnership1);
        await _repo.CreatePartnershipAsync(partnership2);

        var result = await _repo.GetPartnershipsForUserAsync("user-alice");

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("user-bob", result[0].PartnerUserId);
        Assert.IsNotNull(result[0].Partner);
        Assert.AreEqual("Bob", result[0].Partner.DisplayName);
        Assert.AreEqual("user-charlie", result[1].PartnerUserId);
        Assert.AreEqual("Charlie", result[1].Partner.DisplayName);
    }

    // RevokePartnershipAsync
    [TestMethod]
    public async Task RevokePartnershipAsync_DeletesBothDirectionalRows()
    {
        var partnership1 = new UserPartnership
        {
            UserId = "user-alice",
            PartnerUserId = "user-bob",
            Role = "user",
            AutoShareEnabled = true,
            CreatedAt = DateTime.UtcNow
        };
        var partnership2 = new UserPartnership
        {
            UserId = "user-bob",
            PartnerUserId = "user-alice",
            Role = "user",
            AutoShareEnabled = true,
            CreatedAt = DateTime.UtcNow
        };
        await _repo.CreatePartnershipAsync(partnership1);
        await _repo.CreatePartnershipAsync(partnership2);

        await _repo.RevokePartnershipAsync("user-alice", "user-bob");

        var remaining = await _db.UserPartnerships.ToListAsync();
        Assert.AreEqual(0, remaining.Count);
    }

    // GetPartnerUserIdsAsync
    [TestMethod]
    public async Task GetPartnerUserIdsAsync_ReturnsListOfPartnerIds()
    {
        var partnership1 = new UserPartnership
        {
            UserId = "user-alice",
            PartnerUserId = "user-bob",
            Role = "user",
            AutoShareEnabled = true,
            CreatedAt = DateTime.UtcNow
        };
        var partnership2 = new UserPartnership
        {
            UserId = "user-alice",
            PartnerUserId = "user-charlie",
            Role = "admin",
            AutoShareEnabled = true,
            CreatedAt = DateTime.UtcNow
        };
        var partnership3 = new UserPartnership
        {
            UserId = "user-alice",
            PartnerUserId = "user-dave",
            Role = "user",
            AutoShareEnabled = false,
            CreatedAt = DateTime.UtcNow
        };
        await _repo.CreatePartnershipAsync(partnership1);
        await _repo.CreatePartnershipAsync(partnership2);
        await _repo.CreatePartnershipAsync(partnership3);

        var result = await _repo.GetPartnerUserIdsAsync("user-alice");

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Contains("user-bob"));
        Assert.IsTrue(result.Contains("user-charlie"));
        Assert.IsFalse(result.Contains("user-dave"));
    }

    // CreateCheckSetShareAsync
    [TestMethod]
    public async Task CreateCheckSetShareAsync_CreatesShareWithPartnershipId()
    {
        var checkSet = DbContextHelper.SeedCheckSet(_db, "Test Set", "user-owner");
        
        var share = new CheckSetShare
        {
            CheckSetId = checkSet.SetId,
            SharedWithUserId = "user-partner",
            Role = "user",
            PartnershipId = 123,
            CreateDateTime = DateTime.UtcNow,
            CreateUserName = "System"
        };

        var result = await _repo.CreateCheckSetShareAsync(share);

        Assert.IsNotNull(result);
        Assert.AreEqual(checkSet.SetId, result.CheckSetId);
        Assert.AreEqual("user-partner", result.SharedWithUserId);
        Assert.AreEqual("user", result.Role);
        Assert.AreEqual(123, result.PartnershipId);
    }

    // GetSharesForCheckSetAsync
    [TestMethod]
    public async Task GetSharesForCheckSetAsync_ReturnsSharesForGivenSet()
    {
        var checkSet1 = DbContextHelper.SeedCheckSet(_db, "Set 1", "user-owner");
        var checkSet2 = DbContextHelper.SeedCheckSet(_db, "Set 2", "user-owner");

        var share1 = new CheckSetShare
        {
            CheckSetId = checkSet1.SetId,
            SharedWithUserId = "user-alice",
            Role = "user",
            PartnershipId = 1,
            CreateDateTime = DateTime.UtcNow,
            CreateUserName = "System"
        };
        var share2 = new CheckSetShare
        {
            CheckSetId = checkSet1.SetId,
            SharedWithUserId = "user-bob",
            Role = "admin",
            PartnershipId = 2,
            CreateDateTime = DateTime.UtcNow,
            CreateUserName = "System"
        };
        var share3 = new CheckSetShare
        {
            CheckSetId = checkSet2.SetId,
            SharedWithUserId = "user-charlie",
            Role = "user",
            PartnershipId = 3,
            CreateDateTime = DateTime.UtcNow,
            CreateUserName = "System"
        };
        await _repo.CreateCheckSetShareAsync(share1);
        await _repo.CreateCheckSetShareAsync(share2);
        await _repo.CreateCheckSetShareAsync(share3);

        var result = await _repo.GetSharesForCheckSetAsync(checkSet1.SetId);

        Assert.AreEqual(2, result.Count);
        Assert.IsTrue(result.Any(s => s.SharedWithUserId == "user-alice"));
        Assert.IsTrue(result.Any(s => s.SharedWithUserId == "user-bob"));
    }

    // DeleteSharesByPartnershipAsync
    [TestMethod]
    public async Task DeleteSharesByPartnershipAsync_RemovesOnlyPartnershipCreatedShares()
    {
        var checkSet = DbContextHelper.SeedCheckSet(_db, "Test Set", "user-owner");

        var share1 = new CheckSetShare
        {
            CheckSetId = checkSet.SetId,
            SharedWithUserId = "user-alice",
            Role = "user",
            PartnershipId = 1,
            CreateDateTime = DateTime.UtcNow,
            CreateUserName = "System"
        };
        var share2 = new CheckSetShare
        {
            CheckSetId = checkSet.SetId,
            SharedWithUserId = "user-bob",
            Role = "user",
            PartnershipId = 1,
            CreateDateTime = DateTime.UtcNow,
            CreateUserName = "System"
        };
        var share3 = new CheckSetShare
        {
            CheckSetId = checkSet.SetId,
            SharedWithUserId = "user-charlie",
            Role = "user",
            PartnershipId = 2,
            CreateDateTime = DateTime.UtcNow,
            CreateUserName = "System"
        };
        await _repo.CreateCheckSetShareAsync(share1);
        await _repo.CreateCheckSetShareAsync(share2);
        await _repo.CreateCheckSetShareAsync(share3);

        await _repo.DeleteSharesByPartnershipAsync(1);

        var remaining = await _db.CheckSetShares.ToListAsync();
        Assert.AreEqual(1, remaining.Count);
        Assert.AreEqual("user-charlie", remaining[0].SharedWithUserId);
        Assert.AreEqual(2, remaining[0].PartnershipId);
    }
}
