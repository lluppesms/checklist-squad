namespace CheckList.Web.Data.Entities;

/// <summary>
/// Represents a directional partnership between two users.
/// Each partnership creates TWO rows: A grants access to B, and B grants access to A.
/// This allows asymmetric roles if needed in the future.
/// </summary>
public class UserPartnership
{
    public int PartnershipId { get; set; }

    /// <summary>The user who GRANTS access to their checklists.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>The user who RECEIVES access to the granting user's checklists.</summary>
    public string PartnerUserId { get; set; } = string.Empty;

    /// <summary>Role the partner has on this user's lists: "user" or "admin".</summary>
    public string Role { get; set; } = "user";

    /// <summary>Whether new checklists auto-share to this partner.</summary>
    public bool AutoShareEnabled { get; set; } = true;

    /// <summary>Optional provenance: the invite that created this partnership.</summary>
    public int? CreatedFromInviteId { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation
    public AppUser User { get; set; } = null!;
    public AppUser Partner { get; set; } = null!;
    public SharingInvite? CreatedFromInvite { get; set; }
}
