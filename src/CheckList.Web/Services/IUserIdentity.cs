namespace CheckList.Web.Services;

public interface IUserIdentity
{
    string? NickName { get; }
    bool HasNickName { get; }
    void SetNickName(string name);
}
