namespace CheckList.Web.Data.Entities;

/// <summary>
/// Represents a pending partnership invitation sent from one user to another.
/// </summary>
public class SharingInvite
{
    public int InviteId { get; set; }

    /// <summary>SHA256 hash of the invite token (128 hex chars). The token itself is never stored.</summary>
    public string InviteTokenHash { get; set; } = string.Empty;

    /// <summary>Entra ID object identifier of the user who created the invite.</summary>
    public string SenderUserId { get; set; } = string.Empty;

    /// <summary>Email address of the intended recipient.</summary>
    public string RecipientEmail { get; set; } = string.Empty;

    /// <summary>Role to grant when accepted: "user" or "admin".</summary>
    public string Role { get; set; } = "user";

    /// <summary>Status: "pending", "accepted", "expired", or "revoked".</summary>
    public string Status { get; set; } = "pending";

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    /// <summary>Filled when the invite is accepted — the UserId of the user who accepted.</summary>
    public string? AcceptedByUserId { get; set; }

    public DateTime? AcceptedAt { get; set; }

    // Navigation
    public AppUser Sender { get; set; } = null!;
    public AppUser? AcceptedByUser { get; set; }
}
