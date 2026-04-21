namespace CheckList.Web.Services;

public class UserIdentityService : IUserIdentity
{
    public string? NickName { get; private set; }

    public bool HasNickName => !string.IsNullOrWhiteSpace(NickName);

    public void SetNickName(string name)
    {
        NickName = name.Trim();
    }
}
