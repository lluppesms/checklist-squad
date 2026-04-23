namespace CheckList.Web.Services;

public class UserIdentityService : IUserIdentity
{
    private string? _nickName;
    private string? _userId;
    private string? _email;
    private bool _isAuthenticated;

    /// <summary>Display name: auth display name when authenticated, otherwise the manually-set nickname.</summary>
    public string? NickName => _nickName;

    public bool HasNickName => !string.IsNullOrWhiteSpace(NickName);

    public string? UserId => _userId;

    public string? Email => _email;

    public bool IsAuthenticated => _isAuthenticated;

    /// <summary>Set a manual nickname (used in tests and as a fallback for unauthenticated sessions).</summary>
    public void SetNickName(string name) => _nickName = name.Trim();

    /// <summary>
    /// Initialise the service from an authenticated Entra ID user's <see cref="ClaimsPrincipal"/>.
    /// Centralises the Entra ID claim name constants so no other code needs to know them.
    /// </summary>
    public void SetAuthenticatedUser(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                     ?? principal.FindFirst("oid")?.Value
                     ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? string.Empty;
        var displayName = principal.FindFirst("name")?.Value
                          ?? principal.Identity?.Name
                          ?? "User";
        var email = principal.FindFirst("preferred_username")?.Value
                    ?? principal.FindFirst(ClaimTypes.Email)?.Value
                    ?? string.Empty;
        SetAuthenticatedUser(userId, displayName, email);
    }

    /// <summary>Initialise the service from explicit Entra ID claim values.</summary>
    public void SetAuthenticatedUser(string userId, string displayName, string email)
    {
        _userId = userId;
        _email = email;
        _isAuthenticated = true;
        if (string.IsNullOrWhiteSpace(_nickName))
        {
            _nickName = displayName.Trim();
        }
    }
}
