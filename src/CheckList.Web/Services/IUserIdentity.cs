namespace CheckList.Web.Services;

public interface IUserIdentity
{
    string? NickName { get; }
    bool HasNickName { get; }
    void SetNickName(string name);

    /// <summary>Entra ID object identifier (oid claim).</summary>
    string? UserId { get; }

    /// <summary>User's email / preferred_username from Entra ID.</summary>
    string? Email { get; }

    /// <summary>True when an Entra ID identity has been set.</summary>
    bool IsAuthenticated { get; }

    /// <summary>Populate identity from an authenticated Entra ID <see cref="ClaimsPrincipal"/>.</summary>
    void SetAuthenticatedUser(ClaimsPrincipal principal);

    /// <summary>Populate identity from explicit Entra ID claim values.</summary>
    void SetAuthenticatedUser(string userId, string displayName, string email);
}
