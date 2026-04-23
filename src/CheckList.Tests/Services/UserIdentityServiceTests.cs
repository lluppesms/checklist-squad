namespace CheckList.Tests.Services;

[TestClass]
public sealed class UserIdentityServiceTests
{
    [TestMethod]
    public void NickName_InitiallyNull()
    {
        var svc = new UserIdentityService();
        Assert.IsNull(svc.NickName);
    }

    [TestMethod]
    public void HasNickName_FalseWhenNotSet()
    {
        var svc = new UserIdentityService();
        Assert.IsFalse(svc.HasNickName);
    }

    [TestMethod]
    public void SetNickName_StoresValue()
    {
        var svc = new UserIdentityService();
        svc.SetNickName("Camper Dave");
        Assert.AreEqual("Camper Dave", svc.NickName);
        Assert.IsTrue(svc.HasNickName);
    }

    [TestMethod]
    public void SetNickName_TrimsWhitespace()
    {
        var svc = new UserIdentityService();
        svc.SetNickName("  Camper Dave  ");
        Assert.AreEqual("Camper Dave", svc.NickName);
    }

    [TestMethod]
    public void SetNickName_CanBeOverwritten()
    {
        var svc = new UserIdentityService();
        svc.SetNickName("Alice");
        svc.SetNickName("Bob");
        Assert.AreEqual("Bob", svc.NickName);
    }

    [TestMethod]
    public void HasNickName_FalseForWhitespaceOnly()
    {
        var svc = new UserIdentityService();
        svc.SetNickName("   ");
        Assert.IsFalse(svc.HasNickName);
    }

    // --- New auth property tests ---

    [TestMethod]
    public void IsAuthenticated_FalseInitially()
    {
        var svc = new UserIdentityService();
        Assert.IsFalse(svc.IsAuthenticated);
    }

    [TestMethod]
    public void UserId_NullInitially()
    {
        var svc = new UserIdentityService();
        Assert.IsNull(svc.UserId);
    }

    [TestMethod]
    public void Email_NullInitially()
    {
        var svc = new UserIdentityService();
        Assert.IsNull(svc.Email);
    }

    [TestMethod]
    public void SetAuthenticatedUser_SetsIsAuthenticated()
    {
        var svc = new UserIdentityService();
        svc.SetAuthenticatedUser("user-123", "Alice", "alice@example.com");
        Assert.IsTrue(svc.IsAuthenticated);
    }

    [TestMethod]
    public void SetAuthenticatedUser_SetsUserId()
    {
        var svc = new UserIdentityService();
        svc.SetAuthenticatedUser("user-123", "Alice", "alice@example.com");
        Assert.AreEqual("user-123", svc.UserId);
    }

    [TestMethod]
    public void SetAuthenticatedUser_SetsEmail()
    {
        var svc = new UserIdentityService();
        svc.SetAuthenticatedUser("user-123", "Alice", "alice@example.com");
        Assert.AreEqual("alice@example.com", svc.Email);
    }

    [TestMethod]
    public void SetAuthenticatedUser_SetsDisplayNameAsNickName_WhenNickNameIsEmpty()
    {
        var svc = new UserIdentityService();
        svc.SetAuthenticatedUser("user-123", "Alice Smith", "alice@example.com");
        Assert.AreEqual("Alice Smith", svc.NickName);
        Assert.IsTrue(svc.HasNickName);
    }

    [TestMethod]
    public void SetAuthenticatedUser_DoesNotOverwriteExistingNickName()
    {
        var svc = new UserIdentityService();
        svc.SetNickName("CamperDave");
        svc.SetAuthenticatedUser("user-123", "Alice Smith", "alice@example.com");
        Assert.AreEqual("CamperDave", svc.NickName);
    }
}
