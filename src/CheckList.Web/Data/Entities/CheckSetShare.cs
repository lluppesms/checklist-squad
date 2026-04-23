namespace CheckList.Web.Data.Entities;

/// <summary>
/// Grants a user access to a checklist set that is owned by another user.
/// </summary>
public class CheckSetShare
{
    public int ShareId { get; set; }

    public int CheckSetId { get; set; }

    /// <summary>Entra ID object identifier of the user being granted access.</summary>
    public string SharedWithUserId { get; set; } = string.Empty;

    /// <summary>Role granted: "admin" (can edit templates) or "user" (can check off items).</summary>
    public string Role { get; set; } = "user";

    public DateTime CreateDateTime { get; set; }

    public string CreateUserName { get; set; } = "UNKNOWN";

    // Navigation
    public CheckSet CheckSet { get; set; } = null!;
    public AppUser SharedWithUser { get; set; } = null!;
}
