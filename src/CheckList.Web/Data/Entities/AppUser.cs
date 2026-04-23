namespace CheckList.Web.Data.Entities;

/// <summary>Stores a cached profile for an Entra ID user.</summary>
public class AppUser
{
    /// <summary>Entra ID object identifier (oid claim) — serves as the primary key.</summary>
    public string UserId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public DateTime CreateDateTime { get; set; }

    public DateTime LastLoginDateTime { get; set; }

    // Navigation
    public ICollection<CheckSet> OwnedCheckSets { get; set; } = [];
    public ICollection<CheckSetShare> CheckSetShares { get; set; } = [];
}
