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
}
